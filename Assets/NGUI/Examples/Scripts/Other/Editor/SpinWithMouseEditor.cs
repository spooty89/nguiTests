//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
#if UNITY_3_5
[CustomEditor(typeof(SpinWithMouse))]
#else
[CustomEditor(typeof(SpinWithMouse), true)]
#endif
public class SpinWithMouseEditor : Editor
{
	enum Highlight
	{
		DoNothing,
		Press,
	}
	
	public override void OnInspectorGUI ()
	{
		GUILayout.Space(6f);
		NGUIEditorTools.SetLabelWidth(86f);
		serializedObject.Update();

		SpinWithMouse swm = target as SpinWithMouse;
		SerializedProperty sp = NGUIEditorTools.DrawProperty("Target", serializedObject, "target", true);
		if(sp.objectReferenceValue != null)
			swm.target = (Transform)sp.objectReferenceValue;
		else
			swm.target = null;
		sp = NGUIEditorTools.DrawProperty("Speed", serializedObject, "speed", true);
		swm.speed = sp.floatValue;


		sp = NGUIEditorTools.DrawProperty("Momentum", serializedObject, "momentum", true);
		swm.momentum = sp.boolValue;
		if (swm.momentum)
		{
			if (NGUIEditorTools.DrawHeader("Momentum", "Momentum", false, true))
			{
				NGUIEditorTools.BeginContents(true);
				sp = NGUIEditorTools.DrawProperty("Smooth Drag Start", serializedObject, "smoothDragStart", true);
				swm.smoothDragStart = sp.boolValue;
				sp = NGUIEditorTools.DrawProperty("Momentum Amount", serializedObject, "momentumAmount", true);
				swm.momentumAmount = sp.floatValue;
				NGUIEditorTools.EndContents();
			}
		}
	}
}
