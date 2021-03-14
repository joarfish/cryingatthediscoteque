using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rhythm : MonoBehaviour {
    [Range(0.00f, 2.00f)] public float tolerance = 0.1f;
    public float span = 0.5f;

    private float _timer = 0.0f;
    private FloorLayout _floorLayout;
    private AudioSource _audioSource;
    private int _beatCount = 0;

    private void OnEnable() {
        _floorLayout = FindObjectOfType<FloorLayout>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start() {
        _audioSource.Play();
        _timer = 0.0f;
    }

    private void Update() {
        var newTimer = _audioSource.time % span;
        if (_timer > newTimer) {
            _beatCount++;
        }

        _timer = newTimer;
    }

    public bool isMoveAllowed() {
        var currentTime = (_audioSource.time % span);
        return currentTime >= span - tolerance || currentTime <= tolerance;
    }

    public float getTimeFragment() {
        var currentTime = (_audioSource.time % span);
        return currentTime / span;
    }

    public int getBeatCount() {
        return _beatCount;
    }
}