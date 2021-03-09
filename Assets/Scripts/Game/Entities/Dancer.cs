using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

public class Dancer : MonoBehaviour {
    [SerializeField] private List<Vector3> waypoints;
    public bool loop = false;

    private FloorLayout _floorLayout;

    private DancerCoordinator _dancerCoordinator;
    public AnimationCurve MovementCurve;

    private int _currentWaypoint = 0;
    private bool _reverse = false;
    
    private float _movementSpeed = 0.125f;
    private float _animationTimer = 0.0f;
    private Vector3 _targetPosition = Vector3.zero;
    private Vector3 _currentPosition = Vector3.zero;

    private void OnEnable() {
        _floorLayout = FindObjectOfType<FloorLayout>();
        _dancerCoordinator = FindObjectOfType<DancerCoordinator>();
        _dancerCoordinator.registerDancer(this);
        _currentPosition = transform.position;
        _targetPosition = waypoints[_currentWaypoint];
    }

    private void OnDisable() {
        _dancerCoordinator.unregisterDancer(this);
    }

    private Field __field = Field.Zero;

    public void Move() {
        /*var currentPosition = transform.position;
        var nextPosition = currentPosition + transform.localToWorldMatrix.MultiplyVector(movement);
        if (!_floorLayout.IsFieldAllowed((int) nextPosition.x, (int) nextPosition.z))
        {
            movement *= -1;
        }*/
        var _lastWaypoint = _currentWaypoint;
        if (loop) {
            _currentWaypoint = (_currentWaypoint + 1) % waypoints.Count;
        }
        else {
            if (_currentWaypoint + 1 == waypoints.Count) {
                _reverse = true;
            }
            else if (_currentWaypoint == 0 && _reverse) {
                _reverse = false;
            }

            _currentWaypoint += (_reverse ? -1 : 1);
        }

        _currentPosition = _targetPosition;
        _targetPosition = waypoints[_currentWaypoint];
        _animationTimer = 0.0f;
        _floorLayout.Free(__field.SetFromVector3(waypoints[_lastWaypoint]));
        _floorLayout.Occupy(__field.SetFromVector3(waypoints[_currentWaypoint]));
    }

    private void Update() {
        _animationTimer += Time.deltaTime;
        transform.position = Vector3.Lerp(_currentPosition, _targetPosition, MovementCurve.Evaluate(_animationTimer / _movementSpeed));
    }

    public void AddWaypoint(Field field) {
        waypoints.Add(new Vector3((float) field.x, transform.position.y, (float) field.z));
    }

    public void RemoveWaypoint(Field field) {
        var idx = waypoints.FindIndex((waypoint) => (int) waypoint.x == field.x && (int) waypoint.z == field.z);
        waypoints.RemoveAt(idx);
    }

    public void UpdateWaypoint(int fieldIndex, Field field) {
        if (fieldIndex != -1 && fieldIndex < waypoints.Count) {
            waypoints[fieldIndex] = field.ToVector3(transform.position.y);
        }
    }

    public int FindWaypoint(Field field) {
        return waypoints.FindIndex((waypoint) => (int) waypoint.x == field.x && (int) waypoint.z == field.z);
    }

    public List<Vector3> GetWaypoints() {
        return waypoints;
    }

    public float GetDancerY() {
        return transform.position.y;
    }
}