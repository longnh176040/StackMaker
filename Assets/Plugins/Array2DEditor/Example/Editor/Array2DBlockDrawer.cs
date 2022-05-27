using UnityEditor;

namespace Array2DEditor
{
    [CustomPropertyDrawer(typeof(Array2DBlock))]
    public class Array2DBlockDrawer : Array2DEnumDrawer<BlockType> { }
}
