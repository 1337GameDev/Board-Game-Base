using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapToGrid : MonoBehaviour {
    [Tooltip("The grid this object is bound to.")]
    [SerializeField]
    public GridController grid;
    [Tooltip("Snap this object so it stays on the grid.")]
    [SerializeField]
    public bool constrainToGrid;

    //cached data to avoid expensive lookups
    private float _shaderGridSpacing = 0;
    private float _shaderGridSpacing_half = 0;
     
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void UpdateCachedValues(){
        grid.UpdateCachedReferences();
        grid.UpdateCachedValues();
        _shaderGridSpacing = grid.GridSpacing();
        _shaderGridSpacing_half = _shaderGridSpacing / 2.0f;
    }

    private void OnDrawGizmos()
    {
    }
}
