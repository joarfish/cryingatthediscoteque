using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum FieldStatus {
    Free,
    Occupied,
    Invalid
}

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

    public void SetFromVector3(Vector3 vector) {
        x = (int) vector.x;
        z = (int) vector.z;
    }

    public int x;
    public int z;
}

public class FloorLayout : MonoBehaviour {
    [Range(1, 20)] public int tilesX = 1;
    [Range(1, 20)] public int tilesZ = 1;

#if (UNITY_EDITOR)
    public bool AlwaysShowGrid = false;
#endif

    private FieldStatus[,] _layout;

    private void OnEnable() {
        _layout = new FieldStatus[tilesX, tilesZ];
        for (var i = 0; i < _layout.GetLength(0); i++) {
            for (var j = 0; j < _layout.GetLength(1); j++) {
                _layout[i, j] = FieldStatus.Free;
            }
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        if (AlwaysShowGrid) {
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

        DrawFieldDisabled(0, 1);
    }

    private void DrawFieldDisabled(int x, int z) {
        Gizmos.DrawLine(new Vector3(x - 0.5f, 0.0f, z + 0.5f), new Vector3(x + 0.5f, 0.0f, z - 0.5f));
        Gizmos.DrawLine(new Vector3(x - 0.5f, 0.0f, z - 0.5f), new Vector3(x + 0.5f, 0.0f, z + 0.5f));
    }


    public bool IsFieldAllowed(ref Field field) {
        return 0 <= field.x && field.x < tilesX && 0 <= field.z && field.z < tilesZ;
    }

    public FieldStatus GetFieldStatus(int x, int z) {
        return _layout[x, z];
    }

    public void Occupy(int x, int z) {
        _layout[x, z] = FieldStatus.Occupied;
    }

    public void Free(int x, int z) {
        _layout[x, z] = FieldStatus.Free;
    }
}