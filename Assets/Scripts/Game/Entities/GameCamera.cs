using Architecture;
using Game.Systems;
using UnityEngine;

namespace Game.Entities
{
    public class GameCamera : MonoBehaviour
    {
        Vector3 lastPlayerPosition;
        Vector3 velocity;

        [Inject] private GameEventSystem gameEventSystem;

        bool running = false;

        GameCamera()
        {
            SimpleDependencyInjection.getInstance().Inject(this);
        }

        private void OnEnable()
        {
            gameEventSystem.OnPositionChanged += handlePlayerPositionChanged;
            gameEventSystem.OnGameStateChanged += handleGameStateChanged;
        }

        private void OnDisable()
        {
            gameEventSystem.OnPositionChanged -= handlePlayerPositionChanged;
            gameEventSystem.OnGameStateChanged -= handleGameStateChanged;
        }

        void handleGameStateChanged(GameState state)
        {
            running = state == GameState.Running;
        }


        void handlePlayerPositionChanged(Vector3 position)
        {
            if (!running)
            {
                return;
            }

            position.y = 0f;

            Vector3 playerVelocity = position - lastPlayerPosition;

            velocity = Vector3.MoveTowards(velocity, playerVelocity, Time.deltaTime * 1.0f);

            transform.Translate(velocity, Space.World);

            lastPlayerPosition = position;
        }
    }
}


