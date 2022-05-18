using UnityEngine;

public class EdibleBlock : Block
{
    public GameObject blockMesh;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constant.PLAYER_TAG))
        {
            Destroy(blockMesh);
        }
    }

    public override void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Constant.PLAYER_TAG))
        {
            gameObject.tag = Constant.WALKABLE_BLOCK_TAG;
        }
    }
}
