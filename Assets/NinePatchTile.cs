using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Tilemaps;

[Flags]
public enum BorderType
{
  None = 0,
  Top = 1,
  Left = 2,
  Right = 4,
  Bottom = 8
}

public static class TileTypeExtensions
{
  public static bool HasFlag(this BorderType tileType, BorderType flag)
  {
    return (tileType & flag) != 0;
  }

  public static bool HasTop(this BorderType tileType)
  {
    return tileType.HasFlag(BorderType.Top);
  }
  public static bool HasBottom(this BorderType tileType)
  {
    return tileType.HasFlag(BorderType.Bottom);
  }
  public static bool HasLeft(this BorderType tileType)
  {
    return tileType.HasFlag(BorderType.Left);
  }
  public static bool HasRight(this BorderType tileType)
  {
    return tileType.HasFlag(BorderType.Right);
  }

}

public class NinePatchRects
{
  public int x;
  public int y;
  public int width;
  public int height;
  public int borderLeft;
  public int borderRight;
  public int borderTop;
  public int borderBottom;
  public RectInt full;
  public RectInt center;
  public RectInt top;
  public RectInt bottom;
  public RectInt left;
  public RectInt right;
  public RectInt topLeft;
  public RectInt topRight;
  public RectInt bottomLeft;
  public RectInt bottomRight;

  public NinePatchRects(int x, int y, int width, int height, int borderLeft, int borderRight, int borderTop, int borderBottom)
  {
    this.x = x;
    this.y = y;
    this.width = width;
    this.height = height;
    this.borderLeft = borderLeft;
    this.borderRight = borderRight;
    this.borderTop = borderTop;
    this.borderBottom = borderBottom;
    full = new RectInt(x, y, width, height);
    center = new RectInt(
      x + borderLeft,
      y + borderTop,
      width - borderLeft - borderRight,
      height - borderTop - borderBottom
    );
    top = new RectInt(
      x + borderLeft,
      y,
      center.width,
      borderTop
      );

    bottom = new RectInt(
      x + borderLeft,
      y + height - borderBottom,
      center.width,
      borderBottom
      );

    left = new RectInt(
      x,
      y + borderTop,
      borderLeft,
      center.height
      );

    right = new RectInt(
      x + width - borderRight,
      y + borderTop,
      borderRight,
      center.height
      );

    bottomLeft = new RectInt(
      x,
      y,
      borderLeft,
      borderTop
      );

    bottomRight = new RectInt(
      x + width - borderRight,
      y,
      borderRight,
      borderTop
      );

    topLeft = new RectInt(
      x,
      y + height - borderBottom,
      borderLeft,
      borderBottom
      );

    topRight = new RectInt(
      x + width - borderRight,
      y + height - borderBottom,
      borderRight,
      borderBottom
      );
  }
}

public class NinePatchTile : Tile {
  public Sprite[] sprites;
  private Sprite sourceSprite = null;

  public void Blit(Texture2D sourceTexture, RectInt sourceRect, Texture2D targetTexture, RectInt targetRect)
  {
    for(var relativeTargetX=0;relativeTargetX<targetRect.width;relativeTargetX++)
      for(var relativeTargetY=0;relativeTargetY<targetRect.height;relativeTargetY++)
      {
        var sourceX = sourceRect.x + (relativeTargetX * sourceRect.width) / targetRect.width;
        var sourceY = sourceRect.y + (relativeTargetY * sourceRect.height) / targetRect.height;
        var targetX = targetRect.x + relativeTargetX;
        var targetY = targetRect.y + relativeTargetY;
        var color = sourceTexture.GetPixel(sourceX, sourceY);
        targetTexture.SetPixel(targetX, targetY, color);
      }
  }

  RectInt RectToRectInt(Rect rect)
  {
    return new RectInt(
      (int)rect.xMin, (int)rect.yMin, (int)rect.width, (int)rect.height
    );
  }

