using Array2DEditor;
using UnityEngine;

//[CreateAssetMenu(fileName = "Level", menuName = "Assets/Level", order = 1)]
public class Level : ScriptableObject
{
    //[SerializeField]
    //private Array2DBlock map;

    //public Array2DBlock Map { get => map; set => map = value; }

    private int mapSize;
    [SerializeField]
    public BlockType[,] map;

    public BlockType[,] Map { get => map;}
    public int MapSize { get => mapSize; set => mapSize = value; }

    public void SaveMap(BlockType[,] sourceMap)
    {
        map = new BlockType[mapSize, mapSize];
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                map[x, y] = sourceMap[x, y];
            }
        }
    }

    public void DebugMap()
    {
        Debug.Log("Map Size: " + mapSize);
        if (map != null)
        {
            for (int x = 0; x < mapSize; x++)
            {
                for (int y = 0; y < mapSize; y++)
                {
                    Debug.Log("x: " + x + " y: " + y + " type: " + map[x, y]);
                }
            }
        }
        else
        {
            Debug.Log("Map is null");
        }
    }
}
