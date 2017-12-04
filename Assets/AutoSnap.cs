using UnityEngine;

[ExecuteInEditMode]
public class AutoSnap : MonoBehaviour
{

  public float snapValueX;
  public float snapValueY;
  public float snapValueZ;


  void Update()
  {
    if (snapValueX != 0)
      transform.position = new Vector3(Mathf.Round(transform.position.x * (1 / snapValueX)) / (1 / snapValueX), transform.position.y, transform.position.z);

    if (snapValueY != 0)
      transform.position = new Vector3(transform.position.x, Mathf.Round(transform.position.y * (1 / snapValueY)) / (1 / snapValueY), transform.position.z);

    if (snapValueZ != 0)
      transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Round(transform.position.z * (1 / snapValueZ)) / (1 / snapValueZ));
  }
}
