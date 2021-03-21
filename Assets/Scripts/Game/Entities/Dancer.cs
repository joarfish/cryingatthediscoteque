using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Game;
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

    private float _movementSpeed = 0.25f;
    private float _animationTimer = 0.0f;
    private Vector3 _targetPosition = Vector3.zero;
    private Vector3 _currentPosition = Vector3.zero;
    private Vector3 _direction = Vector3.zero;

    private void OnEnable() {
        _floorLayout = FindObjectOfType<FloorLayout>();
        _dancerCoordinator = FindObjectOfType<DancerCoordinator>();
        _dancerCoordinator.registerDancer(this);

        if (waypoints.Count == 0) {
            return;
        }
        
        
        
        _currentPosition = transform.position;
        _targetPosition = waypoints[_currentWaypoint];
    }

    private void OnDisable() {
        _dancerCoordinator.unregisterDancer(this);
    }

    private Field __field = Field.Zero;

    public void Move() {

        if (waypoints.Count == 0) {
            return;
        }
        
        _lastWaypoint = _currentWaypoint;
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
        _direction = _currentPosition - _targetPosition;
        _animationTimer = 0.0f;
        _moving = true;
    }

    private bool _moving = false;
    private int _lastWaypoint = -1;

    private void Update() {
        _animationTimer += Time.deltaTime;
        if (transform.position == _targetPosition && _moving) {
            _floorLayout.Free(__field.SetFromVector3(waypoints[_lastWaypoint]));
            _floorLayout.Occupy(__field.SetFromVector3(waypoints[_currentWaypoint]), this);
            _moving = false;
        }
        else if (_moving) {
            transform.position = Vector3.Lerp(_currentPosition, _targetPosition,
                MovementCurve.Evaluate(_animationTimer / _movementSpeed));
        }

    }

    public float GetDancerY() {
        return transform.position.y;
    }

    public Vector3 GetDirection() {
        return _direction;
    }
}