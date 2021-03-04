using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Architecture;
using Game.Systems;

namespace Game
{
    namespace Entities
    {
        public class GoalArea : MonoBehaviour
        {
            BoxCollider boxCollider;
            [Inject] GameEventSystem gameEventSystem;

            private void Awake()
            {
                boxCollider = GetComponent<BoxCollider>();
                SimpleDependencyInjection.getInstance().Inject(this);

            }

            private void OnTriggerEnter(Collider other)
            {
                if (other.gameObject.tag.Contains("Player"))
                {
                    gameEventSystem.SendReachedGoalArea();
                }
            }
        }
    }
}

