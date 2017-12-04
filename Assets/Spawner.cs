using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine.Tilemaps;

public class Spawner : MonoBehaviour {
  public Food currentFood;
  public Food defaultFood;

	void Update () {
    if (!currentFood)
    {
      var position = RandomPosition();
      currentFood = Instantiate<Food>(defaultFood, this.transform.parent, false);
      currentFood.transform.localPosition = new Vector3(position.x, position.y, 0);
    }
	}

  Vector2Int RandomPosition()
  {
    return new Vector2Int(Random.Range(1, 5), Random.Range(1, 5));
  }
}
