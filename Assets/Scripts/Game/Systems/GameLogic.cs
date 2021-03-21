using System;
using Application;
using Architecture;
using Helpers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Systems {
    public enum GameState {
        Paused,
        Running,
        GameOver,
        LevelComplete
    }

    public class GameLogic {
        private GameEventSystem _gameEventSystem;

        [Inject] private ApplicationEventSystem _appEventSystem;
        [Inject] private Scheduler _scheduler;

        private GameState _state;
        private Rhythm _currentRhythm;
        private PlayerControls _player;
        private FloorLayout _floorLayout;

        public GameLogic() {
            SimpleDependencyInjection.getInstance().Inject(this);
        }

        public void PrepareGame() {
            _appEventSystem.LevelReady += StartGame;
            _gameEventSystem = new GameEventSystem();

            SimpleDependencyInjection.getInstance().Bind<GameEventSystem>(_gameEventSystem);

            _gameEventSystem.OnPlayerInput += HandlePlayerInput;
            _gameEventSystem.OnDancerHitPlayer += HandleDancerHitPlayer;
            _gameEventSystem.OnPlayerMovedToGoal += HandlePlayerMovedToGoal;
            _gameEventSystem.OnPlayerFailed += HandlePlayerFailed;
        }

        public void StartGame() {
            SwitchState(GameState.Paused);
            _appEventSystem.LevelReady -= StartGame;

            _currentRhythm = Object.FindObjectOfType<Rhythm>();
            _player = Object.FindObjectOfType<PlayerControls>();
            _floorLayout = Object.FindObjectOfType<FloorLayout>();
        }

        private void SwitchState(GameState state) {
            _state = state;
            _gameEventSystem.SendGameStateChanged(_state);
        }

        private int _lastPlayerBeat = -1;
        private float _lastPlayerMove = -1.0f;

        private void HandlePlayerInput(Field newField) {
            var currentBeat = _currentRhythm.getBeatCount();
            var currentTime = Time.time;
            if (!_currentRhythm.isMoveAllowed() || (currentBeat == _lastPlayerBeat && currentTime - _lastPlayerMove > 0.15f)) {
                _gameEventSystem.SendPlayerMovementNotAllowed();
                return;
            }

            if (currentBeat == _lastPlayerBeat && currentTime - _lastPlayerMove <= 0.15f) {
                Debug.Log("Double Beat!");
            }
            
            if (_floorLayout.IsFieldAllowed(newField) &&
                _floorLayout.GetFieldStatus(newField) == FieldStatus.Free) {
                _lastPlayerBeat = currentBeat;
                _lastPlayerMove = currentTime;
                _player.Move(newField);
                _gameEventSystem.SendPlayerPositionChanged(newField);
            }
        }

        private void HandleDancerHitPlayer(Vector3 direction) {
            var currentPlayerField = _floorLayout.GetPlayerField();
            direction.y = 0.0f;
            direction.Normalize();
            var nextField1 = currentPlayerField.Clone().SubtractByVector3(direction * 1.0f);
            var nextField2 = currentPlayerField.Clone().SubtractByVector3(direction * 2.0f);
            
            if (!_floorLayout.IsFieldAllowed(nextField1) || !_floorLayout.IsFieldAllowed(nextField2)) {
                _gameEventSystem.SendPlayerFailed();
                return;
            }

            var nextField1Status = _floorLayout.GetFieldStatus(nextField1);
            var nextField2Status = _floorLayout.GetFieldStatus(nextField2);

            switch (nextField1Status, nextField2Status) {
                case (FieldStatus.Free, FieldStatus.Free):
                case (FieldStatus.Hole, FieldStatus.Free):
                    _player.Move(nextField2);
                    _gameEventSystem.SendPlayerPositionChanged(nextField2);
                    break;
                case (FieldStatus.Free, FieldStatus.Invalid):
                    _player.Move(nextField1);
                    _gameEventSystem.SendPlayerPositionChanged(nextField1);
                    break;
                case (_, _):
                    Debug.Log("default case?");
                    _gameEventSystem.SendPlayerFailed();
                    break;
            }
        }

        private void HandlePlayerMovedToGoal() {
            Debug.Log("You won the level!");
        }

        private void HandlePlayerFailed() {
            _appEventSystem.SendRestartLevel();
        }

        ~GameLogic() {
            SimpleDependencyInjection.getInstance().Unbind<GameEventSystem>();
            _appEventSystem.LevelReady -= StartGame;
            _gameEventSystem.OnPlayerInput -= HandlePlayerInput;
            _gameEventSystem.OnDancerHitPlayer -= HandleDancerHitPlayer;
            _gameEventSystem.OnPlayerMovedToGoal -= HandlePlayerMovedToGoal;
            _gameEventSystem.OnPlayerFailed -= HandlePlayerFailed;
        }
    }
}