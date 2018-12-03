using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SnapToGrid))]
public class SnapToGridEditor : Editor {

    public SerializedProperty
    grid,
    constrainToGrid;

	public void OnEnable() {
        grid = serializedObject.FindProperty("grid");
        constrainToGrid = serializedObject.FindProperty("constrainToGrid");
	}

	public override void OnInspectorGUI() {
		serializedObject.Update ();
        EditorGUILayout.PropertyField(grid);
        EditorGUILayout.PropertyField(constrainToGrid);
        serializedObject.ApplyModifiedProperties();
	}

	private void OnSceneGUI () {
        Tools.current = Tool.None;

        SnapToGrid snapToGrid = target as SnapToGrid;
        Transform handleTransform = snapToGrid.transform;
        Vector3 handlePosition = handleTransform.position;
        Quaternion handleRotation = handleTransform.rotation;

        Handles.color = Color.white;

        if ((grid!=null) && (grid.objectReferenceValue != null)) {
            GridController gridController = (GridController)grid.objectReferenceValue;
            //get grid spacing value of shader
            gridController.UpdateCachedReferences();
            gridController.UpdateCachedValues();
            float gridSpacing_fromShader = gridController.GridSpacing();
            //float gridSpacingUnitToSnapTo = gridSpacing_fromShader / 2.0f;

            //replace transform move tool so it snaps to grid
            EditorGUI.BeginChangeCheck();
            Vector3 newTransPos = Handles.DoPositionHandle(handlePosition, handleRotation);
            newTransPos = gridController.GetGridSquareNearest(newTransPos);
            newTransPos = new Vector3(newTransPos.x, handlePosition.y, newTransPos.z);

            bool constrain = constrainToGrid.boolValue;

            if (EditorGUI.EndChangeCheck()) {
                //if we dont care about constraining, ORR the point is on the grid (it would have short circuted if we should constrain)
                if (!constrain || gridController.IsPointOnGrid(newTransPos))
                {
                    Undo.RecordObject(handleTransform, "Move Snap To Grid Transform");
                    EditorUtility.SetDirty(handleTransform);
                    handleTransform.position = newTransPos;
                }
            }
        }
    }

}
