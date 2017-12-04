using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class CameraController : MonoBehaviour {
  public new Camera camera;
  public Vector2 GetFocusPoint()
  {
    if (focus.Count == 0)
      return transform.position;
    var sum = focus.Aggregate(Vector2.zero, (position, cameraFocus) => position + cameraFocus.GetCameraPosition());
    return sum / focus.Count;
  }

  public void Update()
  {
    var maxspeed = 10;
    var target = GetFocusPoint();
    var current = new Vector2(camera.transform.position.x, camera.transform.position.y);
    var diff = target - current;
    var direction = diff.normalized;
    var distance = diff.magnitude;
    var maxtravel = maxspeed * Time.deltaTime;
    var travel = Mathf.Min(maxtravel, distance);
    var newposition = current + direction * travel;
    camera.transform.position = new Vector3(newposition.x, newposition.y, camera.transform.position.z);
  }

  private List<CameraFocus> focus = new List<CameraFocus>();

  public void OnTriggerEnter2D(Collider2D collision)
  {
    var cameraFocus = collision.GetComponent<CameraFocus>();
    if (cameraFocus)
      focus.Add(cameraFocus);
  }

  public void OnTriggerExit2D(Collider2D collision)
  {
    var cameraFocus = collision.GetComponent<CameraFocus>();
    if (cameraFocus)
      focus.Remove(cameraFocus);
  }
}
