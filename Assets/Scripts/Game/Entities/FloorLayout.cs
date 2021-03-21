using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Architecture;
using Game;
using Game.Systems;
using UnityEditor;
using UnityEngine;

public enum FieldStatus {
    Free,
    Occupied,
    Invalid,
    Hole,
    Player,
}

public class FloorLayout : MonoBehaviour {
    [Range(1, 20)] public int tilesX = 1;
    [Range(1, 20)] public int tilesZ = 1;
    public List<Field> invalidFields;
    public List<Field> holeFields;
    public Field goalField;

    private FieldStatus[,] _layout;
    [Inject] private GameEventSystem _gameEventSystem;
    private Field _playerField = Field.Zero;

    public FloorLayout() {
        SimpleDependencyInjection.getInstance().Inject(this);
    }

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
        
        _gameEventSystem.OnPlayerPositionChanged += HandlePlayerPositionChanged;
    }

    public void OnDestroy() {
        _gameEventSystem.OnPlayerPositionChanged -= HandlePlayerPositionChanged;
    }

    private void HandlePlayerPositionChanged(Field newField) {
        _layout[_playerField.x, _playerField.z] = FieldStatus.Free;
        _layout[newField.x, newField.z] = FieldStatus.Player;
        _playerField = newField;
        if (_playerField.x == goalField.x && _playerField.z == goalField.z) {
            _gameEventSystem.SendPlayerMovedToGoal();
        }
    }
    
    public bool IsFieldAllowed(Field field) {
        return 0 <= field.x && field.x < tilesX && 0 <= field.z && field.z < tilesZ;
    }

    public FieldStatus GetFieldStatus(Field field) {
        return _layout[field.x, field.z];
    }

    public void Occupy(Field field, Dancer dancer) {
        if (_layout[field.x, field.z] == FieldStatus.Player) {
            _gameEventSystem.SendDancerHitPlayer(dancer.GetDirection());
        }

        _layout[field.x, field.z] = FieldStatus.Occupied;
    }

    public Field GetPlayerField() {
        return _playerField;
    }

    public void Free(Field field) {
        _layout[field.x, field.z] = FieldStatus.Free;
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

        if (invalidFields == null) {
            return;
        }

        foreach (var invalidField in invalidFields) {
            DrawFieldDisabled(invalidField.x, invalidField.z);
        }

        foreach (var holeField in holeFields) {
            DrawFieldHole(holeField.x, holeField.z);
        }
    }

    private void DrawFieldDisabled(int x, int z) {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(x - 0.5f, 0.0f, z + 0.5f), new Vector3(x + 0.5f, 0.0f, z - 0.5f));
        Gizmos.DrawLine(new Vector3(x - 0.5f, 0.0f, z - 0.5f), new Vector3(x + 0.5f, 0.0f, z + 0.5f));
    }

    private void DrawFieldHole(int x, int z) {
        Handles.color = Color.blue;
        Handles.DrawWireDisc(new Vector3(x, 0.0f, z), Vector3.up, 0.4f);
    }

#endif
}