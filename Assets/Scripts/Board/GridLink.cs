using UnityEngine;
using System.Collections;

public class GridLink
{
    private GridController _sourceGrid;
    public GridCell sourceGridCell;

    public GridController destinationGrid;
    public GridCell destinationGridCell;
    public DIRECTION destinationOrientation;

    public bool maintainSourceOrientation;

}
