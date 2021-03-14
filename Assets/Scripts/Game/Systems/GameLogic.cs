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

        private void HandlePlayerInput(Field newField) {
            if (!_currentRhythm.isMoveAllowed()) {
                
            }
            else {
                if (_floorLayout.IsFieldAllowed(newField) &&
                    _floorLayout.GetFieldStatus(newField) == FieldStatus.Free) {
                    _player.Move(newField);
                    _gameEventSystem.SendPlayerPositionChanged(newField);
                    var camera = Camera.main;
                    if (camera) {
                        Camera.main.backgroundColor = Color.green;
                    }
                }
            }
        }

        private void HandleDancerHitPlayer() {
            Camera.main.backgroundColor = Color.red;
            _player.BounceBack();
        }

        ~GameLogic() {
            SimpleDependencyInjection.getInstance().Unbind<GameEventSystem>();
            _appEventSystem.LevelReady -= StartGame;
            _gameEventSystem.OnPlayerInput -= HandlePlayerInput;
            _gameEventSystem.OnDancerHitPlayer -= HandleDancerHitPlayer;
        }
    }
}