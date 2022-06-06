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

    private Vector2 startPos, winPos;

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

    //[TabGroup("Map Editor", "Create New Map")]
    [HorizontalGroup("Map Editor/Create New Map/Pickable")]
    [VerticalGroup("Map Editor/Create New Map/Pickable/Left")]
    [ShowInInspector] [ReadOnly] [Space(10)]
    private static Color CurrentBlock = NumberColors[0];
    [VerticalGroup("Map Editor/Create New Map/Pickable/Right")]
    [Button] [PropertySpace(10)]
    public void Restart()
    {
        for (int x = 0; x < currentMapSize; x++)
        {
            for (int y = 0; y < currentMapSize; y++)
            {
                editedTiles[x, y] = BlockType.Empty;
            }
        }
    }

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
        if (levelContainer != null)
        {
            DestroyImmediate(levelContainer);
        }
        levelContainer = new GameObject("Level " + levelNumer.ToString());

        for (int x = 0; x < currentMapSize; x++)
        {
            for (int y = 0; y < currentMapSize; y++)
            {
                if (editedTiles[x, y] == BlockType.Empty)
				{
					continue;
				}
				else
				{
					CreateBlock(editedTiles[x, y], new Vector3(x, 0, y), levelContainer.transform);
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

        GameObject spawnedObj = Instantiate(objToSpawn, position, Quaternion.identity, parent);
        if (type == BlockType.Win)
        {
            Vector2 adjacentBlock = FindSurroundedBlock();
            if (adjacentBlock.x == winPos.x - 1)
            {
                spawnedObj.transform.Rotate(Vector3.up * 90f);
            }
            else if (adjacentBlock.x == winPos.x + 1)
            {
                spawnedObj.transform.Rotate(Vector3.up * -90f);
            }
            else if (adjacentBlock.y == winPos.y + 1)
            {
                spawnedObj.transform.Rotate(Vector3.up * -180f);
            }
        }
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
        startPos = winPos = Vector2.one * -1;
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
                    if (editedTiles[x, y] != currentBlockType)
					{
                        editedTiles[x, y] = currentBlockType;

                        if (currentBlockType == BlockType.Start)
                        {
                            if (startPos == Vector2.one * -1)
                            {
                                startPos.x = x; startPos.y = y;
                            }
                            else
                            {
                                editedTiles[(int)startPos.x, (int)startPos.y] = BlockType.Empty;
                                startPos.x = x; startPos.y = y;
                            }
                        }

                        if (currentBlockType == BlockType.Win)
                        {
                            if (winPos == Vector2.one * -1)
                            {
                                winPos.x = x; winPos.y = y;
                            }
                            else
                            {
                                editedTiles[(int) winPos.x, (int) winPos.y] = BlockType.Empty;
                                startPos.x = x; startPos.y = y;
                            }
                        }
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

    #endregion

    #region Edit Map
    private bool isEditingMap = false;

    [TabGroup("Map Editor", "Edit An Existing Map")]
    [ShowInInspector]
    [AssetsOnly]
    [InlineEditor(InlineEditorModes.GUIOnly)] [InlineButton("StartEditMap")]
    private Level levelToEdit;

    [Header("Map Custom")] [ShowIf("isEditingMap")]
    [TabGroup("Map Editor", "Edit An Existing Map")]
    [CustomValueDrawer("CustomEditMapSizeRange")]
    public int EditMapSize;

    private int winPosDir;

    [TabGroup("Map Editor", "Edit An Existing Map")]
    [Button]
    public void ReloadLevel()
    {
        if (levelContainer != null)
        {
            DestroyImmediate(levelContainer);
        }
        levelContainer = new GameObject(levelToEdit.name);

        for (int x = 0; x < currentMapSize; x++)
        {
            for (int y = 0; y < currentMapSize; y++)
            {
                if (editedTiles[x, y] == BlockType.Empty)
                {
                    continue;
                }
                else
                {
                    LoadBlock(editedTiles[x, y], new Vector3(x, 0, y), levelContainer.transform);
                }
            }
        }
    }

    private void StartEditMap()
    {
        if (levelToEdit == null)
        {
            isEditingMap = false;
            Debug.LogError("Choose a level to edit !!!");
            return;
        }
        else
        {
            //levelToEdit.DebugMap();
            //Debug.Log(levelToEdit.MapSize);
            EditMapSize = levelToEdit.MapSize;
            currentMapSize = EditMapSize;
            winPosDir = levelToEdit.WinPosDirection;

            //CopyMap(levelToEdit.Map, editedTiles, EditMapSize);
            editedTiles = ConvertArray2DBlockToEnumArray(levelToEdit.MapEnumBlock);
            startPos = FindBlockPosition(BlockType.Start);
            winPos = FindBlockPosition(BlockType.Win);

            isEditingMap = true;
        }
    }

    private float CustomEditMapSizeRange(int value, GUIContent label)
    {
        int size = EditorGUILayout.IntSlider(label, value, mapSizeMin, mapSizeMax);

        if (isEditingMap)
        {
            if (currentMapSize != size)
            {
                InitMap(size);
                currentMapSize = size;
                ShowMapEditor();
            }
            else
            {
                ShowMapEditor();
            }
        }
        
        return size;
    }

    private void LoadBlock(BlockType type, Vector3 position, Transform parent)
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

    #endregion

    #region Shared Stuff
    [ButtonGroup("SameBtnGroup")]
    public void SaveLevel()
    {
        if (levelContainer == null)
        {
            Debug.LogError("A map should be created first.");
            return;
        }

        Level level = ScriptableObject.CreateInstance<Level>();
        level.MapSize = currentMapSize;
        
        Array2DBlock ar2B = ConvertEnumToArray2DBlock(editedTiles);
        //var cells = ar2B.GetCells();
        //for(int i = 0; i < cells.GetLength(0); i++)
        //{
        //    for (int j = 0; j < cells.GetLength(1); j++)
        //    {
        //        Debug.Log(i + " " + j + " " + cells[i, j] + " " + editedTiles[i, j]);
        //    }
        //}
        level.MapEnumBlock = ar2B;

        string localPath = "Assets/Resources/Levels/" + levelContainer.name + ".asset";
        AssetDatabase.CreateAsset(level, localPath);
    }

    [ButtonGroup("SameBtnGroup")]
    public void DeleteLevel()
    {
        if (levelContainer == null)
        {
            Debug.LogError("A map should be created first.");
            return;
        }

        string localPath = "Assets/Resources/Levels/" + levelContainer.name + ".asset";
        AssetDatabase.DeleteAsset(localPath);
    }

    private Color GetColorBlock(int x, int y)
    {
        BlockType clickBlock = BlockType.Empty;

        if (editedTiles != null) clickBlock = editedTiles[x, y];

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

    private Vector2 FindBlockPosition(BlockType blockToFind)
    {
        Vector2 blockPos = -Vector2.one;
        if (editedTiles != null)
        {
            for (int x = 0; x < currentMapSize; x++)
            {
                for (int y = 0; y < currentMapSize; y++)
                {
                    if (editedTiles[x, y] == blockToFind)
                    {
                        blockPos = new Vector2(x, y);
                        break;
                    }
                }
            }
        }
        return blockPos;
    }

    private Vector2 FindSurroundedBlock()
    {
        Vector2 blockPos = -Vector2.one;
        if (winPos != Vector2.one * -1)
        {
            int winPosX = (int)winPos.x;
            int winPosY = (int)winPos.y;

            if (winPosX < currentMapSize && (editedTiles[winPosX + 1, winPosY] == BlockType.Edible ||
                editedTiles[winPosX + 1, winPosY] == BlockType.Inedible))
            {
                blockPos.x = winPosX + 1; blockPos.y = winPosY;
            }
            else if (winPosX > 0 && (editedTiles[winPosX - 1, winPosY] == BlockType.Edible ||
                editedTiles[winPosX - 1, winPosY] == BlockType.Inedible))
            {
                blockPos.x = winPosX - 1; blockPos.y = winPosY;
            }
            else if (winPosY < currentMapSize && (editedTiles[winPosX, winPosY + 1] == BlockType.Edible ||
                editedTiles[winPosX, winPosY + 1] == BlockType.Inedible))
            {
                blockPos.x = winPosX; blockPos.y = winPosY + 1;
            }
            else if (winPosY > 0 && (editedTiles[winPosX, winPosY - 1] == BlockType.Edible ||
                editedTiles[winPosX, winPosY - 1] == BlockType.Inedible))
            {
                blockPos.x = winPosX; blockPos.y = winPosY - 1;
            }
        }
        return blockPos;
    }

    //private void CopyMap(BlockType[,] source, BlockType[,] target, int size)
    //{
    //    target = new BlockType[size, size];
    //    for (int i = 0; i < size; i++)
    //    {
    //        for (int j = 0; j < size; j++)
    //        {
    //            target[i, j] = source[i, j];
    //        }
    //    }
    //}

    private Array2DBlock ConvertEnumToArray2DBlock(BlockType[,] enumToConvert)
    {
        Array2DBlock array2DBlock = new Array2DBlock(enumToConvert.GetLength(0));

        Vector2Int sizeMap = new Vector2Int(enumToConvert.GetLength(0), enumToConvert.GetLength(1));
        array2DBlock.GridSize = sizeMap;

        for (int i = 0; i< enumToConvert.GetLength(0); i++)
        {
            for (int j = 0; j < enumToConvert.GetLength(1); j++)
            {
                array2DBlock.SetCell(i, j, enumToConvert[i, j]);
            }
        }        

        return array2DBlock;
    }

    private BlockType[,] ConvertArray2DBlockToEnumArray(Array2DBlock arrayToConvert)
    {
        var cells = arrayToConvert.GetCells();
        int cellSizeX = cells.GetLength(0);
        int cellSizeY = cells.GetLength(1);

        BlockType[,] enumArray = new BlockType[cellSizeX, cellSizeY];

        for (int i = 0; i < enumArray.GetLength(0); i++)
        {
            for (int j = 0; j < enumArray.GetLength(1); j++)
            {
                enumArray[i, j] = cells[j, i];
            }
        }

        return enumArray;
    }
    #endregion
}
