using UnityEngine;

namespace Array2DEditor
{
    [System.Serializable]
    public class Array2DBlock : Array2D<BlockType>
    {
        [SerializeField]
        CellRowBlockEnum[] cells = new CellRowBlockEnum[Consts.defaultGridSize];

        public Array2DBlock(int size)
        {
            cells = new CellRowBlockEnum[size];
            for (int i = 0; i<size; i++)
            {
                cells[i] = new CellRowBlockEnum(size);
            }
        }

        protected override CellRow<BlockType> GetCellRow(int idx)
        {
            return cells[idx];
        }
    }

    [System.Serializable]
    public class CellRowBlockEnum : CellRow<BlockType> {
        public CellRowBlockEnum(int size)
        {
            row = new BlockType[size];
        }
    }
}
