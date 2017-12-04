using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Segment : MonoBehaviour
{
  public Segment next;
  public Segment previous;
  public Segment DefaultGrowSegment;
  public Vector2Int direction = Vector2Int.left;
  public Sprite spriteLeft;
  public Sprite spriteRight;
  public Sprite spriteUp;
  public Sprite spriteDown;

  public GridObject GridObject { get { return GetComponent<GridObject>(); } }
  public Vector2Int Position
  {
    get { return GridObject.Position; }
    set { GridObject.Position = value; }
  }

  public void MoveRelative(Vector2Int relative)
  {
    MoveAbsolute(GridObject.Position + relative);
  }

  public void MoveAbsolute(Vector2Int absolute)
  {
    var oldPosition = GridObject.Position;
    Position = absolute;
    direction = absolute - oldPosition;
    if (previous != null)
      previous.MoveAbsolute(oldPosition);

    UpdateSegment();
  }

  public void Insert(Segment segment)
  {
    if (previous)
      previous.next = segment;
    segment.previous = previous;
    segment.next = this;
    previous = segment;

    UpdateSegment();
  }

  public Segment Grow()
  {
    Segment segment = Instantiate<Segment>(DefaultGrowSegment, transform.parent, false);
    segment.Position = Position;
    segment.direction = direction;
    segment.UpdateSegment();
    Insert(segment);
    return segment;
  }

  public void GrowAndMoveRelative(Vector2Int relative)
  {
    Grow();
    Position += relative;
    direction = relative;
    UpdateSegment();
  }

  public void UpdateSegment()
  {
    var spriteRenderer = GetComponent<SpriteRenderer>();
    spriteRenderer.sprite = direction == Vector2Int.left
      ? spriteLeft
      : direction == Vector2Int.right
      ? spriteRight
      : direction == Vector2Int.up
      ? spriteUp
      : direction == Vector2Int.down
      ? spriteDown
      : spriteLeft;
  }

  public void DetachTail()
  {
    if (previous == null)
      return; // Already detached
    previous.next = null;
    previous.UpdateSegment();

    previous = null;

    UpdateSegment();
  }
  public void Destroy()
  {
    if (previous != null)
      previous.Destroy();
    GameObject.Destroy(this.gameObject);
  }
}
