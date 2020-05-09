﻿//(c) copyright by Martin M. Klöckener
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using Wichtel;

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
        Horizontal,
        Vertical
    }
    
    public enum FillDirectionHorizontal
    {
        LeftToRight,
        RightToLeft
    }

    public enum FillDirectionVertical
    {
        TopToBottom,
        BottomToTop
    }

    public Vector2 spacing;
    public FitType fitType;
    public FillDirection fillDirection;
    public FillDirectionHorizontal fillDirectionHorizontal;
    public FillDirectionVertical fillDirectionVertical;
    [Min(1)]public int rows;
    [Min(1)]public int columns;
    public Sizing defaultSizingX;
    public Sizing defaultSizingY;
    public Vector2 fixedCellSize;
    [Min(0.00001f)] public Vector2 cellAspectRatio;
    public bool ignoreInactiveChildren = true;
    public bool useChildScaleX, useChildScaleY;
    public bool childForceExpandX, childForceExpandY;
    public bool fitContentSizeX, fitContentSizeY;

    [SerializeField, HideInInspector] private Sizing sizingXPreviousValue;
    [SerializeField, HideInInspector] private Sizing sizingXcached;
    [SerializeField, HideInInspector] private Sizing sizingYPreviousValue;
    [SerializeField, HideInInspector] private Sizing sizingYcached;

    private readonly List<RectTransform> rectChildrenNew = new List<RectTransform>();
    private readonly List<Component> toIgnoreList = new List<Component>();
    private readonly List<float> childrenSizesX = new List<float>();
    private readonly List<float> childrenSizesY = new List<float>();

    private readonly List<Sizing> sizingsX = new List<Sizing>();
    private readonly List<Sizing> sizingsY = new List<Sizing>();

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
        if ((defaultSizingX == Sizing.FixedAspectRatio && sizingXPreviousValue != Sizing.FixedAspectRatio) || (defaultSizingY == Sizing.FixedAspectRatio && sizingYPreviousValue != Sizing.FixedAspectRatio))
        {
            if(defaultSizingX != Sizing.FixedAspectRatio) sizingXcached = defaultSizingX; //cache sizingX
            if(defaultSizingY != Sizing.FixedAspectRatio) sizingYcached = defaultSizingY; //cache sizingY
            defaultSizingX = Sizing.FixedAspectRatio;
            defaultSizingY = Sizing.FixedAspectRatio;
        }
        //- -
        //- User chose other mode than FixedAspectRatio -
        if ((defaultSizingX != Sizing.FixedAspectRatio && sizingXPreviousValue == Sizing.FixedAspectRatio) || (defaultSizingY != Sizing.FixedAspectRatio && sizingYPreviousValue == Sizing.FixedAspectRatio))
        {
            defaultSizingX = sizingXcached; //restore cacheX
            defaultSizingY = sizingYcached; //restore cacheY
        }
        //- -
        sizingXPreviousValue = defaultSizingX;
        sizingYPreviousValue = defaultSizingY;
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

        //-- LayoutElements --
        var layoutElements = new UltimateLayoutElement[rectChildrenNew.Count];

        sizingsX.Clear();
        sizingsY.Clear();
        for (int i = 0; i < rectChildrenNew.Count; i++)
        {
            layoutElements[i] = rectChildrenNew[i].GetComponent<UltimateLayoutElement>();
            var sizingX = (layoutElements[i] == null) ? defaultSizingX : layoutElements[i].sizingX;
            var sizingY = (layoutElements[i] == null) ? defaultSizingY : layoutElements[i].sizingY;
            sizingsX.Add(sizingX);
            sizingsY.Add(sizingY);
        }
        //-- --
        
        //-- CellSize --
        for (int i = 0; i < rectChildrenNew.Count; i++)
        {
            if      (sizingsX[i] == Sizing.KeepChildSize) childrenSizesX.Add(rectChildrenNew[i].rect.width);
            else if (sizingsX[i] == Sizing.Fit)           childrenSizesX.Add(gridCellWidth);
            else if (sizingsX[i] == Sizing.FixedSize)     childrenSizesX.Add(fixedCellSize.x);
            
            if      (sizingsY[i] == Sizing.KeepChildSize) childrenSizesY.Add(rectChildrenNew[i].rect.height);
            else if (sizingsY[i] == Sizing.Fit)           childrenSizesY.Add(gridCellHeight);
            else if (sizingsY[i] == Sizing.FixedSize)     childrenSizesY.Add(fixedCellSize.y);

            if (sizingsX[i] == Sizing.FixedAspectRatio && sizingsX[i] == Sizing.FixedAspectRatio)
            {
                float childAspectRatio = cellAspectRatio.x / cellAspectRatio.y;
                float gridAspectRatio = gridCellWidth / gridCellHeight;
                    
                gridCellWidth = (gridAspectRatio <= childAspectRatio) ? gridCellWidth : (gridCellHeight * childAspectRatio);
                gridCellHeight = (gridAspectRatio >= childAspectRatio) ? gridCellHeight : (gridCellWidth / childAspectRatio);
                    
                childrenSizesX.Add(gridCellWidth);
                childrenSizesY.Add(gridCellHeight);
            }
        }
        //-- --
        
        //-- CellPosition --
        //-- Get grid Height and Width --
        float[] columnWidths = new float[columns];
        float[] rowHeights = new float[rows];
        
        for (int i = 0; i < rectChildrenNew.Count; i++)
        {
            (int rowCount, int columnCount) = CalculateRowAndColumnCount(i);

            columnWidths[columnCount] = Mathf.Max(columnWidths[columnCount], childrenSizesX[i]);
            rowHeights[rowCount] = Mathf.Max(rowHeights[rowCount], childrenSizesY[i]);
        }
        
        //-- --
        
        //- ChildForceExpand & fitToContentSize-
        float totalCellWidthOfOneRow = columnWidths.Sum();
        float totalRowWidth = totalCellWidthOfOneRow + (spacing.x * (float)(columns-1)) + padding.left + padding.right;

        float totalCellHeightOfOneColumn = rowHeights.Sum();
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
            (int rowCount, int columnCount) = CalculateRowAndColumnCount(i);

            var item = rectChildrenNew[i];

            float cellWidth = columnWidths[columnCount];
            float cellHeight = rowHeights[rowCount];

            //-- ChildAlignment --
            #region ChildAlignment
            float alignmentOffsetX = 0f;
            float alignmentOffsetY = 0f;
            if (childAlignment == TextAnchor.LowerCenter || childAlignment == TextAnchor.MiddleCenter || childAlignment == TextAnchor.UpperCenter)
            {
                alignmentOffsetX = (columnWidths[columnCount] / 2f) - (childrenSizesX[i] / 2f);
            }
            if (childAlignment == TextAnchor.LowerRight || childAlignment == TextAnchor.MiddleRight || childAlignment == TextAnchor.UpperRight)
            {
                alignmentOffsetX = columnWidths[columnCount] - childrenSizesX[i];
            }
            if (childAlignment == TextAnchor.MiddleLeft || childAlignment == TextAnchor.MiddleCenter || childAlignment == TextAnchor.MiddleRight)
            {
                alignmentOffsetY = (rowHeights[rowCount] / 2f) - (childrenSizesY[i] / 2f);
            }
            if (childAlignment == TextAnchor.LowerLeft || childAlignment == TextAnchor.LowerCenter || childAlignment == TextAnchor.LowerRight)
            {
                alignmentOffsetY = rowHeights[rowCount] - childrenSizesY[i];
            }
            #endregion
            //-- --

            float xPos = columnWidths.Take(columnCount).Sum();
            float yPos = rowHeights.Take(rowCount).Sum();

            xPos += (spacing.x * columnCount) + padding.left + ((excessParentWidth/Mathf.Max((columns-1f), 1f))*columnCount) + alignmentOffsetX;
            yPos += (spacing.y * rowCount) + padding.top + ((excessParentHeight/Mathf.Max((rows-1f), 1f))*rowCount) + alignmentOffsetY;
            
            if(defaultSizingX != Sizing.KeepChildSize) SetChildAlongAxis(item, 0, xPos, cellWidth);
            else                                       SetChildAlongAxis(item, 0, xPos);
                
            if(defaultSizingY != Sizing.KeepChildSize) SetChildAlongAxis(item, 1, yPos, cellHeight);
            else                                       SetChildAlongAxis(item, 1, yPos);
        }
        #endregion
    }

    private (int, int) CalculateRowAndColumnCount(int i)
    {
        int rowCount;
        int columnCount;


        int originRow;
        int originColumn;

        circularInt bla = new circularInt();

        int blaa = bla;

        circularInt asdf = 5 + bla + 5;

        bool asdfd = (3 == bla);

        //BLABLABLABLA ToDO: hier weiter arbeiten :D

        if (fillDirection == FillDirection.Horizontal)
        {
            if (fillDirectionHorizontal == FillDirectionHorizontal.LeftToRight) columnCount = i % columns;
            else                                                                columnCount = columns-1-(i % columns);
        
            if (fillDirectionVertical == FillDirectionVertical.TopToBottom) rowCount = i / columns;
            else                                                            rowCount = rows-1-(i/columns);
        }
        else
        {
            if (fillDirectionVertical == FillDirectionVertical.TopToBottom) rowCount = i % rows;
            else                                                            rowCount = rows-1-(i % rows);
        
            if (fillDirectionHorizontal == FillDirectionHorizontal.LeftToRight) columnCount = i / rows;
            else                                                                columnCount = columns-1-(i/rows);
        }
        
        return (rowCount, columnCount);
    }

    public override void CalculateLayoutInputVertical(){}
    public override void SetLayoutHorizontal(){}
    public override void SetLayoutVertical(){}
}
}