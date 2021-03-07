using System;
using Architecture;
using UnityEngine;

namespace Game.Systems {
    public class GameEventSystem : IInjectable {
        public event Action<GameState> OnGameStateChanged;
        public event Action OnReachedGoalArea;
        public event Action OnDronesCoughtPlayer;

        public event Action<bool> OnGroundedChanged;
        public event Action<float> OnRotationChanged;
        public event Action<Vector3> OnPositionChanged;

        public event Action<float> OnUpdateTrailLength;

        public void SendGroundedChanged(bool grounded) {
            OnGroundedChanged?.Invoke(grounded);
        }

        public void SendRotationChanged(float rotation) {
            OnRotationChanged?.Invoke(rotation);
        }

        public void SendPositionChanged(Vector3 position) {
            OnPositionChanged?.Invoke(position);
        }

        public void SendUpdateTrailLength(float length) {
            OnUpdateTrailLength?.Invoke(length);
        }

        public void SendGameStateChanged(GameState state) {
            OnGameStateChanged?.Invoke(state);
        }

        public void SendReachedGoalArea() {
            OnReachedGoalArea?.Invoke();
        }

        public void SendDronesReachedPlayer() {
            OnDronesCoughtPlayer?.Invoke();
        }
    }
}