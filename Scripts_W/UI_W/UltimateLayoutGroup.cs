//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Haferbrei {
public class UltimateLayoutGroup : LayoutGroup
{
    public enum FitType
    {
        Uniform,
        Width,
        Height,
        FixedRows,
        FixedColumns
    }

    public enum Sizing
    {
        KeepChildSize,
        Fit,
        FixedSize,
        FixedAspectRatio
    }

    public enum FillDirection
    {
        Horizontally,
        Vertically
    }

    public enum Grid
    {
        Fixed,
        Dynamic
    }
    
    public Vector2 spacing;
    public FitType fitType;
    public FillDirection fillDirection;
    [Min(1)]public int rows;
    [Min(1)]public int columns;
    public Sizing sizingX;
    public Sizing sizingY;
    public Grid gridX, gridY;
    public Vector2 fixedCellSize;
    [Min(0.00001f)] public Vector2 cellAspectRatio;
    public bool ignoreInactiveChildren = true;
    public bool useChildScaleX, useChildScaleY;
    public bool childForceExpandX, childForceExpandY;
    public bool allowOverlapX, allowOverlapY;
    public bool fitContentSizeX, fitContentSizeY;

    [SerializeField, HideInInspector] private Sizing sizingXPreviousValue;
    [SerializeField, HideInInspector] private Sizing sizingXcached;
    [SerializeField, HideInInspector] private Sizing sizingYPreviousValue;
    [SerializeField, HideInInspector] private Sizing sizingYcached;

    private bool keepChildSize_DynamicGridX => (sizingX == Sizing.KeepChildSize && gridX == Grid.Dynamic);
    private bool keepChildSize_DynamicGridY => (sizingY == Sizing.KeepChildSize && gridY == Grid.Dynamic);
    private bool keepChildSize_FixedGridX => (sizingX == Sizing.KeepChildSize && gridX == Grid.Fixed);
    private bool keepChildSize_FixedGridY => (sizingY == Sizing.KeepChildSize && gridY == Grid.Fixed);
    
    private readonly List<RectTransform> rectChildrenNew = new List<RectTransform>();
    private readonly List<Component> toIgnoreList = new List<Component>();
    private readonly List<float> childrenSizesX = new List<float>();
    private readonly List<float> childrenSizesY = new List<float>();