  public void UpdateNinePatch()
  {
    var spriteCount = 2 * 2 * 2 * 2;
    if (this.sourceSprite == this.sprite)
      return;
    var sourceSprite = this.sprite;
    sprites = new Sprite[spriteCount];
    if (sourceSprite == null)
    {
      this.sourceSprite = null;
      return;
    }

    var sourceTexture = sourceSprite.texture;

    var sourceRects = new NinePatchRects(
      (int)(sourceSprite.rect.x),
      (int)(sourceSprite.rect.y),
      (int)(sourceSprite.rect.width),
      (int)(sourceSprite.rect.height),
      (int)(sourceSprite.border.x),
      (int)(sourceSprite.border.z),
      (int)(sourceSprite.border.w),
      (int)(sourceSprite.border.y)
    );

    var targetRects = sourceRects;

    var bottom = sourceRects.bottom;
    bottom.xMin = sourceRects.full.xMin;
    bottom.xMax = sourceRects.full.xMax;

    var top = sourceRects.top;
    top.xMin = sourceRects.full.xMin;
    top.xMax = sourceRects.full.xMax;

    var left = sourceRects.left;
    left.yMin = sourceRects.full.yMin;
    left.yMax = sourceRects.full.yMax;

    var right = sourceRects.right;
    right.yMin = sourceRects.full.yMin;
    right.yMax = sourceRects.full.yMax;

    for (var index = 0; index < spriteCount; index++)
    {
      if (sprites[index] != null)
        continue;
      var tileType = (BorderType)index;

      var targetTexture = new Texture2D(sourceTexture.width, sourceTexture.height, TextureFormat.ARGB32, false);

      //Blit(sourceTexture, sourceRects.center, targetTexture, sourceRects.full);

      //if (tileType.HasTop())
      //  Blit(sourceTexture, sourceRects.top, targetTexture, top);
      //if (tileType.HasBottom())
      //  Blit(sourceTexture, sourceRects.bottom, targetTexture, bottom);
      //if (tileType.HasRight())
      //  Blit(sourceTexture, sourceRects.right, targetTexture, right);
      //if (tileType.HasLeft())
      //  Blit(sourceTexture, sourceRects.left, targetTexture, left);

      if (tileType.HasTop() && tileType.HasRight())
        Blit(sourceTexture, sourceRects.topRight, targetTexture, targetRects.topRight);
      if (!tileType.HasTop() && tileType.HasLeft())
        Blit(sourceTexture, sourceRects.topLeft, targetTexture, targetRects.topLeft);
      if (tileType.HasBottom() && tileType.HasRight())
        Blit(sourceTexture, sourceRects.bottomRight, targetTexture, targetRects.bottomRight);
      if (tileType.HasBottom() && tileType.HasLeft())
        Blit(sourceTexture, sourceRects.bottomLeft, targetTexture, targetRects.bottomLeft);

      targetTexture.Apply();

      var targetSprite = Sprite.Create(targetTexture, sourceSprite.rect, new Vector2(0.5f, 0.5f), sourceSprite.pixelsPerUnit);

      sprites[index] = targetSprite;
    }
    this.sourceSprite = sourceSprite;
  }

  public bool HasMatchingTile(ITilemap tilemap, Vector3Int position)
  {
    return tilemap.GetTile(position) == this;
  }

  public override void RefreshTile(Vector3Int location, ITilemap tilemap)
  {
    for (int yd = -1; yd <= 1; yd++)
      for (int xd = -1; xd <= 1; xd++)
      {
        Vector3Int position = new Vector3Int(location.x + xd, location.y + yd, location.z);
        if (HasMatchingTile(tilemap, position))
          tilemap.RefreshTile(position);
      }
  }

  public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
  {
    //UpdateNinePatch();

    bool top = HasMatchingTile(tilemap, position + Vector3Int.up);
    bool bottom = HasMatchingTile(tilemap, position + Vector3Int.down);
    bool left = HasMatchingTile(tilemap, position + Vector3Int.left);
    bool right = HasMatchingTile(tilemap, position + Vector3Int.right);

    BorderType borderType = BorderType.None;
    if (!top) borderType |= BorderType.Top;
    if (!bottom) borderType |= BorderType.Bottom;
    if (!left) borderType |= BorderType.Left;
    if (!right) borderType |= BorderType.Right;

    int index = (int)borderType;

    var sprite = sprites[index];

    tileData.sprite = sprite;
  }

#if UNITY_EDITOR
  // The following is a helper that adds a menu item to create a RoadTile Asset
  [MenuItem("Assets/Create/NinePatchTile")]
  public static void CreateRoadTile()
  {
    string path = EditorUtility.SaveFilePanelInProject("Save NinePatchTile", "New NinePatchTile", "Asset", "Save NinePatchTile", "Assets");
    if (path == "")
      return;
    AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<NinePatchTile>(), path);
  }
#endif
}


public class AlternatingTile: Tile
{
  public Sprite[] Sprites;
  public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
  {
    int n = position.x + position.y + position.z;
    int index = Math.Abs(n) % Sprites.Length;
    var sprite = Sprites[index];
    tileData.sprite = sprite;
  }

#if UNITY_EDITOR
  // The following is a helper that adds a menu item to create a RoadTile Asset
  [MenuItem("Assets/Create/AlternatingTile")]
  public static void CreateAlternatingTile()
  {
    string path = EditorUtility.SaveFilePanelInProject("Save AlternatingTile", "New AlternatingTile", "Asset", "Save AlternatingTile", "Assets");
    if (path == "")
      return;
    AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<AlternatingTile>(), path);
  }
#endif
}