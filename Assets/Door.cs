using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Door : MonoBehaviour {

  public Button[] Buttons;
  public Sprite closed;
  public Sprite opened;

  private List<Collider2D> colliders = new List<Collider2D>();

  public bool IsOpen
  {
    get { return colliders.Count > 0 || Buttons.All(button => button.IsTriggered); }
  }

	void Start () {
    foreach (var button in Buttons)
      button.Listener = gameObject;
    OnTriggerChanged();
  }

  public void OnTriggerChanged()
  {
    GetComponent<SpriteRenderer>().sprite = IsOpen ? opened : closed;
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    colliders.Add(collision);
    OnTriggerChanged();
  }

  private void OnTriggerExit2D(Collider2D collision)
  {
    colliders.Remove(collision);
    OnTriggerChanged();
  }
}
