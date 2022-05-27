using Array2DEditor;
using UnityEngine;

//[CreateAssetMenu(fileName = "Level", menuName = "Assets/Level", order = 1)]
public class Level : ScriptableObject
{
    [SerializeField]
    private Array2DBlock map;

    public Array2DBlock Map { get => map; set => map = value; }
}
