using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameExtensions;
using System;

public class GridController : MonoBehaviour, IGridController
{
    [Tooltip("Whether to draw gizmos for the grid width/length bounds or not.")]
    [SerializeField]
    private bool drawGizmo = true;
    [Tooltip("The color of the gizmo boundaries to draw with.")]
    [SerializeField]
    private Color gizmoColor = Color.green;

    [Tooltip("The length of the grid.")]
    [SerializeField]
    private float gridLength = 10.0f;
    public float GridLength()
    {
        return gridLength;
    }

    [Tooltip("The width of the grid.")]
    [SerializeField]
    private float gridWidth = 10.0f;
    public float GridWidth()
    {
        return gridWidth;
    }

    //public int GridLengthCells { get { return _gridLengthCells; } }
    [SerializeField]
    private int _gridLengthCells = 0;
    //public int GridWidthCells { get { return _gridWidthCells; } }
    [SerializeField]
    private int _gridWidthCells = 0;

    //cached data to avoid expensive lookups
    [Tooltip("The cached material for the grid.")]
    private Material _gridMaterial;
    [Tooltip("The cached rendered for the grid.")]
    private Renderer _gridRenderer;
    public Renderer GridRenderer() {
        return _gridRenderer;
    }
    [Tooltip("The cached grid spacing float value from the grid shader for the grid.")]
    private float _shaderGridSpacing = 0;
    private float _shaderGridSpacing_half = 0;
    public float GridSpacing() {
        return _shaderGridSpacing;
    }

    //private values
    private GridCell[,] gridCells;
    private IGridPathRenderer _pathRenderer;

    public Transform testObj;


    private List<GridLink> _gridLinks;

    public delegate bool ObstructedGridCellFilter(GridCell c);
    private static bool GridCellIsObstacle(GridCell gridCell) {
        bool isObstacle = false;
        //TODO

        return isObstacle;
    }
    public ObstructedGridCellFilter ObstructedGridCell = GridCellIsObstacle;

    private void Awake() {

    }

    // Use this for initialization
    void Start() {
        UpdateCachedReferences();
        UpdateCachedValues();
        Refresh();
        GameObject testCapsule = Resources.Load("TestCapsule") as GameObject;
        GameObject testCapsule_red = Resources.Load("TestCapsule_Red") as GameObject;

        if (testObj != null) {
            bool testPostBasedFunctions = false;
            GridCell testObjCell = GetGridCellFromPoint(testObj.position);

            if (testPostBasedFunctions) {
                Debug.Log("cell from point:" + GetCellIndexFromPoint(testObj.position));

                Vector3 adjPoint_back = GetAdjacentGridSquareFromPos(testObj.position, DIRECTION.BACKWARD);
                Instantiate(testCapsule, adjPoint_back, Quaternion.identity).name = gameObject.name + " - BACK";

                Vector3 adjPoint_right = GetAdjacentGridSquareFromPos(testObj.position, DIRECTION.RIGHT);
                Instantiate(testCapsule_red, adjPoint_right, Quaternion.identity).name = gameObject.name + " - RIGHT";

                Debug.Log("GetAdjacentGridSquare BACK:" + adjPoint_back);
                Debug.Log("adj pos cell BACK:" + GetCellIndexFromPoint(adjPoint_back));
            } else {
                GridCell adjCell_back = GetAdjacentGridCellFromGridCell(testObjCell, DIRECTION.BACKWARD);
                GridCell adjCell_right = GetAdjacentGridCellFromGridCell(testObjCell, DIRECTION.RIGHT);

                Instantiate(testCapsule, adjCell_back.WorldPosition, Quaternion.identity).name = gameObject.name + " - BACK";
                Instantiate(testCapsule_red, adjCell_right.WorldPosition, Quaternion.identity).name = gameObject.name + " - RIGHT";
            }

            //path renderer
            /*
            List<GridVector> pathDirs = new List<GridVector>
            {
                new GridVector(){Direction = DIRECTION.BACKWARD, Distance = 0},
                new GridVector(){Direction = DIRECTION.RIGHT, Distance = 3},
                new GridVector(){Direction = DIRECTION.FORWARD, Distance = 1},
                new GridVector(){Direction = DIRECTION.RIGHT, Distance = 0}
            }; */
            List<GridVector> pathDirs = new List<GridVector>
            {
                new GridVector(){Direction = DIRECTION.BACKWARD, Distance = 0},
                new GridVector(){Direction = DIRECTION.RIGHT, Distance = 1},
                new GridVector(){Direction = DIRECTION.FORWARD, Distance = 0},
                new GridVector(){Direction = DIRECTION.RIGHT, Distance = 1},
                new GridVector(){Direction = DIRECTION.BACKWARD, Distance = 0},
                new GridVector(){Direction = DIRECTION.RIGHT, Distance = 1},
                new GridVector(){Direction = DIRECTION.FORWARD, Distance = 0},
                new GridVector(){Direction = DIRECTION.RIGHT, Distance = 1},
                new GridVector(){Direction = DIRECTION.BACKWARD, Distance = 0},
                new GridVector(){Direction = DIRECTION.RIGHT, Distance = 1},
                new GridVector(){Direction = DIRECTION.FORWARD, Distance = 0},
                new GridVector(){Direction = DIRECTION.RIGHT, Distance = 1},
                new GridVector(){Direction = DIRECTION.BACKWARD, Distance = 0},
                new GridVector(){Direction = DIRECTION.RIGHT, Distance = 1},
                new GridVector(){Direction = DIRECTION.FORWARD, Distance = 0},
                new GridVector(){Direction = DIRECTION.RIGHT, Distance = 1},
                new GridVector(){Direction = DIRECTION.BACKWARD, Distance = 0},
                new GridVector(){Direction = DIRECTION.RIGHT, Distance = 1},
                new GridVector(){Direction = DIRECTION.FORWARD, Distance = 0}
            };
            List<GridCell> gridPath = GetGridCellPath(testObjCell, pathDirs);
            _pathRenderer.RenderPath(gridPath);

        }
    }

