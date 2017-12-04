using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour {
  private List<Collider2D> triggers = new List<Collider2D>();
  public bool IsTriggered { get { return triggers.Count > 0; } }
  public GameObject Listener { get; set; }

  public void OnTriggerEnter2D(Collider2D collision)
  {
    triggers.Add(collision);
    if (triggers.Count == 1 && Listener != null)
    {
      Listener.SendMessage("OnTriggerChanged");
    }
  }

  public void OnTriggerExit2D(Collider2D collision)
  {
    triggers.Remove(collision);
    if (triggers.Count == 0 && Listener != null)
    {
      Listener.SendMessage("OnTriggerChanged");
    }
  }
}
