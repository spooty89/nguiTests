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
		NGUIEditorTools.DrawProperty("Target", serializedObject, "target", true);
		NGUIEditorTools.DrawProperty("Speed", serializedObject, "speed", true);
		
		SpinWithMouse swm = target as SpinWithMouse;

		SerializedProperty sp = NGUIEditorTools.DrawProperty("Momentum", serializedObject, "momentum", true);
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
