using UnityEngine;

namespace Game
{
    namespace Helpers
    {
        class PausableAnimation
        {
            private float _speed;
            private Animator _animator;

            public PausableAnimation(Animator animator)
            {
                _animator = animator;
            }

            public void PauseAnimation()
            {
                if (_animator.speed == 0) return;

                _speed = _animator.speed;
                _animator.speed = 0;
            }

            public void ResumeAnimation()
            {
                _animator.speed = _speed;
            }
        }
    }
}


