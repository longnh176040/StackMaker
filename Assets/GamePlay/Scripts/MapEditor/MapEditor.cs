using UnityEditor;
using UnityEngine;
using Array2DEditor;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;

public class MapEditor: MonoBehaviour
{
    #region MAP_PARAMS
    private const float TileSize = 20;

    private const int mapSizeMin = 10;
    private const int mapSizeMax = 20;
    private int currentMapSize = -1;
    private bool firstEdit = true;

    private BlockType[,] editedTiles;

    private BlockType currentBlockType = BlockType.Empty;

    private static readonly Color[] NumberColors = new Color[6]
    {
            new Color32(236, 212, 212, 200),		// Empty Block
			new Color32(150, 146, 146, 200),		// Wall Block
			new Color32(255, 242, 0, 200),		// Edible Block
			new Color32(251, 250, 153, 200),		// Inedible Block
			new Color32(0, 255, 0, 200),		// Start Block
			new Color32(255, 0, 0, 200),		// Win Block
    };

    #endregion

    #region Create New Map
    [FoldoutGroup("Map Editor/Create New Map/Prefabs to Instanitiate")]
    [SerializeField] private GameObject WallPrefabs;
    [FoldoutGroup("Map Editor/Create New Map/Prefabs to Instanitiate")]
    [SerializeField] private GameObject EdibleBlockPrefabs;
    [FoldoutGroup("Map Editor/Create New Map/Prefabs to Instanitiate")]
    [SerializeField] private GameObject InedibleBlockPrefabs;
    [FoldoutGroup("Map Editor/Create New Map/Prefabs to Instanitiate")]
    [SerializeField] private GameObject StartPointPrefabs;
    [FoldoutGroup("Map Editor/Create New Map/Prefabs to Instanitiate")]
    [SerializeField] private GameObject WinPointPrefabs;

    [TabGroup("Map Editor", "Create New Map")]
    [Header("Level")]
    [SerializeField] private int levelNumer;
    private GameObject levelContainer = null;

    //=====================MAP CUSTOM=======================
    [Header("Map Custom")]
    [TabGroup("Map Editor", "Create New Map")]
    [CustomValueDrawer("CustomMapSizeRange")]
    public int CustomMapSize;

    [TabGroup("Map Editor", "Create New Map")]
    [ShowInInspector] [ReadOnly] [Space(10)]
    private static Color CurrentBlock = NumberColors[0];

    [ButtonGroup("Map Editor/Create New Map/BlockColor")]
    [GUIColor(0.925f, 0.83f, 0.83f)]
    private void Empty()
    {
        CurrentBlock = NumberColors[0];
        currentBlockType = BlockType.Empty;
    }

    [ButtonGroup("Map Editor/Create New Map/BlockColor")]
    [GUIColor(0.59f, 0.57f, 0.57f)]
    private void Wall()
    {
        CurrentBlock = NumberColors[1];
        currentBlockType = BlockType.Wall;
    }

    [ButtonGroup("Map Editor/Create New Map/BlockColor")]
    [GUIColor(1, 0.95f, 0)]
    private void Edible()
    {
        CurrentBlock = NumberColors[2];
        currentBlockType = BlockType.Edible;
    }

    [ButtonGroup("Map Editor/Create New Map/BlockColor")]
    [GUIColor(0.984f, 0.98f, 0.6f)]
    private void Inedible()
    {
        CurrentBlock = NumberColors[3];
        currentBlockType = BlockType.Inedible;
    }

    [ButtonGroup("Map Editor/Create New Map/BlockColor")]
    [GUIColor(0, 1, 0)]
    private void Begin()
    {
        CurrentBlock = NumberColors[4];
        currentBlockType = BlockType.Start;
    }

    [ButtonGroup("Map Editor/Create New Map/BlockColor")]
    [GUIColor(1, 0, 0)]
    private void Win()
    {
        CurrentBlock = NumberColors[5];
        currentBlockType = BlockType.Win;
    }

    //[TabGroup("Map Editor", "Create New Map")]
    //[Space(10)]
    //[SerializeField] private Array2DBlock mapCustom;

    //============================================


    [ButtonGroup("Map Editor/Create New Map/FunctionButton")]
    public void LoadLevel()
    {
        if (WallPrefabs == null || EdibleBlockPrefabs == null ||
            InedibleBlockPrefabs == null || StartPointPrefabs == null || WinPointPrefabs == null)
        {
            Debug.LogError("Lack of prefabs.");
            return;
        }

        if (editedTiles == null)
        {
            Debug.LogError("Fill in all the fields in order to generate a map.");
            return;
        }

        //TODO: Check xem đầy đủ vị trí bắt đầu / kết thúc của map chưa => Nếu chưa thì báo lỗi
        levelContainer = new GameObject("Level " + levelNumer.ToString());

        //var cells = mapCustom.GetCells();

        //for (int y = 0; y < mapCustom.GridSize.y; y++)
        //{
        //    for (int x = 0; x < mapCustom.GridSize.x; x++)
        //    {
        //        if (cells[y, x] == BlockType.Empty)
        //        {
        //            continue;
        //        }
        //        else
        //        {
        //            CreateBlock(cells[y, x], new Vector3(y, 0, x), levelContainer.transform);
        //        }
        //    }
        //}

        for (int x = 0; x < currentMapSize; x++)
        {
            for (int y = 0; y < currentMapSize; y++)
            {
                if (editedTiles[y, x] == BlockType.Empty)
				{
					continue;
				}
				else
				{
					CreateBlock(editedTiles[y, x], new Vector3(y, 0, x), levelContainer.transform);
				}
			}
        }
    }

