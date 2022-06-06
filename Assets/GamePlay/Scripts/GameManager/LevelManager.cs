using Array2DEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Level[] levels;

    [SerializeField] private GameObject WallPrefabs;
    [SerializeField] private GameObject EdibleBlockPrefabs;
    [SerializeField] private GameObject InedibleBlockPrefabs;
    [SerializeField] private GameObject StartPointPrefabs;
    [SerializeField] private GameObject WinPointPrefabs;

    [SerializeField] private Transform levelContainer;

    private int winPosDir;

    public void SpawnLevel(Level levelToSpawn)
    {
        winPosDir = levelToSpawn.WinPosDirection;
        var cells = levelToSpawn.MapEnumBlock.GetCells();

        for (int x = 0; x < cells.GetLength(0); x++)
        {
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                CreateBlock(cells[x, y], new Vector3(x, 0, y), levelContainer);
            }
        }
    }

    private void CreateBlock(BlockType type, Vector3 position, Transform parent)
    {
        switch (type)
        {
            case BlockType.Wall:
                Instantiate(WallPrefabs, position, Quaternion.identity, parent);
                break;
            case BlockType.Edible:
                Instantiate(EdibleBlockPrefabs, position, Quaternion.identity, parent);
                break;
            case BlockType.Inedible:
                Instantiate(InedibleBlockPrefabs, position, Quaternion.identity, parent);
                break;
            case BlockType.Start:
                Instantiate(StartPointPrefabs, position, Quaternion.identity, parent);
                break;
            case BlockType.Win:
                GameObject winBlock = Instantiate(WinPointPrefabs, position, Quaternion.identity, parent);
                winBlock.transform.Rotate(Vector3.up * winPosDir);
                break;
        }
    }
}
