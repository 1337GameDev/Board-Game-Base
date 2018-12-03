using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CurvedLineRenderer))]
public class CurvedGridPathRenderer : MonoBehaviour, IGridPathRenderer {
    private CurvedLineRenderer lineRenderer;

    private const float defaultLineWidth = 1.0f;
    private Material defaultMaterial;
    // Start is called before the first frame update
    void Awake() {
        lineRenderer = GetComponent<CurvedLineRenderer>();
        defaultMaterial = Resources.Load("LineMaterials/CurvedLineMaterial") as Material;
        lineRenderer.SetLineWidth(defaultLineWidth);
        lineRenderer.SetLineMaterial(defaultMaterial);

    }

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void RenderPath(List<GridCell> path) {
        List<Vector3> pathPoints = path.ConvertAll(c => gameObject.transform.InverseTransformPoint(c.WorldPosition) );
        lineRenderer.SetLinePoints(pathPoints);
    }
}
