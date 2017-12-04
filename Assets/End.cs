using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class End : MonoBehaviour {
  public Button[] Buttons;

  public void Start()
  {
    foreach (var button in Buttons)
      button.Listener = this.gameObject;
  }

  public void OnTriggerChanged()
  {
    if (Buttons.All(button => button.IsTriggered))
    {
      SceneManager.LoadScene("end");
    }
  }
}
