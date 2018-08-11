using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(LimitedRange))]
public class LimitedRangeDrawer : PropertyDrawer {

	public float floatFieldWidth = 50f;
	public float labelWidth = 110f;
	public float labelCharacterWidth = 10f;
	public float fieldHeight = 16;
	// public float sliderWidth = 0.5f;
	public float horizontalPadding = 2f;

	public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label) {
		
		SerializedProperty minimumBound = property.FindPropertyRelative("minimumBound");
		SerializedProperty maximumBound = property.FindPropertyRelative("maximumBound");
		SerializedProperty lowerBound = property.FindPropertyRelative("_lowerBound");
		SerializedProperty upperBound = property.FindPropertyRelative("_upperBound");

		EditorGUI.BeginProperty(pos, label, property);

		//draw label
		float x = pos.x;
		string name = property.name;
		EditorGUI.LabelField(new Rect(x, pos.y, name.Length * labelCharacterWidth, fieldHeight), label);
		x += labelWidth + horizontalPadding;

		//draw minimum bound field
		minimumBound.floatValue = EditorGUI.FloatField(new Rect(x, pos.y, floatFieldWidth, fieldHeight), minimumBound.floatValue);
		x += floatFieldWidth + horizontalPadding;

		//draw upper/lower bound range slider
		float lower = lowerBound.floatValue;
		float upper = upperBound.floatValue;
		float sliderWidth = pos.width - ((floatFieldWidth + horizontalPadding) * 2) - labelWidth - horizontalPadding; //slider width is flexible
		EditorGUI.MinMaxSlider(new Rect(x, pos.y, sliderWidth, fieldHeight), ref lower, ref upper, minimumBound.floatValue, maximumBound.floatValue);
		lowerBound.floatValue = lower;
		upperBound.floatValue = upper;
		x += sliderWidth + horizontalPadding;

		//draw maximum bound field
		maximumBound.floatValue = EditorGUI.FloatField(new Rect(x, pos.y, floatFieldWidth, fieldHeight), maximumBound.floatValue);

		EditorGUI.EndProperty();
	}
}
