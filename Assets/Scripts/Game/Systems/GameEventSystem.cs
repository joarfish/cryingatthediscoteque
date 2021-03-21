using System;
using Architecture;
using UnityEngine;

namespace Game.Systems {
    public class GameEventSystem : IInjectable {
        public event Action<GameState> OnGameStateChanged;
        public event Action<Field> OnPlayerInput;
        public event Action<Field> OnPlayerPositionChanged;
        public event Action<Field> OnDancerMoveToField;
        public event Action<Vector3> OnDancerHitPlayer;
        public event Action OnPlayerMovedToGoal;

        public event Action OnPlayerFailed;

        public event Action OnPlayerMovementNotAllowed;

        public void SendGameStateChanged(GameState state) {
            OnGameStateChanged?.Invoke(state);
        }

        public void SendPlayerPositionChanged(Field newField) {
            OnPlayerPositionChanged?.Invoke(newField);
        }

        public void SendDancerMoveToField(Field newField) {
            OnDancerMoveToField?.Invoke(newField);
        }

        public void SendPlayerInput(Field newField) {
            OnPlayerInput?.Invoke(newField);
        }

        public void SendDancerHitPlayer(Vector3 direction) {
            OnDancerHitPlayer?.Invoke(direction);
        }

        public void SendPlayerMovementNotAllowed() {
            OnPlayerMovementNotAllowed?.Invoke();
        }

        public void SendPlayerMovedToGoal() {
            OnPlayerMovedToGoal?.Invoke();
        }

        public void SendPlayerFailed() {
            OnPlayerFailed?.Invoke();
        }
    }
}