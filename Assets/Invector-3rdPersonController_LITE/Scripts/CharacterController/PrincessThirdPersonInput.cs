using Inputs;
using Unity.Netcode;
using UnityEngine;

namespace Invector.vCharacterController
{
    public class PrincessThirdPersonInput : NetworkBehaviour
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

            InputHandle();                  // update the input methods
            cc.UpdateAnimator();            // updates the Animator Parameters

            //if(Input.GetKeyDown(KeyCode.M))
            //{
            //    InteractWithUI();
            //}
        }

        public void RegisterEvent()
        {
            inputReader.MoveEvent += GetMoveInput;
            inputReader.LookEvent += GetCameraInput;
            inputReader.SprintEvent += SprintInput;
            //inputReader.EscEvent += InteractWithUI;
            inputReader.JumpEvent += JumpInput;
        }

        public void UnRegisterEvent()
        {
            inputReader.MoveEvent -= GetMoveInput;
            inputReader.LookEvent -= GetCameraInput;
            inputReader.SprintEvent -= SprintInput;
            //inputReader.EscEvent -= InteractWithUI;
            inputReader.JumpEvent -= JumpInput;
        }

        //public void InteractWithUI()
        //{
        //    interactWithUI = !interactWithUI;
        //    if (interactWithUI)
        //    {
        //        CursorUnLock();
        //    }
        //    else
        //    {
        //        CursorLock();
        //    }
        //}

        private void GetCameraInput(Vector2 arg0)
        {
            cameraValue = arg0;
        }

        private void GetMoveInput(Vector2 arg0)
        {
            movementValue = arg0;
        }

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


        public virtual void MoveInput()
        {
            float x = movementValue.x;
            float z = movementValue.y;

            if (x != 0 || z != 0)
            {
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
            cc.Sprint(isSprint);
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
            if (JumpConditions())
            {
                cc.Jump();
            }
        }

        #endregion       
    }
}