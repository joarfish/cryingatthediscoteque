using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Architecture;


namespace Game
{
    namespace Configuration
    {
        [CreateAssetMenu(fileName = "CryingAtTheDiscoteque", menuName = "CryingAtTheDiscoteque")]
        public class GameConfiguration : ScriptableObject, IInjectable
        {
            public string mainMenu;
            public string loadingScreen;
            public string gameOverScreen;
            public string[] levels;
            public string HUD;
        }
    }
}
