using InputHandling;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
namespace InputHandling
{
    using static InputSystem_Actions;

    public interface IInputReader
    {
        Vector3 moveInput { get; }
        void EnablePlayerActions();
        void DisablePlayerActions();
    }

    [CreateAssetMenu(fileName = "New Input Reader", menuName = "Input/Input Reader")]
    public class InputReader : ScriptableObject, IPlayerActions, IInputReader
    {
        #region Player Action Events
        public event UnityAction<Vector2> MoveEvent = delegate { };
        
        public event UnityAction<bool> JumpEvent = delegate { };
        public event UnityAction<bool> LeftArmAttackEvent = delegate { };
        public event UnityAction<bool> RightArmAttackEvent = delegate { };

        #endregion
        #region Editor Action Test Events
        public event UnityAction<Vector2> LookEvent = delegate { };
        public event UnityAction<bool> InteractEvent = delegate { };
        
        #endregion
        
        InputSystem_Actions _inputActions;
        
        public Vector3 moveInput => _inputActions.Player.Move.ReadValue<Vector2>();
        public Vector3 lookInput => _inputActions.Player.Look.ReadValue<Vector2>();
        public bool jump => _inputActions.Player.Jump.IsPressed();
        public bool interact => _inputActions.Player.Interact.IsPressed();
        public bool leftArmAttack => _inputActions.Player.LeftArmAttack.IsPressed();
        public bool rightArmAttack => _inputActions.Player.RightArmAttack.IsPressed();
        
        
        //-----------------------------------------------------------------------------
        public void EnablePlayerActions()
        {
            if (_inputActions is null)
            {
                _inputActions = new InputSystem_Actions();
                _inputActions.Player.SetCallbacks(this);
            }
            _inputActions.Player.Enable();
        }
        public void DisablePlayerActions()
        {
            if (_inputActions is null)
                return;
            _inputActions.Player.Disable();
            _inputActions = null;
        }

        #region InputSystem_Actions.IPlayerActions Members

        //*************IMPLEMENTED METHODS*************
        public void OnMove(InputAction.CallbackContext context)
        {
            MoveEvent.Invoke(context.ReadValue<Vector2>());
        }
        public void OnLook(InputAction.CallbackContext context)
        {
            LookEvent.Invoke(context.ReadValue<Vector2>());
        }
        public void OnJump(InputAction.CallbackContext context)
        {
            JumpEvent?.Invoke(context.action.WasPressedThisFrame());
        }
        public void OnLeftArmAttack(InputAction.CallbackContext context)
        {
            LeftArmAttackEvent?.Invoke(context.action.WasPressedThisFrame());
        }
        public void OnRightArmAttack(InputAction.CallbackContext context)
        {
            RightArmAttackEvent?.Invoke(context.action.WasPressedThisFrame());
        }
        public void OnInteract(InputAction.CallbackContext context)
        {
            InteractEvent?.Invoke(context.action.WasPressedThisFrame());
        }
        //********************************************

        //*************NOT IMPLEMENTED METHODS*************
        public void OnAttack(InputAction.CallbackContext context)
        {
            //noop
        }
        public void OnCrouch(InputAction.CallbackContext context)
        {
            //noop
        }
        public void OnPrevious(InputAction.CallbackContext context)
        {
            //noop
        }
        public void OnNext(InputAction.CallbackContext context)
        {
            //noop
        }
        public void OnSprint(InputAction.CallbackContext context)
        {
            //noop
        }
        //********************************************
        #endregion
    }
}