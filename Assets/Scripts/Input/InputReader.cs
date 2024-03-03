using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

namespace Inputs
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "MMMIU/Input Reader")]
    public class InputReader : ScriptableObject, MyPlayerInput.IPlayerActions, MyPlayerInput.IUIActions
    {
        [SerializeField]
        private Texture2D hoverTex;

        public void SetCursorHover()
        {
            Cursor.SetCursor(hoverTex, Vector2.zero, CursorMode.Auto);
        }

        public void SetCursorDefault()
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }


        // Both
        public event UnityAction EscEvent;

        // Player
        public event UnityAction<Vector2> MoveEvent;
        public event UnityAction<Vector2> LookEvent;
        public event UnityAction AttackEvent;
        public event UnityAction InteractionEvent;
        public event UnityAction<bool> SprintEvent;
        public event UnityAction OpenDevPanelEvent;
        public event UnityAction OpenQuestPanelEvent;
        public event UnityAction JumpEvent;
        public event UnityAction SpecialSkillOneEvent;
        public event UnityAction HealSkillEvent;
        public event UnityAction PlayerRightClickStartEvent;
        public event UnityAction PlayerRightClickEndEvent;
        public event UnityAction<Vector2> PlayerPointEvent;
        public event UnityAction<Vector2> PlayerScrollWheelEvent;


        // UI
        public event UnityAction<Vector2> NavigateEvent;
        public event UnityAction SubmitEvent;
        public event UnityAction CancelEvent;
        public event UnityAction<Vector2> PointEvent;
        public event UnityAction LeftClickStartEvent;
        public event UnityAction LeftClickEndEvent;
        public event UnityAction<Vector2> ScrollWheelEvent;
        public event UnityAction MiddleClickEvent;
        public event UnityAction RightClickEvent;
        public event UnityAction CloseUIPanelEvent;
        public event UnityAction CloseDevPanelEvent;
        public event UnityAction UI_CloseQuestPanelEvent;
        public event UnityAction<Vector2> VCamMoveEvent;

        private MyPlayerInput playerInput;

        public Vector2 MouseCurrPosition;

        private void OnEnable()
        {
            if (playerInput == null)
            {
                playerInput = new MyPlayerInput();
                playerInput.Player.SetCallbacks(this);
                playerInput.UI.SetCallbacks(this);
            }
        }

        private void OnDisable()
        {
            DisableAllInput();
        }

        public void EnablePlayerInput()
        {
            Debug.Log("EnablePlayerInput");
            playerInput.UI.Disable();
            playerInput.Player.Enable();
            SetCursorDefault();
        }

        public void EnableUIInput()
        {
            Debug.Log("EnableUIInput");
            playerInput.Player.Disable();
            playerInput.UI.Enable();
            SetCursorDefault();
        }

        public void DisableAllInput()
        {
            Debug.Log("DisableAllInput");
            playerInput.Player.Disable();
            playerInput.UI.Disable();
        }

        //private bool IsDeviceMouse(InputAction.CallbackContext context) => context.control.device.name == "Mouse";

        public void OnESC(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                EscEvent?.Invoke();
            }
        }

        public void OnNavigate(InputAction.CallbackContext context)
        {
            NavigateEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                SubmitEvent?.Invoke();
            }
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                CancelEvent?.Invoke();
            }
        }

        public void OnPoint(InputAction.CallbackContext context)
        {
            MouseCurrPosition = context.ReadValue<Vector2>();
            PointEvent?.Invoke(MouseCurrPosition);
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                LeftClickStartEvent?.Invoke();
            }
            else if (context.canceled)
            {
                LeftClickEndEvent?.Invoke();
            }
        }

        public void OnScrollWheel(InputAction.CallbackContext context)
        {
            ScrollWheelEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnMiddleClick(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                MiddleClickEvent?.Invoke();
            }
        }

        public void OnRightClick(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                RightClickEvent?.Invoke();
            }
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                AttackEvent?.Invoke();
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                JumpEvent?.Invoke();
            }
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            if (context.performed || context.canceled)
            {
                SprintEvent?.Invoke(context.ReadValueAsButton());
            }
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            LookEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            MoveEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnCloseUIPanel(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                CloseUIPanelEvent?.Invoke();
            }
        }

        public void OnInteraction(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                InteractionEvent?.Invoke();
            }
        }

        public void OnOpenDevPanel(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OpenDevPanelEvent?.Invoke();
            }
        }

        public void OnCloseDevPanel(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                CloseDevPanelEvent?.Invoke();
            }
        }

        public void OnOpenQuestPanel(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                OpenQuestPanelEvent?.Invoke();
            }
        }

        public void OnCloseTaskPanel(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                UI_CloseQuestPanelEvent?.Invoke();
            }
        }

        public void OnSpecialSkillOne(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                SpecialSkillOneEvent?.Invoke();
            }
        }

        public void OnHealSkill(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                HealSkillEvent?.Invoke();
            }
        }

        public void OnVCamMove(InputAction.CallbackContext context)
        {
            VCamMoveEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnPlayerRightClick(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                PlayerRightClickStartEvent?.Invoke();
            }
            else if (context.canceled)
            {
                PlayerRightClickEndEvent?.Invoke();
            }
        }

        public void OnPlayerPoint(InputAction.CallbackContext context)
        {
            PlayerPointEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnPlayerScrollWheel(InputAction.CallbackContext context)
        {
            PlayerScrollWheelEvent?.Invoke(context.ReadValue<Vector2>());
        }
    }
}