    [ButtonGroup("Map Editor/Create New Map/FunctionButton")]
    [Button]
    public void ResetLevel()
    {
        if (levelContainer != null)
        {
            DestroyImmediate(levelContainer);
        }
        InitMap(currentMapSize);
        currentBlockType = BlockType.Empty;
        CurrentBlock = NumberColors[0];
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
    
    private void InitMap(int value)
	{
        editedTiles = new BlockType[value, value];

        for (int x = 0; x < value; x++)
        {
            for (int y = 0; y < value; y++)
            { 
                editedTiles[x, y] = BlockType.Empty;
            }
        }
    }

    private void ShowMapEditor()
	{
        Rect rect = EditorGUILayout.GetControlRect(true, TileSize * currentMapSize);
        rect = rect.AlignCenter(TileSize * currentMapSize);

        rect = rect.AlignBottom(rect.height);
        SirenixEditorGUI.DrawSolidRect(rect, NumberColors[0]);

        for (int i = 0; i < currentMapSize * currentMapSize; i++)
        {
            Rect tileRect = rect.SplitGrid(TileSize, TileSize, i);
            SirenixEditorGUI.DrawBorders(tileRect.SetWidth(tileRect.width + 1).SetHeight(tileRect.height + 1), 1);

            int x = i % currentMapSize;
            int y = i / currentMapSize;

            BlockType edited = BlockType.Empty;
            if (editedTiles != null) edited = editedTiles[x, y];

            //Coloring all edited block
            if (edited == BlockType.Wall)
			{
                SirenixEditorGUI.DrawSolidRect(new Rect(tileRect.x + 1, tileRect.y + 1, tileRect.width - 1, tileRect.height - 1), NumberColors[1]);
            }

            if (edited == BlockType.Edible)
            {
                SirenixEditorGUI.DrawSolidRect(new Rect(tileRect.x + 1, tileRect.y + 1, tileRect.width - 1, tileRect.height - 1), NumberColors[2]);
            }

            if (edited == BlockType.Inedible)
            {
                SirenixEditorGUI.DrawSolidRect(new Rect(tileRect.x + 1, tileRect.y + 1, tileRect.width - 1, tileRect.height - 1), NumberColors[3]);
            }

            if (edited == BlockType.Start)
            {
                SirenixEditorGUI.DrawSolidRect(new Rect(tileRect.x + 1, tileRect.y + 1, tileRect.width - 1, tileRect.height - 1), NumberColors[4]);
            }

            if (edited == BlockType.Win)
            {
                SirenixEditorGUI.DrawSolidRect(new Rect(tileRect.x + 1, tileRect.y + 1, tileRect.width - 1, tileRect.height - 1), NumberColors[5]);
            }

            //Color unedited block
            if (tileRect.Contains(Event.current.mousePosition))
			{
                SirenixEditorGUI.DrawSolidRect(new Rect(tileRect.x + 1, tileRect.y + 1, tileRect.width - 1, tileRect.height - 1), GetColorBlock(x, y));

                if ((Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag) && Event.current.button == 0)
                {
                    if (editedTiles[x,y] != currentBlockType)
					{
                        editedTiles[x, y] = currentBlockType;
					}
                    Event.current.Use();
                }
            }
        }

        GUIHelper.RequestRepaint();
    }

    private float CustomMapSizeRange(int value, GUIContent label)
	{
        var size = EditorGUILayout.IntSlider(label, value, mapSizeMin, mapSizeMax);

        if (firstEdit)
		{
            firstEdit = false; 
            currentMapSize = size;
            InitMap(size);
            ShowMapEditor(); 
        }
        else
		{
            if (currentMapSize != size)
            {
                currentMapSize = size;
                InitMap(size);
                ShowMapEditor(); 
            }
            else
            {
                ShowMapEditor(); 
            }
        }
        return size;
	}

    private Color GetColorBlock(int x, int y)
	{
        BlockType clickBlock = BlockType.Empty;
        
        if(editedTiles != null) clickBlock = editedTiles[x, y]; 

        Color curblockColor = NumberColors[0];
        switch (clickBlock)
		{
            case BlockType.Empty:
                curblockColor = NumberColors[0];
                break;
            case BlockType.Wall:
                curblockColor = NumberColors[1];
                break;
            case BlockType.Edible:
                curblockColor = NumberColors[2];
                break;
            case BlockType.Inedible:
                curblockColor = NumberColors[3];
                break;
            case BlockType.Start:
                curblockColor = NumberColors[4];
                break;
            case BlockType.Win:
                curblockColor = NumberColors[5];
                break;
        }

        curblockColor.a = 255;
        return curblockColor;
	}

    #endregion

    #region Edit Map
    [TabGroup("Map Editor", "Edit An Existing Map")]
    [ShowInInspector]
    [AssetsOnly]
    [InlineEditor(InlineEditorModes.GUIOnly)]
    private Level levelToEdit;

    [TabGroup("Map Editor", "Edit An Existing Map")]
    [Button]
    public void ReloadLevel()
    {

    }

    #endregion

    [ButtonGroup("SameBtnGroup")]
    public void SaveLevel()
    {
        if (levelContainer == null)
        {
            Debug.LogError("A map should be created first.");
            return;
        }

        Level level = ScriptableObject.CreateInstance<Level>();
        //level.Map = mapCustom;

        string localPath = "Assets/Resources/Levels/" + levelContainer.name + ".asset";
        AssetDatabase.CreateAsset(level, localPath);
        EditorUtility.FocusProjectWindow();
    }

    [ButtonGroup("SameBtnGroup")]
    public void DeleteLevel()
    {

    }
}
