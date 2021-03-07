using System;
using Architecture;

namespace Application {
    public class ApplicationEventSystem : IInjectable {
        public event Action StartGame;
        public event Action RestartLevel;
        public event Action NextLevel;
        public event Action EndGame;

        public event Action ShowCountdown;

        public event Action GameOver;
        public event Action LevelDone;

        public event Action LevelReady;

        public void SendStartGame() {
            StartGame?.Invoke();
        }

        public void SendRestartLevel() {
            RestartLevel?.Invoke();
        }

        public void SendNextLevel() {
            NextLevel?.Invoke();
        }

        public void SendEndGame() {
            EndGame?.Invoke();
        }

        public void SendGameOver() {
            GameOver?.Invoke();
        }

        public void SendLevelReady() {
            LevelReady?.Invoke();
        }

        public void SendLevelDone() {
            LevelDone?.Invoke();
        }

        public void SendShowCountdown() {
            ShowCountdown?.Invoke();
        }
    }
}