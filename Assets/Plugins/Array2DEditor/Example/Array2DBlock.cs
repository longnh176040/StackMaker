using UnityEngine;

namespace Array2DEditor
{
    [System.Serializable]
    public class Array2DBlock : Array2D<BlockType>
    {
        [SerializeField]
        CellRowBlockEnum[] cells = new CellRowBlockEnum[Consts.defaultGridSize];

        protected override CellRow<BlockType> GetCellRow(int idx)
        {
            return cells[idx];
        }
    }

    [System.Serializable]
    public class CellRowBlockEnum : CellRow<BlockType> { }
}
