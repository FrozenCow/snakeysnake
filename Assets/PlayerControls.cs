using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.Analytics;

public class PlayerControls : MonoBehaviour {
  private Segment segment;
  public Image stuckImage;

  public void Start()
  {
    this.segment = GetComponent<Segment>();
  }

  public IEnumerable<Segment> GetSegments()
  {
    var segment = this.segment;
    while (segment != null)
    {
      yield return segment;
      segment = segment.previous;
    }
  }

  bool IsBlockedByWall(Vector2Int position)
  {
    return GridObject.IsBlocked(position);
  }

  bool IsBlockedByObject(Vector2Int position)
  {
    var objects = GridObject.GetObjects(position).ToArray();
    if (objects.Any(o => o.GetComponent<Segment>() != null))
      return true;
    if (objects
      .Select(o => o.GetComponent<Door>())
      .Where(door => door != null)
      .Any(door => !door.IsOpen))
      return true;
    return false;
  }

  bool IsBlocked(Vector2Int position)
  {
    return IsBlockedByWall(position) || IsBlockedByObject(position);
  }

  bool IsStuck()
  {
    var directions = new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
    return directions.All(direction => IsBlocked(segment.Position + direction));
  }

  // Update is called once per frame
  void Update () {
    if (Input.GetKeyDown(KeyCode.R))
    {
      Reset();
      return;
    }

    var stuck = IsStuck();
    if (stuck != stuckImage.enabled)
    {
      if (stuck)
      {
        LogEvent("stuck");
        stuckImage.enabled = true;
        stuckImage.canvasRenderer.SetAlpha(0.0f);
        stuckImage.CrossFadeAlpha(1.0f, 5.0f, false);
      }
      else
      {
        stuckImage.enabled = false;
        stuckImage.canvasRenderer.SetAlpha(0.0f);
      }
    }

    Vector2Int direction = Vector2Int.zero;
		if (Input.GetKeyDown(KeyCode.UpArrow))
      direction = Vector2Int.up;
    if (Input.GetKeyDown(KeyCode.DownArrow))
      direction = Vector2Int.down;
    if (Input.GetKeyDown(KeyCode.LeftArrow))
      direction = Vector2Int.left;
    if (Input.GetKeyDown(KeyCode.RightArrow))
      direction = Vector2Int.right;
    if (direction != Vector2Int.zero)
    {
      var newPosition = segment.Position + direction;
      if (IsBlocked(newPosition))
        return;

      movesSinceCheckpoint++;

      var objects = GridObject.GetObjects(newPosition).ToArray();
      var food = objects.Select(o => o.GetComponent<Food>()).FirstOrDefault(f => f != null);
      if (food != null)
      {
        checkpointObjects.Add(food.gameObject);
        food.gameObject.SetActive(false);
        segment.GrowAndMoveRelative(direction);
        return;
      }
      var moveAwayFromDoor = GridObject.GetObjects(segment.Position).Select(o => o.GetComponent<Door>()).Where(o => o != null).FirstOrDefault();
      if (moveAwayFromDoor != null)
        Detach();
      segment.MoveRelative(direction);
      if (moveAwayFromDoor != null)
        Checkpoint(moveAwayFromDoor);
    }

    if (Input.GetKeyDown(KeyCode.A))
      segment.Grow();
	}

  public void Detach()
  {
    segment.DetachTail();
  }

  private string checkpointName = "";
  private List<GameObject> checkpointObjects = new List<GameObject>();
  private Vector2Int checkpointPosition;
  private int movesSinceCheckpoint = 0;
  private int resetsSinceCheckpoint = 0;
  public void Checkpoint(Door door)
  {
    LogEvent("checkpoint");

    checkpointName = door.transform.parent.name;
    checkpointPosition = segment.Position;
    checkpointObjects.Clear();
    movesSinceCheckpoint = 0;
    resetsSinceCheckpoint = 0;
  }
  
  public void Reset()
  {
    movesSinceCheckpoint = 0;
    resetsSinceCheckpoint++;
    LogEvent("reset");

    foreach (var obj in checkpointObjects)
      obj.SetActive(true);

    var tail = segment.previous;
    this.segment.DetachTail();
    if (tail != null)
      tail.Destroy();
    this.segment.Position = checkpointPosition;
  }

  public void LogEvent(string eventName)
  {
    Analytics.CustomEvent(eventName, new Dictionary<string, object>() { { "area", checkpointName }, { "moves", movesSinceCheckpoint }, { "resets", resetsSinceCheckpoint } });
  }

  private List<Area> areas = new List<Area>();

  public void OnTriggerEnter2D(Collider2D collision)
  {
    var area = collision.GetComponent<Area>();
    if (area)
      areas.Add(area);
  }

  public void OnTriggerExit2D(Collider2D collision)
  {
    var area = collision.GetComponent<Area>();
    if (area)
      areas.Remove(area);
  }

  //private void OnTriggerEnter2D(Collider2D collision)
  //{
  //  var food = collision.GetComponent<Food>();
  //  if (food == null)
  //    return;
  //  segment.Grow();
  //  GameObject.Destroy(food.gameObject);
  //}
}
