using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardController : MonoBehaviour, IGameBoardController {
    [Tooltip("The grid of this gameboard.")]
    [SerializeField]
    public IGridController grid;

    /// <summary>
    /// The list of tiles ocuppying this grid cell
    /// </summary>
    public BoardTile[] Tiles;

    private void Awake() {
        UpdateCachedReferences();
        UpdateCachedValues();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void UpdateCachedReferences() {

    }

    public void UpdateCachedValues(){

    }

}
