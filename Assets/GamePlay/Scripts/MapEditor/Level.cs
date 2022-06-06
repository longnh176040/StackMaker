using Array2DEditor;
using UnityEngine;

//[CreateAssetMenu(fileName = "Level", menuName = "Assets/Level", order = 1)]
[System.Serializable]
public class Level : ScriptableObject
{
    [SerializeField]
    private int mapSize;
    [SerializeField]
    private int winPosDirection;

    [SerializeField]
    private Array2DBlock mapEnumBlock;

    public Array2DBlock MapEnumBlock { get => mapEnumBlock; set => mapEnumBlock = value; }

    //[SerializeField]
    //public BlockType[,] map;

    //public BlockType[,] Map { get => map;}
    public int MapSize { get => mapSize; set => mapSize = value; }
    public int WinPosDirection { get => mapSize;}

    //public void DebugMap()
    //{
    //    Debug.Log("Map Size: " + mapSize);
    //    if (map != null)
    //    {
    //        for (int x = 0; x < mapSize; x++)
    //        {
    //            for (int y = 0; y < mapSize; y++)
    //            {
    //                Debug.Log("x: " + x + " y: " + y + " type: " + map[x, y]);
    //            }
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("Map is null");
    //    }
    //}
}
