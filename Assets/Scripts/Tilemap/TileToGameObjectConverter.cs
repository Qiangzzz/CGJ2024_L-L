using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileToGameObjectConverter : MonoBehaviour
{
    public Tilemap tilemap; // ��Ҫ�ֶ���Tilemap������ק�����ֶ�
    public GameObject tilePrefab; // ����������Ƭ��Ԥ�Ƽ�
    public Transform parentTransform; // ָ���ĸ���GameObject

    private List<GameObject> tileGameObjects = new List<GameObject>(); // �洢���ɵ�GameObject
    [Button("����level�ؿ�")]
    public void ReGenerateGridGO()
    {
        ConvertTilesToGameObjects();
    }
    void Start()
    {
        ConvertTilesToGameObjects();
    }

    void ConvertTilesToGameObjects()
    {
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                Vector3Int localPlace = new Vector3Int(x + bounds.xMin, y + bounds.yMin, 0);
                TileBase tile = tilemap.GetTile(localPlace);
                if (tile != null)
                {
                    CreateTileGameObject(localPlace, tile);
                }
            }
        }
    }

    void CreateTileGameObject(Vector3Int position, TileBase tile)
    {
        Vector3 worldPosition = tilemap.CellToWorld(position);
        GameObject tileGO = Instantiate(tilePrefab, worldPosition, Quaternion.identity, parentTransform);
        tileGO.name = "Tile_" + position.x + "_" + position.y;
        tileGameObjects.Add(tileGO); // �����ɵ�GameObject��ӵ��б���
        SpriteRenderer spriteRenderer = tileGO.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Tile tileData = tile as Tile;
            if (tileData != null)
            {
                spriteRenderer.sprite = tileData.sprite;
            }
        }
    }

    // ���������������������������ɵ�GameObject�������������µ�
    public void ClearGeneratedObjects()
    {
        foreach (GameObject obj in tileGameObjects)
        {
            if (obj != null)
            {
                DestroyImmediate(obj); // �������ɵ�GameObject
            }
        }
        tileGameObjects.Clear(); // ����б�
    }
}
