using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridController))]
public class GridControllerEditor : Editor {

    private float _handleSnap = 0.1f;

    public SerializedProperty
    drawGizmo,
    gizmoColor,
    gridLength,
    gridWidth,
    _gridWidthCells,
    _gridLengthCells,
    _gridJumps,
    testObj;

	public void OnEnable() {
        drawGizmo = serializedObject.FindProperty("drawGizmo");
        gizmoColor = serializedObject.FindProperty("drawGizmo");
        gridLength = serializedObject.FindProperty("gridLength");
        gridWidth = serializedObject.FindProperty("gridWidth");
        _gridWidthCells = serializedObject.FindProperty("_gridWidthCells");
        _gridLengthCells = serializedObject.FindProperty("_gridLengthCells");
        _gridJumps = serializedObject.FindProperty("_gridJumps");

        testObj = serializedObject.FindProperty ("testObj");

	}

	public override void OnInspectorGUI() {
		serializedObject.Update ();

		EditorGUILayout.PropertyField (drawGizmo);
		bool drawGizmoValue = drawGizmo.boolValue;

        EditorGUILayout.PropertyField(gridLength);
        EditorGUILayout.PropertyField(gridWidth);
        EditorGUILayout.PropertyField(_gridWidthCells);
        EditorGUILayout.PropertyField(_gridLengthCells);
        EditorGUILayout.PropertyField(testObj);

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Grid Jumps", GUI.skin.textField);
        //RenderListElementsWithButtons(path);

        serializedObject.ApplyModifiedProperties();
	}

	private void OnSceneGUI () {
        Tools.current = Tool.None;

        GridController gridController = target as GridController;
        Transform gridTransform = gridController.transform;
        Vector3 gridOriginPosition = gridTransform.position;
        Quaternion gridRotation = gridTransform.rotation;

		Handles.color = Color.white;

        //get grid spacing value of shader
        float gridSpacing_fromShader = 0.0f;
        if (Application.isEditor) {
            gridSpacing_fromShader = gridController.gameObject.GetComponent<Renderer>().sharedMaterial.GetFloat("_MainGridSpacing");
        } else {
            gridSpacing_fromShader = gridController.gameObject.GetComponent<Renderer>().material.GetFloat("_MainGridSpacing");
        }

        float gridSpacingUnitToSnapTo = gridSpacing_fromShader / 2.0f;

        Vector3 widthVector = new Vector3(gridWidth.floatValue, 0, 0);
        Vector3 lengthVector = new Vector3(0, 0, gridLength.floatValue);

        //replace transform move tool so it snaps to grid
        EditorGUI.BeginChangeCheck();

        Vector3 newTransPos = Handles.DoPositionHandle(gridOriginPosition, gridRotation);
        newTransPos = new Vector3(Mathf.Round(newTransPos.x / gridSpacing_fromShader) * gridSpacing_fromShader, 0, Mathf.Round(newTransPos.z / gridSpacing_fromShader) * gridSpacing_fromShader);
        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(gridTransform, "Move Grid Transform");
            EditorUtility.SetDirty(gridTransform);
            gridTransform.position = newTransPos;
        }

        //first gizmo handle
        Vector3 lengthGizmoPoint = gridOriginPosition + lengthVector;
        float lengthHandleSize = HandleUtility.GetHandleSize(lengthGizmoPoint) * 0.5f;

        EditorGUI.BeginChangeCheck();
        Handles.color = Color.blue;
        Vector3 lengthHandlePos = Handles.Slider(lengthGizmoPoint, Vector3.forward, lengthHandleSize, Handles.ArrowHandleCap, _handleSnap);
        //lengthHandlePos = new Vector3(0, 0, Mathf.Round(lengthHandlePos.z / gridSpacing_fromShader) * gridSpacingUnitToSnapTo);
        lengthHandlePos = new Vector3(0, 0, Mathf.Round(lengthHandlePos.z / gridSpacing_fromShader) * gridSpacingUnitToSnapTo);
        //ensure handle does NOT collapse on the object origin
        lengthHandlePos = new Vector3(0, 0, Mathf.Max(lengthHandlePos.z, gridSpacing_fromShader/2) );

        if (EditorGUI.EndChangeCheck ()) {
			Undo.RecordObject(gridController, "Move Length Gizmo");
			EditorUtility.SetDirty(gridController);
            gridController.SetGridLength(lengthHandlePos.z);
		}

        //2nd gizmo handle
        Vector3 widthHandlePoint = gridOriginPosition + widthVector;
        float widthHandleSize = HandleUtility.GetHandleSize(widthHandlePoint) * 0.5f;

        EditorGUI.BeginChangeCheck();
        Handles.color = Color.red;
        Vector3 widthHandlePos = Handles.Slider(widthHandlePoint, Vector3.right, widthHandleSize, Handles.ArrowHandleCap, _handleSnap);
        widthHandlePos = new Vector3(Mathf.Round(widthHandlePos.x / gridSpacingUnitToSnapTo) * gridSpacingUnitToSnapTo, 0, 0);
        //ensure handle does NOT collapse on the object origin
        widthHandlePos = new Vector3(Mathf.Max(widthHandlePos.x, gridSpacing_fromShader/2), 0, 0);

        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(gridController, "Move Width Gizmo");
            EditorUtility.SetDirty(gridController);
            gridController.SetGridWidth(widthHandlePos.x);
        }

        //render handles for any grid jumps
        /*
        Handles.color = Color.white;

        for (int i = 0; i < path.arraySize; i++)
        {
            Vector3 point = path.GetArrayElementAtIndex(i).vector3Value;

            EditorGUI.BeginChangeCheck();
            Vector3 handlePos = Handles.DoPositionHandle(point, handleRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(moveToPoints, "Move Array Point");
                EditorUtility.SetDirty(moveToPoints);
                moveToPoints.SetPointElement(i, handlePos);
            }
        }
        */

    }


}
