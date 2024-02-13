using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

namespace Inputs
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "MMMIU/Input Reader")]
    public class InputReader : ScriptableObject, MyPlayerInput.IPlayerActions, MyPlayerInput.IUIActions
    {
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


        // UI
        public event UnityAction<Vector2> NavigateEvent;
        public event UnityAction SubmitEvent;
        public event UnityAction CancelEvent;
        public event UnityAction<Vector2> PointEvent;
        public event UnityAction ClickEvent;
        public event UnityAction<Vector2> ScrollWheelEvent;
        public event UnityAction MiddleClickEvent;
        public event UnityAction RightClickEvent;
        public event UnityAction CloseUIPanelEvent;
        public event UnityAction CloseDevPanelEvent;
        public event UnityAction UI_CloseQuestPanelEvent;
        public event UnityAction<Vector2> VCamMoveEvent;

        private MyPlayerInput playerInput;

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
        }

        public void EnableUIInput()
        {
            Debug.Log("EnableUIInput");
            playerInput.Player.Disable();
            playerInput.UI.Enable();
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
            PointEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                ClickEvent?.Invoke();
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

        public void OnVCamMove(InputAction.CallbackContext context)
        {
            VCamMoveEvent?.Invoke(context.ReadValue<Vector2>());
        }
    }
}