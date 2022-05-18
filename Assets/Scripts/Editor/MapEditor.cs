using UnityEditor;
using UnityEngine;

public class MapEditor : EditorWindow
{
    [MenuItem("MapGenerator/Map Generator")]
    public static void ShowWindow()
    {
        GetWindow<MapEditor>("Map Generator");
    }

    void Update()
    {
        
    }
}
