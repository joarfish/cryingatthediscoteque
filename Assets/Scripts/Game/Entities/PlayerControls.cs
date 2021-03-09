using System;
using UnityEngine;

public class PlayerControls : MonoBehaviour {
    public Rhythm rhythm;
    public AnimationCurve MovementCurve;

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
    private readonly float _movementSpeed = 0.125f;

    private Camera _mainCamera;

    private static readonly int _maxLastPositions = 3;
    private RingStack<Vector3> _lastPositions;

    private void OnEnable() {
        _floorLayout = FindObjectOfType<FloorLayout>();
        _floorLayout.SetPlayerController(this);
    }

    void Start() {
        _targetPosition = _currentPosition = transform.position;
        _lastPositions = new RingStack<Vector3>(_maxLastPositions);
        _lastPositions.push(_targetPosition);
        _mainCamera = Camera.main;
    }

    void Update() {
        
        _animationTimer += Time.deltaTime;
        if (_currentPosition != _targetPosition) {
            transform.position = Vector3.Lerp(_currentPosition, _targetPosition, MovementCurve.Evaluate(_animationTimer / _movementSpeed));
        }
        
        if (!rhythm.isMoveAllowed()) {
            return;
        }

        bool newInput = false;

        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            _nextPosition = _targetPosition +_north;
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
            _nextField.SetFromVector3(_nextPosition);

            if (_floorLayout.IsFieldAllowed(ref _nextField) && _floorLayout.GetFieldStatus(ref _nextField) == FieldStatus.Free) {
                _lastPositions.push(_targetPosition);
                _currentPosition = _targetPosition;
                _targetPosition = _nextPosition;
                _animationTimer = 0.0f;
                _mainCamera.backgroundColor = Color.green;
                _floorLayout.SetPlayer(_nextField);
            } else if (_floorLayout.GetFieldStatus(ref _nextField) == FieldStatus.Occupied) {
                _mainCamera.backgroundColor = Color.red;                    
            }
        }
    }

    public void BounceBack() {
        if (_lastPositions.hasElements()) {
            _animationTimer = 0.0f;
            _currentPosition = _targetPosition;
            _targetPosition = _lastPositions.pop();
            _nextField.SetFromVector3(_targetPosition);
            _floorLayout.SetPlayer(_nextField);
        }
        else {
            Debug.Log("No more positions to bounce back to!");
        }
    }
}

struct RingStack<T> {
    private T[] _elements;
    private int _lastIndex;
    private int _firstIndex;
    private int _capacity;
    private int _elementCount;

    public RingStack(int capacity) {
        _elements = new T[capacity];
        _lastIndex = 0;
        _firstIndex = 0;
        _capacity = capacity;
        _elementCount = 0;
    }

    public void push(T element) {
        _elements[_lastIndex] = element;
        _lastIndex = (_lastIndex + 1) % _capacity;
        if (_elementCount == _capacity) {
            _firstIndex = (_firstIndex + 1) % _capacity;
        }
        
        if (_firstIndex == _lastIndex) {
            _firstIndex = (_firstIndex + 1) % _capacity;
        }

        _elementCount = (_elementCount == _capacity) ? _elementCount : _elementCount + 1;
    }

    public T pop() {
        if (_lastIndex == _firstIndex) {
            throw new Exception("Cannot call pop on an empty array!");
        }

        _lastIndex--;
        _elementCount--;
        
        if (_lastIndex == -1) {
            _lastIndex = _capacity - 1;
        }

        if (_lastIndex == _firstIndex && _elementCount > 0) {
            _firstIndex--;
            if (_firstIndex == -1) {
                _firstIndex = _capacity - 1;
            }
        }

        return _elements[_lastIndex];
    }

    public bool hasElements() {
        return _elementCount != 0;
    }

    public override string ToString() {
        var r = "_lastIndex=" + _lastIndex + " _firstIndex=" + _firstIndex + " _elementCount="+_elementCount + "\n";
        
        for (var i = 0; i < _elements.Length; i++) {
            r += i + ": " + _elements[i] + "\n";
        }

        return r;
    }
}