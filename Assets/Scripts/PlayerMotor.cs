using System;
using UnityEngine;

public class PlayerMotor : MonoBehaviour {
  #region constants
  private const float DISTANCE_BETWEEN_COLLIDERS = 0.005f;
  #endregion

  #region public variables
  public new BoxCollider2D collider;
  public LayerMask groundLayer;
  public float gravity;
  public float maximumSpeed;
  #endregion

  #region public hidden in inspector variables
  [HideInInspector]
  public float inputHorizontal;
  #endregion

  #region private non-hidden readonly variables
  [ReadOnly]
  [SerializeField]
  private bool isGrounded = false;
  #endregion

  #region private variables
  private Vector2 currentPossibleSpeed = Vector2.zero;
  private float limitY = float.NaN;
  private float limitX = float.NaN;
  #endregion

  #region public functions
  public void Jump() {

  }
  #endregion

  #region unity events
  private void FixedUpdate() {
    isGrounded = IsGrounded();
    currentPossibleSpeed = CalculateSpeed();
    UpdateLimit();
    ApplySpeed();
  }
  #endregion

  #region private functions
  private Vector2 CalculateSpeed() {
    float x = inputHorizontal * maximumSpeed * Time.deltaTime;
    float y = gravity * Time.deltaTime;
    return new Vector2(x, y);
  }

  private bool IsGrounded() {
    RaycastHit2D hit = Physics2D.BoxCast(transform.position, collider.size, 0.0f, Vector2.down, 0.1f, groundLayer);
    return hit.collider != null;
  }

  private void UpdateLimit() {
    limitY = limitX = float.NaN;
    // x limit
    if (Mathf.Abs(currentPossibleSpeed.x) > float.Epsilon) {
      RaycastHit2D xHit = Physics2D.BoxCast(transform.position, collider.size, 0.0f, Mathf.Sign(currentPossibleSpeed.x) > 0 ? Vector2.right : Vector2.left, Mathf.Abs(currentPossibleSpeed.x), groundLayer);
      if (xHit.collider != null) {
        if (Mathf.Sign(currentPossibleSpeed.x) > 0 && xHit.point.x < transform.position.x + collider.size.x / 2.0f + currentPossibleSpeed.x ||
            Mathf.Sign(currentPossibleSpeed.x) < 0 && xHit.point.x > transform.position.x - collider.size.x / 2.0f + currentPossibleSpeed.x) {
          limitX = xHit.point.x + (Mathf.Sign(currentPossibleSpeed.x) > 0 ? -collider.size.x / 2.0f : collider.size.x / 2.0f);
        }
      }
    }
    // y limit
    if (Mathf.Abs(currentPossibleSpeed.y) > float.Epsilon) {
      RaycastHit2D yHit = Physics2D.BoxCast(transform.position, collider.size, 0.0f, Mathf.Sign(currentPossibleSpeed.y) > 0 ? Vector2.up : Vector2.down, Mathf.Abs(currentPossibleSpeed.y), groundLayer);
      if (yHit.collider != null) {
        if (Mathf.Sign(currentPossibleSpeed.y) > 0 && yHit.point.y < transform.position.y + collider.size.y / 2.0f + currentPossibleSpeed.y ||
            Mathf.Sign(currentPossibleSpeed.y) < 0 && yHit.point.y > transform.position.y - collider.size.y / 2.0f + currentPossibleSpeed.y) {
          limitY = yHit.point.y + collider.size.y / 2.0f;
        }
      }
    }
  }

  private void ApplySpeed() {
    float x = (float.IsNaN(limitX) ? transform.position.x + currentPossibleSpeed.x : limitX) + (Mathf.Abs(currentPossibleSpeed.x) > float.Epsilon ? Mathf.Sign(currentPossibleSpeed.x) > 0 ? -DISTANCE_BETWEEN_COLLIDERS : DISTANCE_BETWEEN_COLLIDERS : 0.0f);
    float y = (float.IsNaN(limitY) ? transform.position.y + currentPossibleSpeed.y : limitY) + DISTANCE_BETWEEN_COLLIDERS;
    transform.position = new Vector3(x, y, transform.position.z);
  }
  #endregion
}
