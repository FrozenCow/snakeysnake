using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;

public class GridObject : MonoBehaviour {
  public Vector2Int Position
  {
    get
    {
      return new Vector2Int((int)transform.position.x, (int)transform.position.y);
    }
    set
    {
      transform.Translate(value.x - Mathf.Round(transform.position.x), value.y - Mathf.Round(transform.position.y), 0);
    }
  }

  public static bool IsBlocked(Vector2Int position)
  {
    Vector3Int position3 = new Vector3Int(position.x, position.y, 0);
    var tilemaps = GameObject.FindObjectsOfType<Tilemap>();
    var tiles = tilemaps
      .Select(tilemap => tilemap.GetTile<Tile>(position3))
      .Where(tile => tile != null);
    return tiles
      .Any(tile => tile.colliderType != Tile.ColliderType.None);
  }

  public static IEnumerable<GridObject> GetObjects(Vector2Int position)
  {
    var objs = Object.FindObjectsOfType<GridObject>();
    return objs
      .Where(obj => obj.Position.Equals(position));
  }

}
