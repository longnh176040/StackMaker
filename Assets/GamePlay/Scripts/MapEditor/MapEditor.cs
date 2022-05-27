using UnityEditor;
using UnityEngine;
using Array2DEditor;
using Sirenix.OdinInspector;

public class MapEditor: MonoBehaviour
{
    #region Create New Map
    [TabGroup("Create New Map")]
    [Header("Prefabs to Instanitiate")]
    [SerializeField] private GameObject WallPrefabs;
    [TabGroup("Create New Map")]
    [SerializeField] private GameObject EdibleBlockPrefabs;
    [TabGroup("Create New Map")]
    [SerializeField] private GameObject InedibleBlockPrefabs;
    [TabGroup("Create New Map")]
    [SerializeField] private GameObject StartPointPrefabs;
    [TabGroup("Create New Map")]
    [SerializeField] private GameObject WinPointPrefabs;

    [TabGroup("Create New Map")]
    [Header("Level")]
    [SerializeField] private int levelNumer;
    private GameObject levelContainer = null;

    [TabGroup("Create New Map")]
    [Space(10)]
    [SerializeField] private Array2DBlock mapCustom;

    [TabGroup("Create New Map")]
    [Button]
    public void LoadLevel()
    {
        if (WallPrefabs == null || EdibleBlockPrefabs == null ||
            InedibleBlockPrefabs == null || StartPointPrefabs == null || WinPointPrefabs == null)
        {
            Debug.LogError("Lack of prefabs.");
            return;
        }

        if (mapCustom == null)
        {
            Debug.LogError("Fill in all the fields in order to generate a map.");
            return;
        }

        //TODO: Check xem đầy đủ vị trí bắt đầu / kết thúc của map chưa => Nếu chưa thì báo lỗi

        var cells = mapCustom.GetCells();
        levelContainer = new GameObject("Level " + levelNumer.ToString());

        for (int y = 0; y < mapCustom.GridSize.y; y++)
        {
            for (int x = 0; x < mapCustom.GridSize.x; x++)
            {
                if (cells[y, x] == BlockType.Empty)
                {
                    continue;
                }
                else
                {
                    CreateBlock(cells[y, x], new Vector3(y, 0, x), levelContainer.transform);
                }
            }
        }
    }

    [TabGroup("Create New Map")]
    [Button]
    public void ResetLevel()
    {
        if (levelContainer == null)
        {
            Debug.LogError("A map should be created first.");
            return;
        }
        else
        {
            DestroyImmediate(levelContainer);
        }
    }

    private void CreateBlock(BlockType type, Vector3 position, Transform parent)
    {
        GameObject objToSpawn = null;
        switch (type)
        {
            case BlockType.Wall:
                objToSpawn = WallPrefabs;
                break;
            case BlockType.Edible:
                objToSpawn = EdibleBlockPrefabs;
                break;
            case BlockType.Inedible:
                objToSpawn = InedibleBlockPrefabs;
                break;
            case BlockType.Start:
                objToSpawn = StartPointPrefabs;
                break;
            case BlockType.Win:
                objToSpawn = WinPointPrefabs;
                break;
        }

        Instantiate(objToSpawn, position, Quaternion.identity, parent);
    }

    #endregion

    #region Edit Map
    [TabGroup("Edit An Existing Map")]
    [ShowInInspector]
    [AssetsOnly]
    [InlineEditor(InlineEditorModes.GUIOnly)]
    private Level levelToEdit;

    [TabGroup("Edit An Existing Map")]
    [Button]
    public void ReloadLevel()
    {

    }

    #endregion

    [ButtonGroup]
    public void SaveLevel()
    {
        if (levelContainer == null)
        {
            Debug.LogError("A map should be created first.");
            return;
        }

        Level level = ScriptableObject.CreateInstance<Level>();
        level.Map = mapCustom;

        string localPath = "Assets/Resources/Levels/" + levelContainer.name + ".asset";
        AssetDatabase.CreateAsset(level, localPath);
        EditorUtility.FocusProjectWindow();
    }

    [ButtonGroup]
    public void DeleteLevel()
    {

    }
}