    // Update is called once per frame
    void Update() {

    }

    public bool GridCellObstructed(GridCell gridCell) {
        bool isObstructed = false;
        foreach (ObstructedGridCellFilter f in ObstructedGridCell.GetInvocationList()) {
            isObstructed = f(gridCell);
            if(isObstructed) {
                break;
            }
        }

        return isObstructed;
    }

    public void SetGridLength(float l) {
        gridLength = l;
        _gridLengthCells = (int)(l / _shaderGridSpacing_half);
        if((_gridLengthCells%2) == 0) {
            _gridLengthCells--;
        }
    }
    public void SetGridWidth(float w) {
        gridWidth = w;
        _gridWidthCells = (int)(w / _shaderGridSpacing_half);
        if ((_gridWidthCells%2) == 0) {
            _gridWidthCells--;
        }
    }

    public Vector3 GetPointFromCell(int lCellCoord, int wCellCoord) {
        return gridCells[lCellCoord, wCellCoord].WorldPosition;
    }

    /// <summary>
    /// Gets a length/width based coordinate in this grid, based on a Vector3 world position coordinate.    
    /// </summary>
    /// <param name="pos"> A valid grid position </param>
    /// <returns> A Vector2 of the grid coordinates (in length / width order) for the world point in the grid array </returns>
    public Vector2Int? GetCellIndexFromPoint(Vector3 pos) {
        Vector2 resultCell = Vector2.zero;
        Vector3 upperLeft = gridCells[0, 0].WorldPosition;

        if (IsPointOnGrid(pos) ) {
            return new Vector2Int(Mathf.FloorToInt(Mathf.Abs(upperLeft.z - pos.z) / _shaderGridSpacing), Mathf.FloorToInt( Mathf.Abs(upperLeft.x - pos.x) / _shaderGridSpacing));
        } else {
            return null;
        }
    }

    /// <summary>
    /// Gets a length/width based coordinate in this grid, based on a Vector3 world position coordinate.    
    /// </summary>
    /// <param name="pos"> A valid grid position </param>
    /// <returns> A Vector2 of the grid coordinates (in length / width order) for the world point in the grid array </returns>
    public GridCell GetGridCellFromPoint(Vector3 pos) {
        Vector2? resultIdx = GetCellIndexFromPoint(pos);

        GridCell resultGridCell = null;
        if((resultIdx != null) && resultIdx.HasValue) {
            resultGridCell = GetGridCellFromIndex(resultIdx.Value);
        }

        return resultGridCell;
    }

    public Vector3 GetAdjacentGridSquareFromPos(Vector3 pos, DIRECTION dir) {
        return pos + (dir.ToVector3() * _shaderGridSpacing);
    }

    /// <summary>
    /// Gets a length/width based coordinate in this grid, based on a Vector3 world position coordinate.    
    /// </summary>
    /// <param name="idxPair"> A valid grid cell idx (clamped between 0 and the length of the grid dimensions) </param>
    /// <returns> A grid cell at the given indices (or the closest grid cell if the indices are out of range) </returns>
    public GridCell GetGridCellFromIndex(Vector2 idxPair) {
        if(((idxPair.x >= gridCells.GetLength(0)) || (idxPair.x < 0)) || ((idxPair.y >= gridCells.GetLength(1)) || (idxPair.y < 0))) {
            return null;
        }

        int lengthIdx = (int)idxPair.x;
        int widthIdx = (int)idxPair.y;
        
        return gridCells[lengthIdx, widthIdx];
    }

