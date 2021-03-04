using UnityEngine;
using Architecture;
using Game.Systems;
using Game.Helpers;
using Game.Systems;

namespace Game
{
    namespace Entities
    {
        public class RollerController : MonoBehaviour
        {
            CharacterController controller;

            LayerMask groundLayer;

            readonly float turnSpeedGrounded = 180.0f;
            readonly float turnSpeedAirborne = 250.0f;
            readonly float normalSpeed = 8.0f;
            readonly float fastSpeed = 20.0f;
            readonly float slowSpeed = 8.0f;
            readonly float speedDecay = 0.025f;
            readonly float jumpForce = 30.0f;
            readonly float gravity = 100.0f;

            bool wasGrounded = false;

            float rotation = 0f;
            Vector3 movementDir;
            bool run = false;

            Animator animator;

            public LayerMask obstacleMask;

            [Inject] GameEventSystem gameEventSystem;

            PausableAnimation pausableAnimation;

            private void Awake()
            {
                controller = GetComponent<CharacterController>();
                animator = GetComponent<Animator>();

                pausableAnimation = new PausableAnimation(animator);

                SimpleDependencyInjection.getInstance().Inject(this);

                gameEventSystem.OnGameStateChanged += GameStateChanged;
            }

            private void OnDisable()
            {
                gameEventSystem.OnGameStateChanged -= GameStateChanged;
            }

            private void GameStateChanged(GameState state)
            {
                if (state == GameState.Running) pausableAnimation.PauseAnimation();
                else pausableAnimation.ResumeAnimation();

                run = state == GameState.Running;
            }


            private void Update()
            {
                if (Input.GetKeyDown("p"))
                {
                    gameEventSystem.SendGameStateChanged(run ? GameState.Paused : GameState.Running);
                }

                if (!run)
                {
                    return;
                }

                float y = movementDir.y;
                float speed = normalSpeed;
                float targetRotation = 0.0f;

                bool isGrounded = controller.isGrounded;

                movementDir = transform.forward;

                if (Input.GetKey("w"))
                {
                    speed = fastSpeed;
                }
                else if (Input.GetKey("s"))
                {
                    speed = slowSpeed;
                }

                if (Input.GetKey("a"))
                {
                    targetRotation = -1.0f;
                }
                else if (Input.GetKey("d"))
                {
                    targetRotation = 1.0f;
                }

                transform.Rotate(Vector3.up * turnSpeedGrounded * Time.deltaTime * targetRotation);
                rotation = Mathf.MoveTowards(rotation, targetRotation, Time.deltaTime * 5.0f);
                animator.SetFloat("rotation", rotation);

                if (targetRotation != 0)
                {
                    gameEventSystem.SendRotationChanged(rotation);
                }

                if (isGrounded)
                {
                    if (Input.GetKeyDown("space"))
                    {
                        y = jumpForce;
                        animator.SetBool("jump", true);
                    }
                    else if (!wasGrounded)
                    {
                        gameEventSystem.SendGroundedChanged(true);
                    }

                    speed = Mathf.MoveTowards(speed, normalSpeed, Time.deltaTime * speedDecay);
                }
                else
                {
                    if (wasGrounded)
                    {
                        gameEventSystem.SendGroundedChanged(false);
                    }

                    y -= gravity * Time.deltaTime;

                    if (y < 0.0f)
                    {
                        y *= 1.05f;
                    }

                    speed = Mathf.MoveTowards(speed, 0.0f, Time.deltaTime * speedDecay);

                    if (Physics.Raycast(transform.position, -transform.up, 1.0f, obstacleMask.value))
                    {
                        animator.SetBool("jump", false);
                    }
                }

                animator.SetFloat("roller_velocity", Mathf.InverseLerp(slowSpeed, fastSpeed, speed));

                movementDir *= speed;
                movementDir.y = y;

                wasGrounded = isGrounded;

                controller.Move(movementDir * Time.deltaTime);

                gameEventSystem.SendPositionChanged(transform.position);
            }

            private void FixedUpdate()
            {
                if (run && (controller.collisionFlags & CollisionFlags.Sides) != 0)
                {
                    Vector3 postion = transform.position;
                    Vector3 dir = transform.forward;

                    RaycastHit hitInfo;

                    if (Physics.Raycast(postion, dir.normalized, out hitInfo, 2.0f, obstacleMask.value))
                    {
                        Debug.Log("There is an obstacle in front of us!");
                    }
                }
            }
        }
    }
}