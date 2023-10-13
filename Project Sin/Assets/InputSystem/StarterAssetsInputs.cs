using UnityEditor.Rendering.LookDev;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool dash;

        // Input Values
        public float thrust1D;
        public float upDown1D;
        public float strafe1D;
        public float roll1D;
        public Vector2 pitchYaw;

        [Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

        public void OnDash(InputValue value)
        {
            DashInput(value.isPressed);
        }

        public void OnThrust(InputValue value)
        {
            //thrust1D = context.ReadValue<float>();
            ThrustInput(value.Get<float>());
        }

		public void OnStrafe(InputValue value)
		{
            StrafeInput(value.Get<float>());
        }

        public void OnUpDown(InputValue value)
        {
            UpDownInput(value.Get<float>());
        }

        public void OnRoll(InputValue value)
        {
            RollInput(value.Get<float>());
        }

        public void OnPitchYaw(InputValue value)
        {
            PitchYawInput(value.Get<Vector2>());
        }

        //public void OnThrust(InputAction.CallbackContext context)
        //{
        //    thrust1D = context.ReadValue<float>();
        //    Debug.Log("I'm pressing W or S");
        //}
        //public void OnStrafe(InputAction.CallbackContext context)
        //{
        //    strafe1D = context.ReadValue<float>();
        //}
        //public void OnUpDown(InputAction.CallbackContext context)
        //{
        //    upDown1D = context.ReadValue<float>();
        //}
        //public void OnRoll(InputAction.CallbackContext context)
        //{
        //    roll1D = context.ReadValue<float>();
        //    Debug.Log("I'm pressing e Or q");
        //}
        //public void OnPitchYaw(InputAction.CallbackContext context)
        //{
        //    pitchYaw = context.ReadValue<Vector2>();
        //}
        // ------------------------------------------------------------------------------------------------------------------------------
#endif
        public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

        public void DashInput(bool newDashState)
        {
            dash = newDashState;
			//Debug.Log("Dash Input is called");
        }

		public void ThrustInput(float newThrustState)
		{
			thrust1D = newThrustState;
        }
        public void StrafeInput(float newThrustState)
        {
            strafe1D = newThrustState;
        }

        public void UpDownInput(float newThrustState)
        {
            upDown1D = newThrustState;
        }

        public void RollInput(float newThrustState)
        {
            roll1D = newThrustState;
        }

        public void PitchYawInput(Vector2 newThrustState)
        {
            pitchYaw = newThrustState;
        }


        private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}