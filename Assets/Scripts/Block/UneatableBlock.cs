using UnityEngine;

public class UneatableBlock : Block
{   
    public override void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Constant.PLAYER_TAG))
        {
            gameObject.tag = Constant.WALKABLE_BLOCK_TAG;
        }
    }
}
