using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour {
  private const float INPUT_THRESHOLD = 0.2f;

  private PlayerMotor motor;
  private PlayerControls controls;

  private void Awake() {
    motor = GetComponent<PlayerMotor>();
    controls = new PlayerControls();
    SetupInputEvents();
  }

  private void OnEnable() {
    controls.Gameplay.Enable();
  }

  private void OnDisable() {
    controls.Gameplay.Disable();
  }

  private void SetupInputEvents() {
    controls.Gameplay.Jump.performed += ctx => { motor.JumpPressed = true; };
    controls.Gameplay.Jump.canceled += ctx => { motor.JumpPressed = false; };
    controls.Gameplay.Move.performed += ctx => { motor.inputHorizontal = ctx.ReadValue<float>(); };
    controls.Gameplay.Move.canceled += ctx => { motor.inputHorizontal = 0.0f; };
  }
}
