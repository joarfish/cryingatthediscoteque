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

    void Start() {
        _floorLayout = FindObjectOfType<FloorLayout>();
        _targetPosition = _currentPosition = transform.position;
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

            if (_floorLayout.IsFieldAllowed(ref _nextField)) {
                _currentPosition = _targetPosition;
                _targetPosition = _nextPosition;
                _animationTimer = 0.0f;
            }
        }
    }
}