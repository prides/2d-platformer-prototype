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
  public float slopeClimbAngle;
  #endregion

  #region public hidden in inspector variables
  [HideInInspector]
  public float inputHorizontal;
  #endregion

  #region private visible in inspector readonly variables
  [ReadOnly]
  [SerializeField]
  private bool isGrounded = false;
  [ReadOnly]
  [SerializeField]
  private bool isJumpPressed = false;
  #endregion

  #region private variables
  #endregion

  #region property
  public bool JumpPressed {
    get { return isJumpPressed; }
    set { isJumpPressed = value; }
  }
  #endregion

  #region public functions
  #endregion

  #region unity events
  private void FixedUpdate() {
    isGrounded = IsGrounded();
    Vector2 speed = CalculateSpeed();
    Debug.DrawRay(transform.position, speed, Color.red);
    CalculateSpeedLimit(ref speed);
    Debug.DrawRay(transform.position, speed, Color.blue);
    ApplySpeed(speed);
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

  private void CalculateSpeedLimit(ref Vector2 speed) {
    // x limit
    if (Mathf.Abs(speed.x) > float.Epsilon) {
      RaycastHit2D xHit = Physics2D.BoxCast(transform.position, collider.size, 0.0f, speed.x > 0 ? Vector2.right : Vector2.left, Mathf.Abs(speed.x), groundLayer);
      if (xHit.collider != null) {
        float collisionAngle = Vector2.Angle(xHit.normal, Vector2.up);
        float rightEdge = transform.position.x + collider.size.x / 2.0f;
        float leftEdge = transform.position.x - collider.size.x / 2.0f;
        if (collisionAngle <= slopeClimbAngle) {
          float remainX = speed.x > 0 ? speed.x - (xHit.point.x - rightEdge) : speed.x - (xHit.point.x - leftEdge);
          float x = Mathf.Abs(remainX) * Mathf.Cos(collisionAngle * Mathf.Deg2Rad) * Mathf.Sign(speed.x);
          float y = Mathf.Abs(remainX) * Mathf.Sin(collisionAngle * Mathf.Deg2Rad);
          if (y > speed.y) {
            speed.x = x + (speed.x - remainX) + (Mathf.Sign(speed.x) * -DISTANCE_BETWEEN_COLLIDERS);
            speed.y = y + DISTANCE_BETWEEN_COLLIDERS;
            // need to additional check for collision
            RaycastHit2D addHit = Physics2D.BoxCast(transform.position, collider.size, 0.0f, speed, Mathf.Abs(speed.magnitude), groundLayer);
            if (addHit.collider != null) {
              float addCollisionAngle = Vector2.Angle(addHit.normal, Vector2.up);
              if (addCollisionAngle > 90.0f) {
                speed.x = speed.y = 0.0f;
                return;
              }
              if (Mathf.Sign(speed.x) > 0 && addHit.point.x < rightEdge + speed.x) {
                speed.x = addHit.point.x - rightEdge - DISTANCE_BETWEEN_COLLIDERS;
              }
              else if (Mathf.Sign(speed.x) < 0 && addHit.point.x > leftEdge + speed.x) {
                speed.x = addHit.point.x - leftEdge + DISTANCE_BETWEEN_COLLIDERS;
              }
              float topEdge = transform.position.y + collider.size.y / 2.0f;
              float bottomEdge = transform.position.y - collider.size.y / 2.0f;
              if (Mathf.Sign(speed.y) > 0 && addHit.point.y < topEdge + speed.y) {
                speed.y = addHit.point.y - topEdge - DISTANCE_BETWEEN_COLLIDERS;
              }
              else if (Mathf.Sign(speed.y) < 0 && addHit.point.y > bottomEdge + speed.y) {
                speed.y = addHit.point.y - bottomEdge + DISTANCE_BETWEEN_COLLIDERS;
              }
            }
          }
        }
        else {
          if (Mathf.Sign(speed.x) > 0 && xHit.point.x < rightEdge + speed.x) {
            speed.x = xHit.point.x - rightEdge - DISTANCE_BETWEEN_COLLIDERS;
          }
          else if (Mathf.Sign(speed.x) < 0 && xHit.point.x > leftEdge + speed.x) {
            speed.x = xHit.point.x - leftEdge + DISTANCE_BETWEEN_COLLIDERS;
          }
        }
      }
    }
    // y limit
    if (Mathf.Abs(speed.y) > float.Epsilon) {
      RaycastHit2D yHit = Physics2D.BoxCast(transform.position, collider.size, 0.0f, Mathf.Sign(speed.y) > 0 ? Vector2.up : Vector2.down, Mathf.Abs(speed.y), groundLayer);
      if (yHit.collider != null) {
        float topEdge = transform.position.y + collider.size.y / 2.0f;
        float bottomEdge = transform.position.y - collider.size.y / 2.0f;
        if (Mathf.Sign(speed.y) > 0 && yHit.point.y < topEdge + speed.y) {
          speed.y = yHit.point.y - topEdge - DISTANCE_BETWEEN_COLLIDERS;
        }
        else if (Mathf.Sign(speed.y) < 0 && yHit.point.y > bottomEdge + speed.y) {
          speed.y = yHit.point.y - bottomEdge + DISTANCE_BETWEEN_COLLIDERS;
        }
      }
    }
  }

  private void ApplySpeed(Vector2 speed) {
    transform.position += new Vector3(speed.x, speed.y, 0.0f);
  }
  #endregion
}
