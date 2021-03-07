using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DancerCoordinator : MonoBehaviour {
    private List<Dancer> _dancers = new List<Dancer>();
    private int _lastBeatMoved = -1;
    private FloorLayout _floorLayout;
    private Rhythm _rhythm;

    private void OnEnable() {
        _floorLayout = FindObjectOfType<FloorLayout>();
        _rhythm = FindObjectOfType<Rhythm>();
    }

    public void registerDancer(Dancer dancer) {
        _dancers.Add(dancer);
    }

    public void unregisterDancer(Dancer dancer) {
        _dancers.Remove(dancer);
    }

    private void Update() {
        var currentBeat = _rhythm.getBeatCount();

        if (currentBeat == _lastBeatMoved) return;

        _dancers.ForEach((dancer) => { dancer.Move(); });

        _lastBeatMoved = currentBeat;
    }
}