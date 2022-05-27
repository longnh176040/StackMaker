using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [MenuItem("Editor/Map Editor")]
    static void OpenMapEditor()
    {
        EditorSceneManager.OpenScene("Assets/GamePlay/_Scenes/MapGenerator.unity");
        Selection.activeObject = GameObject.Find("Map Editor");
    }
}
