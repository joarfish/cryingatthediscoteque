using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Architecture;
using System;
using Game.Configuration;
using Game.Systems;

namespace Application
{
    public class Main : MonoBehaviour
    {
        public GameConfiguration GameConfig;

        private ApplicationEventSystem _appEventSystem;

        private string _currentLevel;

        private readonly List<int> _cleanupStack = new List<int>();

        private GameLogic _gameLogic;

        private Scene _rootScene;

        private void Start()
        {
            MainMenu();
            PreloadLoadingScreen();
        }

        private void Awake()
        {
            SetupServices();

            _rootScene = SceneManager.GetActiveScene();
        }

        private void OnEnable()
        {
            _appEventSystem.StartGame += StartGame;
            _appEventSystem.RestartLevel += RestartLevel;
            _appEventSystem.EndGame += EndGame;
            _appEventSystem.GameOver += GameOver;
            _appEventSystem.LevelDone += LevelDone;
        }

        private void OnDisable()
        {
            _appEventSystem.StartGame -= StartGame;
            _appEventSystem.RestartLevel -= RestartLevel;
            _appEventSystem.EndGame -= EndGame;
            _appEventSystem.GameOver -= GameOver;
            _appEventSystem.LevelDone -= LevelDone;
        }

        void SetupServices()
        {
            SimpleDependencyInjection di = SimpleDependencyInjection.getInstance();

            _appEventSystem = new ApplicationEventSystem();

            di.Bind<GameConfiguration>(GameConfig);
            di.Bind<ApplicationEventSystem>(_appEventSystem);

            Debug.Log("Bound all services.");
        }

        void PreloadLoadingScreen()
        {
            StartCoroutine(LoadSceneInactive(GameConfig.loadingScreen));
        }

        void MainMenu()
        {
            StartCoroutine(LoadScene(GameConfig.mainMenu));
        }

        void StartGame()
        {
            _gameLogic = new GameLogic();
            _gameLogic.PrepareGame();

            StartCoroutine(RunSequentially(
                () => LoadLevel(1),
                CleanupScenes
            ));
        }

        void RestartLevel()
        {

            StartCoroutine(RunSequentially(
                CleanupScenes,
                UnloadLevel,
                () => LoadLevel(_currentLevel)
            ));

            _gameLogic.PrepareGame();
        }

        void EndGame()
        {
            StartCoroutine(RunSequentially(
                CleanupScenes,
                () => LoadScene(GameConfig.mainMenu),
                UnloadLevel
            ));

            _gameLogic = null;
        }

        void GameOver()
        {
            StartCoroutine(LoadScene(GameConfig.gameOverScreen));
        }

        void LevelDone()
        {
            StartCoroutine(LoadScene("LevelDone"));
        }

        delegate IEnumerator Job();

        IEnumerator RunSequentially(params Job[] fns)
        {
            for (int i = 0; i < fns.Length; i++)
            {
                yield return fns[i]();
            }
        }

        IEnumerator CleanupScenes()
        {
            int unloaded = 0;

            _cleanupStack.ForEach((sceneBuildIndex) =>
            {
                var unloadOperation = SceneManager.UnloadSceneAsync(sceneBuildIndex);
                unloadOperation.completed += (operation) =>
                {
                    unloaded++;
                };
            });

            while (unloaded != _cleanupStack.Count) yield return null;

            _cleanupStack.Clear();

            yield return new WaitForEndOfFrame();
        }

        IEnumerator LoadScene(string name, bool addToCleanupStack = true, Action<AsyncOperation> callback = null)
        {
            var loadOperation = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
            if (addToCleanupStack) loadOperation.completed += (operation) =>
            {
                _cleanupStack.Add(SceneManager.GetSceneByName(name).buildIndex);
            };

            if (callback != null) loadOperation.completed += callback;

            while (!loadOperation.isDone) yield return null;

            yield return new WaitForEndOfFrame();
        }

        IEnumerator UnloadScene(string name, Action<AsyncOperation> callback = null)
        {
            var unloadOperation = SceneManager.UnloadSceneAsync(name);
            unloadOperation.completed += (operation) =>
            {
                _cleanupStack.Remove(SceneManager.GetSceneByName(name).buildIndex);
            };

            if (callback != null) unloadOperation.completed += callback;

            while (!unloadOperation.isDone) yield return null;

            yield return new WaitForEndOfFrame();
        }

        private IEnumerator LoadSceneInactive(string name, Action<AsyncOperation> callback = null)
        {
            void SetInactive(AsyncOperation operation)
            {
                callback?.Invoke(operation);
                Scene loadScene = default;

                loadScene = SceneManager.GetSceneByName(name);

                GameObject[] objects = loadScene.GetRootGameObjects();

                foreach (var t in objects)
                {
                    t.SetActive(false);
                }
            }

            yield return LoadScene(name, false, SetInactive);
        }

        IEnumerator LoadLevel(int number)
        {
            if (GameConfig.levels.Length < number) yield break;
            yield return LoadLevel(GameConfig.levels[number - 1]);
        }

        IEnumerator LoadLevel(string name)
        {
            SceneManager.LoadSceneAsync(GameConfig.HUD, LoadSceneMode.Additive);

            yield return LoadScene(name, false, (op) => {
                _currentLevel = name;
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(name));
                _appEventSystem.SendLevelReady();
            });
        }

        IEnumerator UnloadLevel()
        {
            SceneManager.SetActiveScene(_rootScene);

            var opUnloadLevel = SceneManager.UnloadSceneAsync(_currentLevel);
            var opUnloadHUD = SceneManager.UnloadSceneAsync(GameConfig.HUD);

            while (!opUnloadHUD.isDone || !opUnloadLevel.isDone) yield return null;

            yield return new WaitForEndOfFrame();
        }
    }
}