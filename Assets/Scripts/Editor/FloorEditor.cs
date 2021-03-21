using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using Game;

[CustomEditor(typeof(FloorLayout))]
public class FloorEditor : Editor {
    private FloorLayout _floorLayout;
    private SerializedProperty _invalidFieldsProperty;
    private SerializedProperty _holeFieldsProperty;

    private void OnEnable() {
        _floorLayout = target as FloorLayout;
        _invalidFieldsProperty = serializedObject.FindProperty("invalidFields");
        _holeFieldsProperty = serializedObject.FindProperty("holeFields");
    }

    void OnSceneGUI() {
        var guiEvent = Event.current;
        bool action = false;

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 &&
            guiEvent.modifiers == EventModifiers.Shift) {
            var field = GetField(guiEvent.mousePosition);

            var possibleIndex = FindInvalidField(field);

            if (possibleIndex == -1) {
                var fieldElement = _invalidFieldsProperty.GetArrayElementAtIndex(_invalidFieldsProperty.arraySize++);
                fieldElement.FindPropertyRelative("x").intValue = field.x;
                fieldElement.FindPropertyRelative("z").intValue = field.z;
            }
            else {
                _invalidFieldsProperty.DeleteArrayElementAtIndex(possibleIndex);
            }

            action = true;
            serializedObject.ApplyModifiedProperties();
        }
        
        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 &&
            guiEvent.modifiers == EventModifiers.Control) {
            var field = GetField(guiEvent.mousePosition);

            var possibleIndex = FindHoleField(field);

            if (possibleIndex == -1) {
                var fieldElement = _holeFieldsProperty.GetArrayElementAtIndex(_holeFieldsProperty.arraySize++);
                fieldElement.FindPropertyRelative("x").intValue = field.x;
                fieldElement.FindPropertyRelative("z").intValue = field.z;
            }
            else {
                _holeFieldsProperty.DeleteArrayElementAtIndex(possibleIndex);
            }

            action = true;
            serializedObject.ApplyModifiedProperties();
        }

        if (guiEvent.type == EventType.Layout) {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && !action) {
            Selection.objects = null;
        }
    }

    private int FindInvalidField(Field field) {
        for (var i = 0; i < _invalidFieldsProperty.arraySize; i++) {
            var fieldElement = _invalidFieldsProperty.GetArrayElementAtIndex(i);
            if (field.x == fieldElement.FindPropertyRelative("x").intValue &&
                field.z == fieldElement.FindPropertyRelative("z").intValue) {
                return i;
            }
        }

        return -1;
    }

    private int FindHoleField(Field field) {
        for (var i = 0; i < _holeFieldsProperty.arraySize; i++) {
            var fieldElement = _holeFieldsProperty.GetArrayElementAtIndex(i);
            if (field.x == fieldElement.FindPropertyRelative("x").intValue &&
                field.z == fieldElement.FindPropertyRelative("z").intValue) {
                return i;
            }
        }

        return -1;
    }

    private static Field GetField(Vector2 mousePosition) {
        var worldPosition = GetFieldAsVector3(mousePosition);

        return new Field((int) worldPosition.x, (int) worldPosition.z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Vector3 GetFieldAsVector3(Vector2 mousePosition, float y = 0.0f) {
        var mouseRay = HandleUtility.GUIPointToWorldRay(mousePosition);
        var distance = (-mouseRay.origin.y / mouseRay.direction.y);

        var worldPosition = mouseRay.GetPoint(distance);
        worldPosition.Set((float) Math.Round(worldPosition.x), y, (float) Math.Round(worldPosition.z));

        return worldPosition;
    }
}