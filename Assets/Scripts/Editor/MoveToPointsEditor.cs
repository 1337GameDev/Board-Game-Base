using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MoveToPoints))]
public class MoveToPointsEditor : Editor {

	public SerializedProperty 
	drawGizmo,
    gizmoPathColor,
	gizmoPathPointColor,
	moveSpeed,
	retracePath,
	loopPath,
	lookAtNextPoint,
	path;

	//gui elements
	private static GUIContent 
		addButton = new GUIContent("+", "add"),
		deleteButton = new GUIContent("-", "delete"),
		addToListButton = new GUIContent("Add", "add element");

	//gui element options
	private static GUILayoutOption listButtonWidth = GUILayout.Width(20f);

	public void OnEnable() {
		drawGizmo = serializedObject.FindProperty ("drawGizmo");
        gizmoPathColor = serializedObject.FindProperty ("gizmoPathColor");
		gizmoPathPointColor = serializedObject.FindProperty ("gizmoPointColor");
		moveSpeed = serializedObject.FindProperty ("moveSpeed");
		retracePath = serializedObject.FindProperty ("retracePath");
		loopPath = serializedObject.FindProperty ("loopPath");
		lookAtNextPoint = serializedObject.FindProperty ("lookAtNextPoint");
		path = serializedObject.FindProperty ("path");

	}

	public override void OnInspectorGUI() {
		serializedObject.Update ();

		EditorGUILayout.PropertyField (drawGizmo);
		bool drawGizmoValue = drawGizmo.boolValue;
		EditorGUI.BeginDisabledGroup (!drawGizmoValue);
		EditorGUI.indentLevel++;
		EditorGUILayout.PropertyField (gizmoPathColor);
		EditorGUILayout.PropertyField (gizmoPathPointColor);
		EditorGUI.indentLevel--;
		EditorGUI.EndDisabledGroup ();

		EditorGUILayout.PropertyField (moveSpeed);
		EditorGUILayout.PropertyField (retracePath);
		EditorGUILayout.PropertyField (loopPath);
		EditorGUILayout.PropertyField (lookAtNextPoint);

		EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
		EditorGUILayout.LabelField("Path Points", GUI.skin.textField);
		RenderListElementsWithButtons (path);

		serializedObject.ApplyModifiedProperties();
	}

	private void OnSceneGUI () {
		MoveToPoints moveToPoints = target as MoveToPoints;
		Transform handleTransform = moveToPoints.transform;
		Quaternion handleRotation = handleTransform.rotation;

		Handles.color = Color.white;

		for(int i=0;i<path.arraySize;i++) {
			Vector3 point = path.GetArrayElementAtIndex (i).vector3Value;

			EditorGUI.BeginChangeCheck();
			Vector3 handlePos = Handles.DoPositionHandle(point, handleRotation);
			if (EditorGUI.EndChangeCheck ()) {
				Undo.RecordObject(moveToPoints, "Move Array Point");
				EditorUtility.SetDirty(moveToPoints);
				moveToPoints.SetPointElement(i,handlePos);
			}
		}
	}

	private static void AddListAddDeleteButtons (SerializedProperty list, int index) {
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
