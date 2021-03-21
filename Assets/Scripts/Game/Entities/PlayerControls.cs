using System;
using Architecture;
using Game;
using Game.Systems;
using Helpers;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerControls : MonoBehaviour {
    public Rhythm rhythm;

    [FormerlySerializedAs("MovementCurve")]
    public AnimationCurve movementCurve;

    private FloorLayout _floorLayout;

    private Vector3 _nextPosition = Vector3.zero;
    private Field _nextField = Field.Zero;
    private readonly Vector3 _north = new Vector3(0.0f, 0.0f, 1.0f);
    private readonly Vector3 _south = new Vector3(0.0f, 0.0f, -1.0f);
    private readonly Vector3 _west = new Vector3(-1.0f, 0.0f, 0.0f);
    private readonly Vector3 _east = new Vector3(1.0f, 0.0f, 0.0f);

    private Vector3 _targetPosition = Vector3.zero;
    private Vector3 _currentPosition = Vector3.zero;
    private float _animationTimer = 0f;
    private const float MovementSpeed = 0.125f;

    private const int MaxLastPositions = 4;
    private RingStack<Vector3> _lastPositions;

    [Inject] private GameEventSystem _gameEventSystem;

    public PlayerControls() {
        SimpleDependencyInjection.getInstance().Inject(this);
    }

    private void OnEnable() {
        _floorLayout = FindObjectOfType<FloorLayout>();
    }

    void Start() {
        _targetPosition = _currentPosition = transform.position;
        _lastPositions = new RingStack<Vector3>(MaxLastPositions);
        _lastPositions.Push(_targetPosition);
    }

    void Update() {
        _animationTimer += Time.deltaTime;
        if (_currentPosition != _targetPosition) {
            transform.position = Vector3.Lerp(_currentPosition, _targetPosition,
                movementCurve.Evaluate(_animationTimer / MovementSpeed));
        }

        HandleInput();
    }

    public void Move(Field field) {
        _nextPosition = field.ToVector3(transform.position.y);
        _lastPositions.Push(_targetPosition);
        _currentPosition = _targetPosition;
        _targetPosition = _nextPosition;
        _animationTimer = 0.0f;
        _gameEventSystem.SendPlayerPositionChanged(_nextField);
    }
    
    public void BounceBack() {
        if (!_lastPositions.HasElements()) {
            return;
        }
        
        _animationTimer = 0.0f;
        _currentPosition = _targetPosition;
        _targetPosition = _lastPositions.Pop();
        _nextField.SetFromVector3(_targetPosition);
        _gameEventSystem.SendPlayerPositionChanged(_nextField);
    }

    public void BounceInDirection(Vector3 direction) {
        _animationTimer = 0.0f;
        direction.y = 0.0f;
        direction.Normalize();
        Move(_nextField.SubtractByVector3(direction * 2.0f));
    }
    
    public void HandleInput() {
        bool newInput = false;

        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            _nextPosition = _targetPosition + _north;
            newInput = true;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            _nextPosition = _targetPosition + _south;
            newInput = true;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            _nextPosition = _targetPosition + _west;
            newInput = true;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            _nextPosition = _targetPosition + _east;
            newInput = true;
        }

        if (newInput) {
            _gameEventSystem.SendPlayerInput(_nextField.SetFromVector3(_nextPosition));
        }
    }
}