using System;
using Architecture;
using UnityEngine;

namespace Game.Systems {
    public class GameEventSystem : IInjectable {
        public event Action<GameState> OnGameStateChanged;
        public event Action<Field> OnPlayerInput;
        public event Action<Field> OnPlayerPositionChanged;
        public event Action<Field> OnDancerMoveToField;
        public event Action OnDancerHitPlayer;

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

        public void SendDancerHitPlayer() {
            OnDancerHitPlayer?.Invoke();
        }
    }
}