using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum FieldStatus {
    Free,
    Occupied,
    Invalid,
    Player,
}

[Serializable]
public struct Field {
    public Field(int x, int z) {
        this.x = x;
        this.z = z;
    }
    
    public Field(Vector3 vector) {
        x = (int)vector.x;
        z = (int)vector.z;
    }

    public static Field Zero => Field.ZeroField;
    private static readonly Field ZeroField = new Field(0, 0);

    public Vector3 ToVector3(float y = 0.0f) {
        return new Vector3((float) this.x, y, (float) this.z);
    }

    public Field SetFromVector3(Vector3 vector) {
        x = (int) vector.x;
        z = (int) vector.z;
        return this;
    }

    public int x;
    public int z;
}

public class FloorLayout : MonoBehaviour {
    [Range(1, 20)] public int tilesX = 1;
    [Range(1, 20)] public int tilesZ = 1;
    public List<Field> invalidFields;

    private FieldStatus[,] _layout;

    private void OnEnable() {
        _layout = new FieldStatus[tilesX, tilesZ];
        for (var i = 0; i < _layout.GetLength(0); i++) {
            for (var j = 0; j < _layout.GetLength(1); j++) {
                _layout[i, j] = FieldStatus.Free;
            }
        }
        foreach (var invalidField in invalidFields) {
            _layout[invalidField.x, invalidField.z] = FieldStatus.Invalid;
        }
    }

#if (UNITY_EDITOR)
    public bool alwaysShowGrid = false;
    
    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        if (alwaysShowGrid) {
            DrawGrid();
        }
        else {
            Gizmos.DrawLine(new Vector3(-0.5f, 0.0f, -0.5f), new Vector3(-0.5f, 0.0f, tilesZ - 0.5f));
            Gizmos.DrawLine(new Vector3(tilesX - 0.5f, 0.0f, -0.5f), new Vector3(tilesX - 0.5f, 0.0f, tilesZ - 0.5f));
            Gizmos.DrawLine(new Vector3(-0.5f, 0.0f, -0.5f), new Vector3(tilesX - 0.5f, 0.0f, -0.5f));
            Gizmos.DrawLine(new Vector3(-0.5f, 0.0f, tilesZ - 0.5f), new Vector3(tilesX - 0.5f, 0.0f, tilesZ - 0.5f));
        }
    }

    private void OnDrawGizmosSelected() {
        DrawGrid();
    }

    private void DrawGrid() {
        Gizmos.color = Color.blue;

        for (var x = 0; x <= tilesX; x++) {
            Gizmos.DrawLine(new Vector3(x - 0.5f, 0.0f, -0.5f), new Vector3(x - 0.5f, 0.0f, (float) tilesZ - 0.5f));
        }

        for (var z = 0; z <= tilesZ; z++) {
            Gizmos.DrawLine(new Vector3(-0.5f, 0.0f, z - 0.5f), new Vector3(tilesX - 0.5f, 0.0f, z - 0.5f));
        }

        foreach (var invalidField in invalidFields) {
            DrawFieldDisabled(invalidField.x, invalidField.z);
        }
    }

    private void DrawFieldDisabled(int x, int z) {
        Gizmos.DrawLine(new Vector3(x - 0.5f, 0.0f, z + 0.5f), new Vector3(x + 0.5f, 0.0f, z - 0.5f));
        Gizmos.DrawLine(new Vector3(x - 0.5f, 0.0f, z - 0.5f), new Vector3(x + 0.5f, 0.0f, z + 0.5f));
    }

#endif

    public bool IsFieldAllowed(ref Field field) {
        return 0 <= field.x && field.x < tilesX && 0 <= field.z && field.z < tilesZ;
    }

    public FieldStatus GetFieldStatus(ref Field field) {
        return _layout[field.x, field.z];
    }

    public void Occupy(Field field) {
        if (_layout[field.x, field.z] == FieldStatus.Player) {
            _playerControls.BounceBack();
        }
        _layout[field.x, field.z] = FieldStatus.Occupied;
    }

    private PlayerControls _playerControls = null;
    private Field _playerField = Field.Zero;
    public void SetPlayerController(PlayerControls playerControls) {
        _playerControls = playerControls;
    }

    public void SetPlayer(Field field) {
        _layout[_playerField.x, _playerField.z] = FieldStatus.Free;
        _layout[field.x, field.z] = FieldStatus.Player;
        _playerField = field;
        //Debug.Log("Player Field: " + _playerField.x + " " + _playerField.z);
    }

    public void Free(Field field) {
        _layout[field.x, field.z] = FieldStatus.Free;
    }
}