    public override void CalculateLayoutInputHorizontal()
    {
        //base.CalculateLayoutInputHorizontal();
        
        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        //-- Get children --
        #region GetChildren
        rectChildrenNew.Clear();
        toIgnoreList.Clear();
        for (int i = 0; i < rectTransform.childCount; i++)
        {
            var rect = rectTransform.GetChild(i) as RectTransform;
            if (rect == null) continue;
            if (ignoreInactiveChildren && !rect.gameObject.activeInHierarchy) continue;
            
            rect.GetComponents(typeof(ILayoutIgnorer), toIgnoreList);

            if (toIgnoreList.Count == 0)
            {
                rectChildrenNew.Add(rect);
                continue;
            }

            foreach (Component component in toIgnoreList)
            {
                var ignorer = (ILayoutIgnorer)component;
                if (!ignorer.ignoreLayout)
                {
                    rectChildrenNew.Add(rect);
                    break;
                }
            }
        }
        #endregion
        //-- --
        
        childrenSizesX.Clear();
        childrenSizesY.Clear();
        
        //-- fixed AspectRatio requires X and Y to be set to AspectRatio --
        #region FixedAspectRatio
        //- User chose FixedAspectRatio
        if ((sizingX == Sizing.FixedAspectRatio && sizingXPreviousValue != Sizing.FixedAspectRatio) || (sizingY == Sizing.FixedAspectRatio && sizingYPreviousValue != Sizing.FixedAspectRatio))
        {
            if(sizingX != Sizing.FixedAspectRatio) sizingXcached = sizingX; //cache sizingX
            if(sizingY != Sizing.FixedAspectRatio) sizingYcached = sizingY; //cache sizingY
            sizingX = Sizing.FixedAspectRatio;
            sizingY = Sizing.FixedAspectRatio;
        }
        //- -
        //- User chose other mode than FixedAspectRatio -
        if ((sizingX != Sizing.FixedAspectRatio && sizingXPreviousValue == Sizing.FixedAspectRatio) || (sizingY != Sizing.FixedAspectRatio && sizingYPreviousValue == Sizing.FixedAspectRatio))
        {
            sizingX = sizingXcached; //restore cacheX
            sizingY = sizingYcached; //restore cacheY
        }
        //- -
        sizingXPreviousValue = sizingX;
        sizingYPreviousValue = sizingY;
        #endregion
        //-- --

        //foreach (var rectTransform in rectChildrenNew)
        //{
        //    var preferredWidth = LayoutUtility.GetPreferredWidth(rectTransform);
        //    Debug.Log(preferredWidth);
        //}
        
        #region Calculation
        if (fitType == FitType.Width || fitType == FitType.Height || fitType == FitType.Uniform)
        {
            float sqrRt = Mathf.Sqrt(rectChildrenNew.Count);
            rows = Mathf.CeilToInt(sqrRt);
            columns = Mathf.CeilToInt(sqrRt);
        }
        
        if (fitType == FitType.Width  || fitType == FitType.FixedColumns)
        {
            rows = Mathf.CeilToInt(rectChildrenNew.Count / (float)columns);
        }
        else if (fitType == FitType.Height || fitType == FitType.FixedRows)
        {
            columns = Mathf.CeilToInt(rectChildrenNew.Count / (float)rows);
        }
            
            
        float gridCellWidth = (parentWidth / (float)columns) - (spacing.x * (float)(columns-1) / (float)columns) - (padding.left / (float)columns) - (padding.right / (float)columns);
        float gridCellHeight = (parentHeight / (float)rows) - (spacing.y * (float)(rows-1) / (float)rows) - (padding.top / (float)rows) - (padding.bottom / (float)rows);
            
        //cellSizeX
        #region Uniform_CellSizeX
        if (sizingX == Sizing.KeepChildSize)
        {
            foreach (var child in rectChildrenNew)
            {
                childrenSizesX.Add(child.rect.width);
            }
        }
        else if (sizingX == Sizing.Fit)
        {
            foreach (var child in rectChildrenNew)
            {
                childrenSizesX.Add(gridCellWidth);
            }
        }
        else if (sizingX == Sizing.FixedSize)
        {
            foreach (var child in rectChildrenNew)
            {
                childrenSizesX.Add(fixedCellSize.x);
            }
        }
        #endregion

        //cellSizeY
        #region Uniform_CellSizeY
        if (sizingY == Sizing.KeepChildSize)
        {
            foreach (var child in rectChildrenNew)
            {
                childrenSizesY.Add(child.rect.height);
            }
        }
        else if (sizingY == Sizing.Fit)
        {
            foreach (var child in rectChildrenNew)
            {
                childrenSizesY.Add(gridCellHeight);
            }
        }
        else if (sizingY == Sizing.FixedSize)
        {
            foreach (var child in rectChildrenNew)
            {
                childrenSizesY.Add(fixedCellSize.y);
            }
        }
        #endregion
        //cellSizeX&Y (FixedAspectRatio)
        #region Uniform_CellSizeX&Y (FixedAspectRatio)
        if (sizingX == Sizing.FixedAspectRatio || sizingY == Sizing.FixedAspectRatio)
        {
            foreach (var child in rectChildrenNew)
            {
                float childAspectRatio = cellAspectRatio.x / cellAspectRatio.y;
                float gridAspectRatio = gridCellWidth / gridCellHeight;
                    
                gridCellWidth = (gridAspectRatio <= childAspectRatio) ? gridCellWidth : (gridCellHeight * childAspectRatio);
                gridCellHeight = (gridAspectRatio >= childAspectRatio) ? gridCellHeight : (gridCellWidth / childAspectRatio);
                    
                childrenSizesX.Add(gridCellWidth);
                childrenSizesY.Add(gridCellHeight);
            }
        }
        #endregion
            
        //-- CellPosition --
        //-- Get dynamic grid values --
        float[] columnWidths = new float[columns];
        float[] rowHeights = new float[rows];
        if (keepChildSize_DynamicGridX || keepChildSize_DynamicGridY)
        {
            for (int i = 0; i < rectChildrenNew.Count; i++)
            {
                int rowCount;
                int columnCount;
                if (fillDirection == FillDirection.Horizontally)
                {
                    rowCount = i / columns;
                    columnCount = i % columns;
                }
                else
                {
                    columnCount = i / rows;
                    rowCount = i % rows;
                }

                columnWidths[columnCount] = Mathf.Max(columnWidths[columnCount], childrenSizesX[i]);
                rowHeights[rowCount] = Mathf.Max(rowHeights[rowCount], childrenSizesY[i]);
            }
        }
        //-- --
        
        //- ChildForceExpand & fitToContentSize-
        float totalCellWidthOfOneRow = keepChildSize_DynamicGridX ? columnWidths.Sum() : childrenSizesX.Take(columns).Sum();
        float totalRowWidth = totalCellWidthOfOneRow + (spacing.x * (float)(columns-1)) + padding.left + padding.right;

        float totalCellHeightOfOneColumn = keepChildSize_DynamicGridY ? rowHeights.Sum() :childrenSizesY.Take(rows).Sum();
        float totalColumnHeight = totalCellHeightOfOneColumn + (spacing.y * (float) (rows - 1)) + padding.bottom + padding.top;

        float excessParentWidth = 0f;
        float excessParentHeight = 0f;
            
        if (fitContentSizeX) //fitContentSize
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, totalRowWidth);
        }
        else //ChildForceExpand
        {
            excessParentWidth = parentWidth - totalRowWidth;
            if (excessParentWidth < 0f) excessParentWidth = 0f;
            excessParentWidth = childForceExpandX ? excessParentWidth : 0f;
        }

        if (fitContentSizeY) //fitContentSize
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalColumnHeight);
        }
        else //ChildForceExpand
        {
            excessParentHeight = parentHeight - totalColumnHeight;
            if (excessParentHeight < 0f) excessParentHeight = 0f;
            excessParentHeight = childForceExpandY ? excessParentHeight : 0f;
        }
        //- -

        for (int i = 0; i < rectChildrenNew.Count; i++)
        {
            int rowCount;
            int columnCount;
            if (fillDirection == FillDirection.Horizontally)
            {
                rowCount = i / columns;
                columnCount = i % columns;
            }
            else
            {
                columnCount = i / rows;
                rowCount = i % rows;
            }

            var item = rectChildrenNew[i];

            float cellWidth = childrenSizesX[i];
            float cellHeight = childrenSizesY[i];

            if (keepChildSize_FixedGridX) cellWidth = gridCellWidth;
            if (keepChildSize_FixedGridY) cellHeight = gridCellHeight;
            if (keepChildSize_DynamicGridX) cellWidth = columnWidths[columnCount];
            if (keepChildSize_DynamicGridY) cellHeight = rowHeights[rowCount];

            //-- ChildAlignment --
            #region ChildAlignment
            float alignmentOffsetX = 0f;
            float alignmentOffsetY = 0f;
            if (childAlignment == TextAnchor.LowerCenter || childAlignment == TextAnchor.MiddleCenter || childAlignment == TextAnchor.UpperCenter)
            {
                alignmentOffsetX = keepChildSize_DynamicGridX ? ((columnWidths[columnCount] / 2f) - (childrenSizesX[i] / 2f)) : (gridCellWidth / 2f) - (childrenSizesX[i] / 2f);
            }
            if (childAlignment == TextAnchor.LowerRight || childAlignment == TextAnchor.MiddleRight || childAlignment == TextAnchor.UpperRight)
            {
                alignmentOffsetX = keepChildSize_DynamicGridX ? (columnWidths[columnCount] - childrenSizesX[i]) : (gridCellWidth - childrenSizesX[i]);
            }
            if (childAlignment == TextAnchor.MiddleLeft || childAlignment == TextAnchor.MiddleCenter || childAlignment == TextAnchor.MiddleRight)
            {
                alignmentOffsetY = keepChildSize_DynamicGridY ? (rowHeights[rowCount]/2f - childrenSizesY[i]/2f) : (gridCellHeight / 2f) - (childrenSizesY[i] / 2f);
            }
            if (childAlignment == TextAnchor.LowerLeft || childAlignment == TextAnchor.LowerCenter || childAlignment == TextAnchor.LowerRight)
            {
                alignmentOffsetY = keepChildSize_DynamicGridY ? rowHeights[rowCount] - childrenSizesY[i] : gridCellHeight - childrenSizesY[i];
            }
            #endregion
            //-- --

            float xPos = (allowOverlapX && (parentWidth < totalRowWidth)) ? (gridCellWidth * columnCount) : (cellWidth * columnCount);
            float yPos = (allowOverlapY && (parentHeight < totalColumnHeight)) ? (gridCellHeight * rowCount) : (cellHeight * rowCount);

            if (keepChildSize_DynamicGridX) xPos = columnWidths.Take(columnCount).Sum();
            if (keepChildSize_DynamicGridY) yPos = rowHeights.Take(rowCount).Sum();

            xPos += (spacing.x * columnCount) + padding.left + ((excessParentWidth/Mathf.Max((columns-1f), 1f))*columnCount) + alignmentOffsetX;
            yPos += (spacing.y * rowCount) + padding.top + ((excessParentHeight/Mathf.Max((rows-1f), 1f))*rowCount) + alignmentOffsetY;
            
            if(sizingX != Sizing.KeepChildSize) SetChildAlongAxis(item, 0, xPos, cellWidth);
            else                                SetChildAlongAxis(item, 0, xPos);
                
            if(sizingY != Sizing.KeepChildSize) SetChildAlongAxis(item, 1, yPos, cellHeight);
            else                                SetChildAlongAxis(item, 1, yPos);
        }
        #endregion
    }

    public override void CalculateLayoutInputVertical(){}
    public override void SetLayoutHorizontal(){}
    public override void SetLayoutVertical(){}
}
}