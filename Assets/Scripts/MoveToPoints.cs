using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoveToPoints : MonoBehaviour {
	private Transform trans;

	[Tooltip("Whether to draw gizmos for the movement vector, or not")]
	[SerializeField]
	private bool drawGizmo = true;
	[Tooltip("The color of the direction gizmo to draw.")]
	[SerializeField]
	private Color gizmoPathColor = Color.green;
	[Tooltip("The color of the path point gizmo to draw.")]
	[SerializeField]
	private Color gizmoPointColor = Color.red;
	[Tooltip("The speed to move along the points.")]
	[SerializeField]
	private float moveSpeed = 10.0f;
	[Tooltip("Whether to retrace the path after reaching the end.")]
	[SerializeField]
	private bool retracePath = false;
	[Tooltip("Whether to loop back to the beginning after following the path.")]
	[SerializeField]
	private bool loopPath = false;
	[Tooltip("Whether to rotate the object towards the next point when following the path.")]
	[SerializeField]
	private bool lookAtNextPoint = false;

	[Tooltip("The path of points to follow.")]
	[SerializeField]
	private List<Vector3> path;
	public List<Vector3> GetPath() {
		return path;
	}

	private int currentPointIdx = 0;
	private float nextPointTolerance = 0.1f;
	private bool isReversing = false;

	public void SetPointElement(int idx, Vector3 p) {
		path[idx] = p;
	}

	void Awake() {
		trans = transform;

		if (path == null) {
			path = new List<Vector3>();
		}
	}

	void OnEnable() {
		if(path.Count > 0) {
			trans.position = path[0];
		}
	}

	// Use this for initialization
	void Start() {
		
	}
	
	// Update is called once per frame
	void Update() {
		if((path.Count > 0) && (currentPointIdx < path.Count) && (currentPointIdx>=0)) {
			if ((path [currentPointIdx] - trans.position).sqrMagnitude <= nextPointTolerance) {//reached next point
				trans.position = path [currentPointIdx];

				if (retracePath && (currentPointIdx == path.Count - 1)) {//if we should retrace AND are at the last position
					isReversing = true;
				} else if(loopPath && (currentPointIdx == 0)) {//(if we should loop AND we are at the first point)
					isReversing = false;
				} else if(loopPath && !retracePath && (currentPointIdx == path.Count - 1)) {//if we are supposed to loop, AND not retrace (AND at the end)
					currentPointIdx = 0;
					trans.position = path[currentPointIdx];
				}

				if (isReversing) {
					currentPointIdx--;
				} else {
					currentPointIdx++;
				}

				if ((currentPointIdx<path.Count) && (currentPointIdx >= 0) && lookAtNextPoint) {//if current index is in range
					trans.LookAt (path [currentPointIdx]);
				}
			} else {//in between points, so keep moving along the path
				trans.position = Vector3.MoveTowards(trans.position, path[currentPointIdx], Time.deltaTime*moveSpeed);
			}
		}
	}

	void OnDisable() {
		
	}

	private void OnDrawGizmos() {
		if (drawGizmo) {
			if ((path != null) && path.Count > 1) {
				//draw lines between all points
				//loop through all points, ending at one point before the last
				Gizmos.color = gizmoPointColor;
				for(int i=0;i<path.Count-1;i++) {
					Debug.DrawLine (path[i], path[i+1], gizmoPathColor);
					Gizmos.DrawSphere (path[i], 0.1f);
				}
				Gizmos.DrawSphere(path[path.Count-1], 0.1f);

			}
		}
	}


}
