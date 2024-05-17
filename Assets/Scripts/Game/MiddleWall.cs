using UnityEngine;
using UnityEngine.Tilemaps;

public class MiddleWall : MonoBehaviour {
    [SerializeField] TileBase tileSprite;

    private int wallHeight;
    public void SetWallHeight(int wallHeight) {
        Tilemap tilemap = GetComponent<Tilemap>();

        for (int i = this.wallHeight - 1; i > 0; i--) {
            tilemap.SetTile(new Vector3Int(-1, i - 4, 0), null);
        }

        for (int i = 0; i < wallHeight; i++) {
            tilemap.SetTile(new Vector3Int(-1, i-4, 0), tileSprite);
        }

        this.wallHeight = wallHeight;
    }
}
