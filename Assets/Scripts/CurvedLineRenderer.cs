using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent( typeof(LineRenderer) )]
public class CurvedLineRenderer : MonoBehaviour 
{
	[Tooltip("The line material.")]
	[SerializeField]
	private Material lineMaterial;
    public void SetLineMaterial(Material m) { lineMaterial = m; }

	[Tooltip("The size the line segments.")]
	[SerializeField]
	private float lineSegmentSize = 0.15f;
	[Tooltip("The width of the line.")]
	[SerializeField]
	private float lineWidth = 1.0f;
    public void SetLineWidth(float w) { lineWidth = w; }

    [Header("Gizmos")]
	[Tooltip("Whether to show gizmos or not.")]
	[SerializeField]
	private bool showGizmos = true;
	[Tooltip("The size of the gizmos.")]
	[SerializeField]
	private float gizmoSize = 0.1f;
	[Tooltip("The color of the gizmos.")]
	[SerializeField]
	private Color gizmoColor = new Color(1,0,0,0.5f);

	[Tooltip("The points to use for the curve. (local space)")]
	[SerializeField]
	private List<Vector3> linePoints = new List<Vector3>();
	public void SetPointElement(int idx, Vector3 p){
		linePoints[idx] = p;
	}
    public void SetLinePoints(List<Vector3> l) { linePoints = l; }

    private LineRenderer lineRenderer;
	private Transform trans;
	//track if the transform has been updated
	private Vector3 lastPosition;
	private Vector3 lastScale;
	private Quaternion lastRotation;

	public void Awake() {
		UpdateLineRenderer ();
	}
	public void OnEnable(){
		//UpdateLineRenderer();
	}

	// Update is called once per frame
	public void Update () 
	{
		//UpdateLineRenderer ();
	}

	public void UpdateLineRenderer() {
		if (trans == null) {
			trans = transform;
		}

		if (lineRenderer == null) {
			lineRenderer = trans.GetComponent<LineRenderer> ();
		}
		//get smoothed values
		Vector3[] smoothedPoints = LineSmoother.SmoothLine(linePoints, lineSegmentSize);
		for (int i = 0; i < smoothedPoints.Length; i++) {
			//convert the local space points to global space
			smoothedPoints[i] = trans.TransformPoint(smoothedPoints[i]);
		}

		//set line settings
        lineRenderer.positionCount = smoothedPoints.Length;
		lineRenderer.SetPositions(smoothedPoints);
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
		lineRenderer.material = lineMaterial;
	}

	void OnDrawGizmos()
	{
		//settings for gizmos
		foreach(Vector3 point in linePoints)
		{
			Gizmos.color = gizmoColor;
			Gizmos.DrawSphere(transform.TransformPoint(point), gizmoSize);
		}


		if (transform.hasChanged)
		{
			UpdateLineRenderer ();
			transform.hasChanged = false;
		}
	}

	void OnValidate() {
		UpdateLineRenderer ();
	}
}
