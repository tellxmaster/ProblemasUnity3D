using UnityEngine;
using Photon.Pun;

namespace Com.MyCompany.MyGame
{
    public class PlayerAnimatorManager : MonoBehaviourPun
    {
        #region Private Fields

        [SerializeField]
        private float moveSpeed = 5.0f;
        [SerializeField]
        private float rotateSpeed = 200.0f;
        [SerializeField]
        private float jumpForce = 1.0f; // Lowered the jumpForce
        [SerializeField]
        private float gravity = -20.0f;

        private Animator animator;
        private CharacterController controller;
        private float verticalSpeed;

        #endregion

        #region MonoBehaviourPun CallBacks

        void Start()
        {
            animator = GetComponent<Animator>();
            if (!animator)
            {
                Debug.LogError("PlayerAnimatorManager is Missing Animator Component", this);
            }

            controller = GetComponent<CharacterController>();
            if (!controller)
            {
                Debug.LogError("PlayerAnimatorManager is Missing CharacterController Component", this);
            }
        }

        void Update()
        {
            if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
            {
                return;
            }

            if (!animator || !controller)
            {
                return;
            }

            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            Vector3 forward = Camera.main.transform.forward;
            Vector3 right = Camera.main.transform.right;
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            Vector3 move = v * forward + h * right;

            animator.SetFloat("Speed", move.magnitude);

            if (move != Vector3.zero)
            {
                Quaternion newRotation = Quaternion.LookRotation(move);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, rotateSpeed * Time.deltaTime);

                // Set the direction parameter based on the dot product between move and forward vectors
                float direction = Vector3.Dot(move.normalized, forward);
                animator.SetFloat("Direction", direction);
            }
            else
            {
                // If the character is not moving, set the direction to 0
                animator.SetFloat("Direction", 0);
            }

            if (controller.isGrounded)
            {
                verticalSpeed = 0;

                if (Input.GetButtonDown("Jump"))
                {
                    verticalSpeed = jumpForce;
                    animator.SetTrigger("Jump");
                }
            }
            else
            {
                verticalSpeed += gravity * Time.deltaTime;
            }

            // Apply vertical speed
            move.y = verticalSpeed;

            // Apply movement
            controller.Move(move * moveSpeed * Time.deltaTime);

        }


        #endregion
    }
}
