using Events;
using Inputs;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Invector.vCharacterController
{
    public class KnightThirdPersonInput : NetworkBehaviour
    {
        #region Variables       
        [HideInInspector] public vThirdPersonController cc;
        [HideInInspector] public vThirdPersonCamera tpCamera;
        [HideInInspector] public Camera cameraMain;
        [HideInInspector] public Animator animator;

        [SerializeField]
        private InputReader inputReader;

        private Vector2 movementValue = Vector2.zero;
        private Vector2 cameraValue = Vector2.zero;
        public bool dead = false;

        #endregion

        public override void OnNetworkSpawn()
        {
            animator = this.GetComponent<Animator>();
            InitilizeController();
            InitializeTpCamera();
            RegisterEvent();
            CursorLock();
            base.OnNetworkSpawn();
        }

        public override void OnNetworkDespawn()
        {
            UnRegisterEvent();
            base.OnNetworkDespawn();
        }

        protected virtual void FixedUpdate()
        {
            cc.UpdateMotor();               // updates the ThirdPersonMotor methods
            cc.ControlLocomotionType();     // handle the controller locomotion type and movespeed
            cc.ControlRotationType();       // handle the controller rotation type
        }

        protected virtual void Update()
        {
            FixRotation();
            InputHandle();                  // update the input methods
            cc.UpdateAnimator();            // updates the Animator Parameters
        }

        public void RegisterEvent()
        {
            inputReader.MoveEvent += GetMoveInput;
            inputReader.LookEvent += GetCameraInput;
            inputReader.SprintEvent += SprintInput;
            //inputReader.EscEvent += InteractWithUI;
            inputReader.InteractionEvent += Interact;
            inputReader.JumpEvent += JumpInput;
            EventManager.Instance.Subscribe<EnemyAttackEvent>(HitByEnemy);
            EventManager.Instance.Subscribe<PlayerDeadEvent>(Dead);
            EventManager.Instance.Subscribe<PlayerRespawnEvent>(Respawn);
        }

        private void HitByEnemy(EventBase baseEvent)
        {
            EnemyAttackEvent e = baseEvent as EnemyAttackEvent;
            if(e.playerType == Items.ItemAccessbility.knight && !dead) 
            { 
                animator.Play("HitReaction"); 
            }
        }

        public void UnRegisterEvent()
        {
            inputReader.MoveEvent -= GetMoveInput;
            inputReader.LookEvent -= GetCameraInput;
            inputReader.SprintEvent -= SprintInput;
            //inputReader.EscEvent -= InteractWithUI;
            inputReader.JumpEvent -= JumpInput;
            EventManager.Instance.Unsubscribe<EnemyAttackEvent>(HitByEnemy);
            EventManager.Instance.Unsubscribe<PlayerDeadEvent>(Dead);
            EventManager.Instance.Unsubscribe<PlayerRespawnEvent>(Respawn);
        }

        private void Respawn(EventBase baseEvent)
        {
            PlayerRespawnEvent e = baseEvent as PlayerRespawnEvent;
            if (e.playerType == Items.ItemAccessbility.knight)
            {
                GetComponent<Rigidbody>().isKinematic = false;
                //this.gameObject.layer = LayerMask.NameToLayer("Player");
                dead = false;
                animator.SetBool("Died", false);
            }
        }

        private void Dead(EventBase baseEvent)
        {
            if (IsLocalPlayer)
            {
                PlayerDeadEvent e = baseEvent as PlayerDeadEvent;
                if (e.playerType == Items.ItemAccessbility.knight)
                {
                    GetComponent<Rigidbody>().isKinematic = true;
                    this.gameObject.GetComponent<Collider>().enabled = false;
                    dead = true;
                    //this.gameObject.layer = LayerMask.NameToLayer("DeadPlayer");
                    animator.Play("Death");
                    animator.SetBool("Died", true);
                    Invoke(nameof(ResetCollider), 2f);
                }
            }
        }

        private void ResetCollider()
        {
            if (!IsServer)
            {
                return;
            }
            this.gameObject.GetComponent<Collider>().enabled = true;
        }

        private void Interact()
        {
            animator.Play("Interaction");
        }

        private void GetCameraInput(Vector2 arg0)
        {
            cameraValue = arg0;
        }

        private void GetMoveInput(Vector2 arg0)
        {
            movementValue = arg0;
        }

        //public void InteractWithUI()
        //{
        //    interactWithUI = !interactWithUI;
        //    if(interactWithUI) 
        //    { 
        //        CursorUnLock(); 
        //    }
        //    else 
        //    { 
        //        CursorLock(); 
        //    }
        //}

        public virtual void OnAnimatorMove()
        {
            cc.ControlAnimatorRootMotion(); // handle root motion animations 
        }

        public void CursorLock()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void CursorUnLock()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }


        #region Basic Locomotion Inputs

        protected virtual void InitilizeController()
        {
            cc = GetComponent<vThirdPersonController>();

            if (cc != null)
                cc.Init();
        }

        protected virtual void InitializeTpCamera()
        {
            if (tpCamera == null)
            {
                tpCamera = FindObjectOfType<vThirdPersonCamera>();
                if (tpCamera == null)
                    return;
                if (tpCamera)
                {
                    tpCamera.SetMainTarget(this.transform);
                    tpCamera.Init();
                }
            }
        }

        protected virtual void InputHandle()
        {
            MoveInput();
            CameraInput();
            //SprintInput();
            //StrafeInput();
            //JumpInput();
        }

        public void FixRotation()
        {
            if (transform.eulerAngles.x != 0f || transform.eulerAngles.z != 0f)
            {
                transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
            }
        }

        public virtual void MoveInput()
        {
            float x = movementValue.x;
            float z = movementValue.y;

            if (animator.GetBool("Interruptible") && (x != 0 || z != 0) && !dead)
            {
                if (animator.GetBool("Attacking"))
                {
                    animator.SetBool("Attacking", false);
                    animator.SetBool("AttackAgain", false);
                }
                cc.input.x = x;
                cc.input.z = z;
            }
            else
            {
                cc.input.x = 0;
                cc.input.z = 0;
            }
        }

        protected virtual void CameraInput()
        {

            if (!cameraMain)
            {
                if (!Camera.main) Debug.Log("Missing a Camera with the tag MainCamera, please add one.");
                else
                {
                    cameraMain = Camera.main;
                    cc.rotateTarget = cameraMain.transform;
                }
            }

            if (cameraMain)
            {
                cc.UpdateMoveDirection(cameraMain.transform);
            }

            if (tpCamera == null)
                return;

            var Y = cameraValue.y;
            var X = cameraValue.x;



            tpCamera.RotateCamera(X, Y);
        }

        protected virtual void StrafeInput()
        {
            //if (Input.GetKeyDown(strafeInput))
            //    cc.Strafe();
        }

        protected virtual void SprintInput(bool isSprint)
        {
            if (!dead)
            {
                cc.Sprint(isSprint);
            }
        }

        /// <summary>
        /// Conditions to trigger the Jump animation & behavior
        /// </summary>
        /// <returns></returns>
        protected virtual bool JumpConditions()
        {
            return cc.isGrounded && cc.GroundAngle() < cc.slopeLimit && !cc.isJumping && !cc.stopMove;
        }

        /// <summary>
        /// Input to trigger the Jump 
        /// </summary>
        protected virtual void JumpInput()
        {
            if (JumpConditions() && !dead)
            {
                if (animator.GetBool("Interruptible"))
                {
                    if (animator.GetBool("Attacking"))
                    {
                        animator.SetBool("Attacking", false);
                        animator.SetBool("AttackAgain", false);
                    }
                    cc.Jump();
                }
            }
        }

        #endregion
    }
}