    public GridCell GetAdjacentGridCellFromGridCell(GridCell orig, DIRECTION dir) {
        Vector2 destIdx = orig.GridPosition;
        int destLengthIdx = (int)orig.GridPosition.x;
        int destWidthIdx = (int)orig.GridPosition.y;

        switch (dir) {
            case DIRECTION.LEFT:
                destWidthIdx--;
                break;
            case DIRECTION.RIGHT:
                destWidthIdx++;
                break;
            case DIRECTION.FORWARD:
                destLengthIdx--;
                break;
            case DIRECTION.FORWARD_LEFT:
                destLengthIdx--;
                destWidthIdx--;
                break;
            case DIRECTION.FORWARD_RIGHT:
                destLengthIdx--;
                destWidthIdx++;
                break;
            case DIRECTION.BACKWARD:
                destLengthIdx++;
                break;
            case DIRECTION.BACKWARD_LEFT:
                destLengthIdx++;
                destWidthIdx--;
                break;
            case DIRECTION.BACKWARD_RIGHT:
                destLengthIdx++;
                destWidthIdx++;
                break;
        }
        
        return GetGridCellFromIndex(new Vector2(destLengthIdx, destWidthIdx) );
    }

    //dir is a NON diagonal direction vector
    public List<Vector3> GetGridPositionsInLine(Vector3 pos, GridVector gridVector, bool includeObstructed = false, bool contiguous = true, Func<GridCell, bool> includeFilter = null) {
        List<Vector3> adjGridSquares = new List<Vector3>();

        for (int i = 0; ((i < gridVector.Distance) || (gridVector.Distance == 0)); i++) {
            pos = GetAdjacentGridSquareFromPos(pos, gridVector.Direction);
            GridCell posGridCell = GetGridCellFromPoint(pos);
            if (posGridCell != null) {
                bool includeGridCell = !GridCellObstructed(posGridCell) && (includeFilter==null || includeFilter(posGridCell) );

                if (includeGridCell || includeObstructed) { //if cell wasn't obstructed (or we want to include obstructed ones anyways)
                    adjGridSquares.Add(pos);
                }

                if(!includeGridCell && contiguous) { //if the cell was obstructed, and a contiguous line of grid cells were wanted
                    break;
                }
            } else {
                break;
            }
        }

        return adjGridSquares;
    }

    //dir is a NON diagonal direction vector
    public List<GridCell> GetGridCellsInLine(GridCell cell, GridVector gridVector, bool includeObstructed = false, bool contiguous = true, Func<GridCell, bool> includeFilter = null) {
        List<GridCell> adjGridCells = new List<GridCell>();

        for (int i = 0; ((i<gridVector.Distance) ||(gridVector.Distance == 0)); i++) {
            cell = GetAdjacentGridCellFromGridCell(cell, gridVector.Direction);
            if (cell != null) {
                bool includeGridCell = !GridCellObstructed(cell) && (includeFilter == null || includeFilter(cell));

                if (includeGridCell || includeObstructed) { //if cell wasn't obstructed (or we want to include obstructed ones anyways)
                    adjGridCells.Add(cell);
                }

                if (!includeGridCell && contiguous) { //if the cell was obstructed, and a contiguous line of grid cells were wanted
                    break;
                }
            } else {
                break;
            }
        }

        return adjGridCells;
    }

    public List<GridCell> GetGridCellPath(GridCell start, List<GridVector> movements, bool includeObstructed = false, bool contiguous = true) {
        List<GridCell> path = new List<GridCell>();
        path.Add(start);
        foreach(GridVector v in movements) {
            path.AddRange(GetGridCellsInLine(path.Last<GridCell>(), v, includeObstructed, contiguous) );
        }

        return path;
    }

    public Vector3 GetUpperLeftCell() {
        //this will use the grid's center, and offset it to the left, and up
        //it finds the CORNER of the grid, and then moves back down and to the right half a cell size (to get the center of that cell)
        return new Vector3(transform.position.x - gridWidth + _shaderGridSpacing_half, transform.position.y, transform.position.z + gridLength - _shaderGridSpacing_half);
    }

    public Vector3 GetGridSquareNearest(Vector3 posToSnap) {
        return new Vector3(
            Mathf.Round(posToSnap.x / _shaderGridSpacing) * _shaderGridSpacing,
            0,
            Mathf.Round(posToSnap.z / _shaderGridSpacing) * _shaderGridSpacing
        );
    }

