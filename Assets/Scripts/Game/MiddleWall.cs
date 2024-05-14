using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MiddleWall : MonoBehaviour {
    [SerializeField] TileBase tileSprite;
    public void SetWallHeight(int wallHeight) {
        Tilemap tilemap = GetComponent<Tilemap>();

        for (int i = 0; i < wallHeight; i++) {
            tilemap.SetTile(new Vector3Int(-1, i-4, 0), tileSprite);
        }
    }
}
