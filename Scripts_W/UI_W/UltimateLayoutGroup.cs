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
    
    public Vector2 spacing;
    public FitType fitType;
    public FillDirection fillDirection;
    [Min(1)]public int rows;
    [Min(1)]public int columns;
    public Sizing sizingX;
    public Sizing sizingY;
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
    
    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
        
        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        var rectChildrenNew = ignoreInactiveChildren ? rectChildren.Where(_rectTrans => _rectTrans.gameObject.activeSelf).ToList() : rectChildren;

        var childrenSizesX = new List<float>();
        var childrenSizesY = new List<float>();
        
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

        #region Uniform, Width, Height

        if (fitType == FitType.Width || fitType == FitType.Height || fitType == FitType.Uniform)
        {
            float sqrRt = Mathf.Sqrt(rectChildren.Count);
            rows = Mathf.CeilToInt(sqrRt);
            columns = Mathf.CeilToInt(sqrRt);
        }
            
        if (fitType == FitType.Width  || fitType == FitType.FixedColumns)
        {
            rows = Mathf.CeilToInt(rectChildren.Count / (float)columns);
        }
        else if (fitType == FitType.Height || fitType == FitType.FixedRows)
        {
            columns = Mathf.CeilToInt(rectChildren.Count / (float)rows);
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
        //- ChildForceExpand & fitToContentSize-
        float totalCellWidthOfOneRow = childrenSizesX.Take(columns).Sum();
        float totalRowWidth = totalCellWidthOfOneRow + (spacing.x * (float)(columns-1)) + padding.left + padding.right;
            
        float totalCellHeightOfOneColumn = childrenSizesY.Take(rows).Sum();
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
            
        /* für wenn nicht alle Zeilen gleich breit sind
        for (int i = 0; i < rows; i++)
        {
            float totalCellWidthInRowi = cellSizesX.Skip(i * columns).Take(columns).Sum();
            float excessParentWidthInRowi = parentWidth - totalCellWidthInRowi - (spacing.x * (float) (columns - 1)) - padding.left - padding.right;
            if (excessParentWidthInRowi < 0f) excessParentWidthInRowi = 0f;
            excessParentWidths.Add(childForceExpandX ? excessParentWidthInRowi : 0f);
        }*/
        
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

            float cellWidth = (sizingX == Sizing.KeepChildSize) ? gridCellWidth : childrenSizesX[i];
            float cellHeight = (sizingY == Sizing.KeepChildSize) ? gridCellHeight : childrenSizesY[i];
            
            //-- ChildAlignment --
            #region ChildAlignment
            float alignmentOffsetX = 0f;
            float alignmentOffsetY = 0f;
            if (childAlignment == TextAnchor.LowerCenter || childAlignment == TextAnchor.MiddleCenter || childAlignment == TextAnchor.UpperCenter)
            {
                alignmentOffsetX = (gridCellWidth / 2f) - (childrenSizesX[i] / 2f);
            }
            if (childAlignment == TextAnchor.LowerRight || childAlignment == TextAnchor.MiddleRight || childAlignment == TextAnchor.UpperRight)
            {
                alignmentOffsetX = gridCellWidth - childrenSizesX[i];
            }
            if (childAlignment == TextAnchor.MiddleLeft || childAlignment == TextAnchor.MiddleCenter || childAlignment == TextAnchor.MiddleRight)
            {
                alignmentOffsetY = (gridCellHeight / 2f) - (childrenSizesY[i] / 2f);
            }
            if (childAlignment == TextAnchor.LowerLeft || childAlignment == TextAnchor.LowerCenter || childAlignment == TextAnchor.LowerRight)
            {
                alignmentOffsetY = gridCellHeight - childrenSizesY[i];
            }
            #endregion
            //-- --

            float xPos = (allowOverlapX && (parentWidth < totalRowWidth)) ? (gridCellWidth * columnCount) : (cellWidth * columnCount);
            float yPos = (allowOverlapY && (parentHeight < totalColumnHeight)) ? (gridCellHeight * rowCount) : (cellHeight * rowCount);
            
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