    public bool IsPointOnGrid(Vector3 pos) {
        return (Mathf.Abs(pos.x) < gridWidth) && (Mathf.Abs(pos.z) < gridLength);
    }

    public void UpdateCachedReferences() {
        if (_gridRenderer == null) {
            _gridRenderer = gameObject.GetComponent<Renderer>();
        }

        if (_gridMaterial == null) {
            if (Application.isEditor) {
                _gridMaterial = _gridRenderer.sharedMaterial;
            } else {
                _gridMaterial = _gridRenderer.material;
            }
        }

        if(_pathRenderer == null) {
            _pathRenderer = gameObject.AddComponent<CurvedGridPathRenderer>();
        }
    }

    public void UpdateCachedValues(){
        if(Application.isEditor) {
            _shaderGridSpacing = _gridRenderer.sharedMaterial.GetFloat("_MainGridSpacing");
        } else {
            _shaderGridSpacing = _gridRenderer.material.GetFloat("_MainGridSpacing");
        }

        _shaderGridSpacing_half = _shaderGridSpacing / 2.0f;
    }

    private void OnDrawGizmos() {
        UpdateCachedReferences();
        UpdateCachedValues();

        if (drawGizmo) {
            Gizmos.color = gizmoColor;
            Vector3 widthVector = new Vector3(gridWidth, 0, 0);
            Vector3 lengthVector = new Vector3(0, 0, gridLength);
            Vector3 globalWidthGizmoPosition = transform.position + widthVector;
            Vector3 globalLengthGizmoPosition = transform.position + lengthVector;

            //determine 4 corner points
            Vector3 upperLeft = transform.position - widthVector;
            upperLeft += lengthVector;

            Vector3 upperRight = transform.position + widthVector;
            upperRight += lengthVector;

            Vector3 lowerLeft = transform.position - widthVector;
            lowerLeft -= lengthVector;

            Vector3 lowerRight = transform.position + widthVector;
            lowerRight -= lengthVector;

            Debug.DrawLine(upperLeft, upperRight, gizmoColor);
            Debug.DrawLine(lowerLeft, lowerRight, gizmoColor);
            Debug.DrawLine(upperLeft, lowerLeft, gizmoColor);
            Debug.DrawLine(upperRight, lowerRight, gizmoColor);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(globalWidthGizmoPosition, 2.0f);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(globalLengthGizmoPosition, 2.0f);

        } //end of drawing gizmos

        //now determine "scale" of grid plane

        //get grid spacing value of shader
        float gridSpacing_fromShader = 0;
        if (Application.isEditor) {
            gridSpacing_fromShader = _gridRenderer.sharedMaterial.GetFloat("_MainGridSpacing");
        } else {
            gridSpacing_fromShader = _gridRenderer.material.GetFloat("_MainGridSpacing");
        }

        float gridSpacingUnitToSnapTo = gridSpacing_fromShader + (gridSpacing_fromShader / 2.0f);

        Vector3 extentsOfGrid = _gridRenderer.bounds.extents;
        float xScale = transform.localScale.x * (Mathf.Abs(gridWidth) / extentsOfGrid.x);
        float zScale = transform.localScale.z * (Mathf.Abs(gridLength) / extentsOfGrid.z);
        transform.localScale = new Vector3(xScale, 1, zScale);
    }

    public void Refresh() {
        gridCells = new GridCell[_gridLengthCells, _gridWidthCells];
        gridCells.InitializeElements();

        Vector3 topLeft = GetUpperLeftCell();
        gridCells[0, 0].WorldPosition = topLeft;

        GameObject testCapsule = Resources.Load("TestCapsule_Green") as GameObject;

        for (int l = 0; l < _gridLengthCells; l++) {
            //for each cell in the length of the grid
            for (int w = 0; w < _gridWidthCells; w++) {
                //skip [0,0] -- it's already been initialized
                if(l==0 && w==0) {
                    Instantiate(testCapsule, gridCells[l, w].WorldPosition, Quaternion.identity).name = gameObject.name + " - Origin";
                    continue;
                }

                //for each cell in the width of the grid
                gridCells[l,w].WorldPosition = new Vector3(topLeft.x + (w*_shaderGridSpacing), transform.position.y, topLeft.z - (l*_shaderGridSpacing));
                gridCells[l,w].GridPosition = new Vector2(l, w);

                //Instantiate(testCapsule, gridCells[l,w].WorldPosition, Quaternion.identity);
            }
        }
    }
}
