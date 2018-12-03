using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CurvedLineRenderer))]
public class CurvedLineRendererEditor : Editor {

	public SerializedProperty 
	lineMaterial,
	lineSegmentSize,
	lineWidth,
	showGizmos,
	gizmoSize,
	gizmoColor,
	linePoints;

	//gui elements
	private static GUIContent 
	addButton = new GUIContent("+", "add"),
	deleteButton = new GUIContent("-", "delete"),
	addToListButton = new GUIContent("Add", "add element");

	//gui element options
	private static GUILayoutOption listButtonWidth = GUILayout.Width(20f);

	public void OnEnable() {
		lineMaterial = serializedObject.FindProperty ("lineMaterial");
		lineSegmentSize = serializedObject.FindProperty ("lineSegmentSize");
		lineWidth = serializedObject.FindProperty ("lineWidth");
		showGizmos = serializedObject.FindProperty ("showGizmos");
		gizmoSize = serializedObject.FindProperty ("gizmoSize");
		gizmoColor = serializedObject.FindProperty ("gizmoColor");
		linePoints = serializedObject.FindProperty ("linePoints");

	}

	public override void OnInspectorGUI() {
		serializedObject.Update ();

		EditorGUILayout.PropertyField (showGizmos);
		bool showGizmoValue = showGizmos.boolValue;
		EditorGUI.BeginDisabledGroup (!showGizmoValue);
		EditorGUI.indentLevel++;
		EditorGUILayout.PropertyField (gizmoColor);
		EditorGUILayout.PropertyField (gizmoSize);
		EditorGUI.indentLevel--;
		EditorGUI.EndDisabledGroup ();

		EditorGUILayout.PropertyField (lineMaterial);
		EditorGUILayout.PropertyField (lineSegmentSize);
		EditorGUILayout.PropertyField (lineWidth);

		EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
		EditorGUILayout.LabelField("Line Points", GUI.skin.textField);
		RenderListElementsWithButtons (linePoints);

		serializedObject.ApplyModifiedProperties();
	}

	private void OnSceneGUI () {
		CurvedLineRenderer curvedLineRenderer = target as CurvedLineRenderer;
		Transform handleTransform = curvedLineRenderer.transform;
		Quaternion handleRotation = handleTransform.rotation;

		Handles.color = Color.white;

		for(int i=0;i<linePoints.arraySize;i++) {
			//convert the local space points to global space
			Vector3 point = handleTransform.TransformPoint(linePoints.GetArrayElementAtIndex(i).vector3Value);

			EditorGUI.BeginChangeCheck();
			Vector3 handlePos = Handles.DoPositionHandle(point, handleRotation);
			if (EditorGUI.EndChangeCheck ()) {
				Undo.RecordObject(curvedLineRenderer, "Move Array Point");
				EditorUtility.SetDirty(curvedLineRenderer);
				//convert the global space point from the handle to locaal space
				curvedLineRenderer.SetPointElement(i,handleTransform.InverseTransformPoint(handlePos) );
				curvedLineRenderer.UpdateLineRenderer();
			}
		}
	}

	private static void AddListAddDeleteButtons(SerializedProperty list, int index) {
		if (!list.isArray) {
			EditorGUILayout.HelpBox(list.name + " is neither an array nor a list!", MessageType.Error);
			return;
		}

		if (GUILayout.Button (addButton, EditorStyles.miniButtonLeft, listButtonWidth)) {
			list.InsertArrayElementAtIndex(index);

		}

		if (GUILayout.Button (deleteButton, EditorStyles.miniButtonLeft, listButtonWidth)) {
			//check if the element was removed (unity can sometimes be helpful and "clear" the element's values instead of deleting)
			int oldSize = list.arraySize;
			list.DeleteArrayElementAtIndex(index);
			if (list.arraySize == oldSize) {
				//if the element was cleared, delete it again to actually remove it
				list.DeleteArrayElementAtIndex(index);
			}
		}
	}

	private static void RenderListElementsWithButtons (SerializedProperty list) {
		if (!list.isArray) {
			EditorGUILayout.HelpBox(list.name + " is neither an array nor a list!", MessageType.Error);
			return;
		}

		for (int i = 0; i < list.arraySize; i++) {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
			AddListAddDeleteButtons(list, i);
			EditorGUILayout.EndHorizontal();
		}

		if (list.arraySize == 0 && GUILayout.Button(addToListButton, EditorStyles.miniButton)) {
			list.arraySize++;
		}
	}
}
