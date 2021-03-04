using System.Collections;
using Application;
using Architecture;
using Helpers;
using TMPro;
using UnityEngine;

namespace Game.Systems
{
    public enum GameState
    {
        Paused,
        Running,
        GameOver,
        LevelComplete
    }

    public class GameLogic
    {
        private GameEventSystem _gameEventSystem;

        [Inject] private ApplicationEventSystem _appEventSystem;
        [Inject] private Scheduler _scheduler;

        private GameState _state;

        public GameLogic()
        {
            SimpleDependencyInjection.getInstance().Inject(this);
        }

        public void PrepareGame()
        {
            _appEventSystem.LevelReady += StartGame;
            _gameEventSystem = new GameEventSystem();

            SimpleDependencyInjection.getInstance().Bind<GameEventSystem>(_gameEventSystem);

            _gameEventSystem.OnReachedGoalArea += handleReachedGoal;
            _gameEventSystem.OnDronesCoughtPlayer += handleDronesCaughtPlayer;
        }

        public void StartGame()
        {
            SwitchState(GameState.Paused);
            _appEventSystem.LevelReady -= StartGame;
            _gameEventSystem.OnPositionChanged += checkFallingToDeath;

            _scheduler.StartCoroutine(Countdown());
        }

        private void SwitchState(GameState state)
        {
            _state = state;
            _gameEventSystem.SendGameStateChanged(_state);
        }

        IEnumerator Countdown()
        {
            GameObject countdown = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("countdown"));
            countdown.name = "Countdown";

            TextMeshProUGUI text = countdown.GetComponentInChildren<TextMeshProUGUI>();

            for (int i = 0; i < 3; i++)
            {
                text.text = (3 - i).ToString();
                yield return new WaitForSeconds(1);
            }

            text.text = "Go!";
            SwitchState(GameState.Running);
            UnityEngine.Object.Destroy(countdown);
        }

        void checkFallingToDeath(Vector3 position)
        {
            if (_state == GameState.Running && position.y < -5.0f)
            {
                SwitchState(GameState.GameOver);
                _appEventSystem.SendGameOver();
            }
        }

        void handleReachedGoal()
        {
            SwitchState(GameState.LevelComplete);
            _appEventSystem.SendLevelDone();
        }

        void handleDronesCaughtPlayer()
        {
            SwitchState(GameState.GameOver);
            _appEventSystem.SendGameOver();
        }

        ~GameLogic()
        {
            SimpleDependencyInjection.getInstance().Unbind<GameEventSystem>();
            _appEventSystem.LevelReady -= StartGame;
            _gameEventSystem.OnReachedGoalArea -= handleReachedGoal;
            _gameEventSystem.OnPositionChanged -= checkFallingToDeath;
            _gameEventSystem.OnDronesCoughtPlayer -= handleDronesCaughtPlayer;
        }
    }
}
