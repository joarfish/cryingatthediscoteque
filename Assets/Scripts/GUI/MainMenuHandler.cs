using UnityEngine;
using Architecture;
using Application;

namespace GUI {
    public class MainMenuHandler : MonoBehaviour {
        [Inject] ApplicationEventSystem _appEventSystem;

        MainMenuHandler() {
            SimpleDependencyInjection.getInstance().Inject(this);
        }

        public void OnClickStartGame() {
            _appEventSystem.SendStartGame();
        }
    }
}