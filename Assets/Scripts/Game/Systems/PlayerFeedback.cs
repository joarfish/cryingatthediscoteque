using System;
using System.Collections;
using System.Collections.Generic;
using Architecture;
using Game.Systems;
using UnityEngine;

public class PlayerFeedback : MonoBehaviour
{
    // Start is called before the first frame update
    [Inject] private GameEventSystem _gameEventSystem;
    private Camera _mainCamera;
    private Color _defaultBackgroundColor;
    private float _animationTime;
    private readonly Color _notallowedColor = Color.red;

    public PlayerFeedback() {
        SimpleDependencyInjection.getInstance().Inject(this);
    }
    
    private void OnEnable() {
        _mainCamera = Camera.main;
        _defaultBackgroundColor = _mainCamera.backgroundColor;
        _gameEventSystem.OnPlayerMovementNotAllowed += HandlePlayerMovementNotAllowed;
    }

    private void OnDisable() {
        _gameEventSystem.OnPlayerMovementNotAllowed -= HandlePlayerMovementNotAllowed;
    }

    private void HandlePlayerMovementNotAllowed() {
        Debug.Log("Movement not allowed!");
        _mainCamera.backgroundColor = _notallowedColor;
        _animationTime = 0.0f;
    }

    private void Update() {
        if (_mainCamera.backgroundColor != _defaultBackgroundColor) {
            _animationTime += Time.deltaTime;
            _mainCamera.backgroundColor = Color.Lerp(_notallowedColor, _defaultBackgroundColor, _animationTime / 0.5f);
        }
    }
}
