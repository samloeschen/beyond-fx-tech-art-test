using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(BossParameters))]
public class BossParametersDrawer : PropertyDrawer {

	float textFieldWidthOffset = 18;
	float textFieldHeight = 16;
	float descriptionTextAreaHeight = 58;
	float verticalPadding = 2;
	float propertyHeight = 0f;

    public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label) {

		SerializedProperty name = property.FindPropertyRelative("name");
		SerializedProperty subtitle = property.FindPropertyRelative("subtitle");
		SerializedProperty strongType = property.FindPropertyRelative("strongType");
		SerializedProperty weakType = property.FindPropertyRelative("weakType");
		SerializedProperty healthPoints = property.FindPropertyRelative("healthPoints");
		SerializedProperty moveSpeed = property.FindPropertyRelative("moveSpeed");
		SerializedProperty attackDelayTime = property.FindPropertyRelative("attackDelayTime");
		SerializedProperty moveDelayTime = property.FindPropertyRelative("moveDelayTime");
		SerializedProperty description = property.FindPropertyRelative("description");
        
		EditorGUI.BeginProperty(pos, label, property);

		propertyHeight = textFieldHeight;
		int indent = EditorGUI.indentLevel;
		
		property.isExpanded = EditorGUI.Foldout(new Rect(pos.x, pos.y, pos.width, textFieldHeight), property.isExpanded, label);

		if (property.isExpanded) {
			
			EditorGUI.indentLevel = 1;

			float y = pos.y + textFieldHeight;
			float width = pos.width - textFieldWidthOffset;

			DrawPropertyField(name, 			pos,	width,	textFieldHeight, 			ref y);
			DrawPropertyField(subtitle, 		pos, 	width, 	textFieldHeight,			ref y);
			DrawPropertyField(strongType, 		pos, 	width, 	textFieldHeight, 			ref y);
			DrawPropertyField(weakType, 		pos, 	width, 	textFieldHeight, 			ref y);
			DrawPropertyField(healthPoints, 	pos, 	width, 	textFieldHeight, 			ref y);
			DrawPropertyField(moveSpeed,	 	pos, 	width, 	textFieldHeight, 			ref y);
			DrawPropertyField(attackDelayTime, 	pos, 	width, 	textFieldHeight, 			ref y);
			DrawPropertyField(moveDelayTime, 	pos, 	width, 	textFieldHeight, 			ref y);
			DrawPropertyField(description, 		pos, 	width, 	descriptionTextAreaHeight, 	ref y);

			propertyHeight = y - pos.y;
			EditorGUI.indentLevel = indent;
		}

        EditorGUI.EndProperty();
		
    }

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
		return propertyHeight;
	}

	void DrawPropertyField (SerializedProperty prop, Rect rootPos, float width, float height, ref float currentY) {
		EditorGUI.PropertyField(new Rect(rootPos.x, currentY, width, height), prop);
		currentY += height + verticalPadding;
	}
}