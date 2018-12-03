using System;
using UnityEngine;

public interface IGridController
{
    float GridLength();
    float GridWidth();
    float GridSpacing();

    void SetGridLength(float l);
    void SetGridWidth(float w);
    Vector3 GetPointFromCell(int lCellCoord, int wCellCoord);
    Vector2Int? GetCellIndexFromPoint(Vector3 pos);
    Vector3 GetAdjacentGridSquareFromPos(Vector3 pos, DIRECTION dir);
    Vector3 GetUpperLeftCell();
    Vector3 GetGridSquareNearest(Vector3 posToSnap);
    bool IsPointOnGrid(Vector3 pos);
    void UpdateCachedReferences();
    void UpdateCachedValues();


}
