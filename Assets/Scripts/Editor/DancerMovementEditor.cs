using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Dancer))]
public class DancerMovementEditor : Editor {
    private Dancer _dancer;
    private FloorLayout _floorLayout;
    private int _selectedWaypoint = -1;
    private SerializedProperty _waypointsProperty;

    private void OnEnable() {
        _dancer = (Dancer) target;
        _floorLayout = FindObjectOfType<FloorLayout>();
        if (!_floorLayout) {
            Debug.LogWarning("You need to set up a floor layout component!");
        }

        _waypointsProperty = serializedObject.FindProperty("waypoints");
    }

    void OnSceneGUI() {
        var guiEvent = Event.current;
        bool action = false;
        
        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.Shift) {
            action = true;
            var field = GetField(guiEvent.mousePosition);
            if (_floorLayout.IsFieldAllowed(ref field)) {
                Undo.RecordObject(_dancer, "Add Dancer Waypoint");
                //_dancer.AddWaypoint(field);
                _waypointsProperty.arraySize++;
                _waypointsProperty.GetArrayElementAtIndex(_waypointsProperty.arraySize-1).vector3Value =
                    field.ToVector3(0.0f);
                serializedObject.ApplyModifiedProperties();
            }
        }
        else if (guiEvent.type == EventType.MouseMove && guiEvent.button == 0) {
            var field = GetField(guiEvent.mousePosition);
            _selectedWaypoint = _dancer.FindWaypoint(field);
        }
        else if (guiEvent.type == EventType.MouseDrag && guiEvent.button == 0 && _selectedWaypoint != -1) {
            action = true;
            var field = GetField(guiEvent.mousePosition);
            if (_floorLayout.IsFieldAllowed(ref field)) {
                Undo.RecordObject(_dancer, "Move Dancer Waypoint");
                _dancer.UpdateWaypoint(_selectedWaypoint, field);
                _waypointsCache = null;
            }
        }
        else if (guiEvent.type == EventType.MouseUp) {
            _selectedWaypoint = -1;
        }

        if (guiEvent.type == EventType.Layout) {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }
        
        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && _selectedWaypoint == -1 && !action) {
            Selection.objects = null;
        }

        DrawPath();
    }

    private readonly string[] _excludedProperties = {"waypoints"};
    public override void OnInspectorGUI() {
        serializedObject.ApplyModifiedProperties();
        //DrawPropertiesExcluding(serializedObject, _excludedProperties);
        DrawDefaultInspector();
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

    private Vector3[] _waypointsCache = null;
    private readonly Vector3 _levelFloor = new Vector3(1.0f, 0.0f, 1.0f);

    private void DrawPath() {
        var currentWaypointsList = _dancer.GetWaypoints();
        if (_waypointsCache == null || _waypointsCache.Length != currentWaypointsList.Count) {
            _waypointsCache = currentWaypointsList.ToArray();

            for (var i = 0; i < _waypointsCache.Length; i++) {
                _waypointsCache[i].Scale(_levelFloor);
            }
        }

        if (_waypointsCache.Length == 0) {
            return;
        }

        if (_selectedWaypoint == 0) {
            Handles.color = Color.blue;
        }

        Handles.DrawSolidDisc(_waypointsCache[0], Vector3.up, 0.125f);
        Handles.color = Color.white;

        for (var i = 1; i < _waypointsCache.Length; i++) {
            Handles.DrawDottedLine(_waypointsCache[i - 1], _waypointsCache[i], 5);
            if (i == _selectedWaypoint) {
                Handles.color = Color.blue;
            }

            Handles.DrawSolidDisc(_waypointsCache[i], Vector3.up, 0.125f);
            Handles.color = Color.white;
        }

        HandleUtility.Repaint();
    }
}