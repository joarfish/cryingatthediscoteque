using UnityEngine;

public class PlayerControls : MonoBehaviour {
    public Rhythm rhythm;

    private FloorLayout _floorLayout;

    private Vector3 _nextPosition = Vector3.zero;
    private Field _nextField = Field.Zero;
    private readonly Vector3 _north = new Vector3(0.0f, 0.0f, 1.0f);
    private readonly Vector3 _south = new Vector3(0.0f, 0.0f, -1.0f);
    private readonly Vector3 _west = new Vector3(-1.0f, 0.0f, 0.0f);
    private readonly Vector3 _east = new Vector3(1.0f, 0.0f, 0.0f);

    void Start() {
        _floorLayout = FindObjectOfType<FloorLayout>();
    }

    void Update() {
        if (!rhythm.isMoveAllowed()) {
            return;
        }

        _nextPosition = transform.position;

        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            _nextPosition += _north;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            _nextPosition += _south;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            _nextPosition += _west;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)) {
            _nextPosition += _east;
        }

        _nextField.SetFromVector3(_nextPosition);

        if (_floorLayout.IsFieldAllowed(ref _nextField)) {
            transform.position = _nextPosition;
        }
    }
}