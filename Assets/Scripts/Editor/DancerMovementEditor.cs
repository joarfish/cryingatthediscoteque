using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using Game;

[CustomEditor(typeof(Dancer))]
public class DancerMovementEditor : Editor {
    private Dancer _dancer;
    private FloorLayout _floorLayout;
    private int _selectedWaypoint = -1;
    private SerializedProperty _waypointsProperty;
    private SerializedObject _transformObject;
    private SerializedProperty _serializedTransformPosition;

    private void OnEnable() {
        _dancer = (Dancer) target;
        _floorLayout = FindObjectOfType<FloorLayout>();
        if (!_floorLayout) {
            Debug.LogWarning("You need to set up a floor layout component!");
        }

        _waypointsProperty = serializedObject.FindProperty("waypoints");
        _transformObject = new SerializedObject(_dancer.transform);
        _serializedTransformPosition = _transformObject.FindProperty("m_LocalPosition");
        CheckFirstWaypointIsTransformPosition();
    }

    private void CheckFirstWaypointIsTransformPosition() {
        if (_waypointsProperty.arraySize == 0) {
            _waypointsProperty.arraySize++;
        }

        _waypointsProperty.GetArrayElementAtIndex(0).vector3Value = _dancer.transform.position;
        serializedObject.ApplyModifiedProperties();
    }

    void OnSceneGUI() {
        var guiEvent = Event.current;
        bool action = false;

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 &&
            guiEvent.modifiers == EventModifiers.Shift) {
            action = true;
            var field = GetField(guiEvent.mousePosition);
            if (_floorLayout.IsFieldAllowed(field)) {
                Undo.RecordObject(_dancer, "Add Dancer Waypoint");
                _waypointsProperty.arraySize++;
                _waypointsProperty.GetArrayElementAtIndex(_waypointsProperty.arraySize - 1).vector3Value =
                    field.ToVector3(_dancer.GetDancerY());
                serializedObject.ApplyModifiedProperties();
            }
        }
        else if (guiEvent.type == EventType.MouseMove && guiEvent.button == 0) {
            var field = GetField(guiEvent.mousePosition);
            _selectedWaypoint = -1;
            for (var i = 1; i < _waypointsProperty.arraySize; i++) {
                var waypointPos = _waypointsProperty.GetArrayElementAtIndex(i).vector3Value;
                if ((int) waypointPos.x == field.x && (int) waypointPos.z == field.z) {
                    _selectedWaypoint = i;
                    break;
                }
            }
        }
        else if (guiEvent.type == EventType.MouseDrag && guiEvent.button == 0 && _selectedWaypoint != -1) {
            action = true;
            var field = GetField(guiEvent.mousePosition);
            if (_floorLayout.IsFieldAllowed(field)) {
                Undo.RecordObject(_dancer, "Move Dancer Waypoint");
                _waypointsProperty.GetArrayElementAtIndex(_selectedWaypoint).vector3Value =
                    field.ToVector3(_dancer.GetDancerY());
                serializedObject.ApplyModifiedProperties();
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
    
    private Vector3 _lastPosition = Vector3.zero;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        _transformObject.Update();
        var position = _serializedTransformPosition.vector3Value;
        if (position != _lastPosition) {
            CheckFirstWaypointIsTransformPosition();
            SceneView.RepaintAll();
            _lastPosition = _serializedTransformPosition.vector3Value;
        }
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

    private readonly Vector3 _levelFloor = new Vector3(1.0f, 0.0f, 1.0f);

    private void DrawPath() {
        if (_selectedWaypoint == 0) {
            Handles.color = Color.blue;
        }

        var firstWaypoint = _waypointsProperty.GetArrayElementAtIndex(0).vector3Value;
        firstWaypoint.Scale(_levelFloor);
        Handles.DrawSolidDisc(firstWaypoint, Vector3.up, 0.125f);
        Handles.color = Color.white;
        
        for (var i = 1; i < _waypointsProperty.arraySize; i++) {
            var waypoint1 = _waypointsProperty.GetArrayElementAtIndex(i-1).vector3Value;
            var waypoint2 = _waypointsProperty.GetArrayElementAtIndex(i).vector3Value;
            waypoint1.Scale(_levelFloor);
            waypoint2.Scale(_levelFloor);
            if (i == _selectedWaypoint) {
                Handles.color = Color.blue;
            }
            Handles.DrawDottedLine(waypoint1, waypoint2, 5);
            Handles.DrawSolidDisc(waypoint2, Vector3.up, 0.125f);
            Handles.color = Color.white;
        }
        HandleUtility.Repaint();
    }
}