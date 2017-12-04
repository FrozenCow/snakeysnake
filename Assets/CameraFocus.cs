using UnityEngine;

public class CameraFocus : MonoBehaviour {
  public Vector2 GetCameraPosition()
  {
    return GetComponent<Collider2D>().bounds.center;
  }
}
