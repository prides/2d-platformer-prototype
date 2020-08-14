using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour {
  private const float INPUT_THRESHOLD = 0.2f;

  private PlayerMotor motor;

  private void Awake() {
    motor = GetComponent<PlayerMotor>();
  }

  private void Update() {
    if (Mathf.Abs(Input.GetAxis("Horizontal")) > INPUT_THRESHOLD) {
      motor.inputHorizontal = Input.GetAxis("Horizontal");
    }
    else {
      motor.inputHorizontal = 0.0f;
    }
  }
}
