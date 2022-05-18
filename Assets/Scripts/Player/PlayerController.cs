using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector3 targetPos;
    private float speed = 15f;
    private bool _canMove;

    private void Start()
    {
        InputHandler.OnSwipe += FindTarget;
    }

    private void Update()
    {
        if (_canMove)
        {
            if (Vector3.Distance(transform.position, targetPos) > 0.0001f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            }
            else
            {
                _canMove = false;
            }
        }
    }

    private void FindTarget(Direction direction)
    {
        switch (direction)
        {
            case Direction.Forward:
                Raycasting(transform.position, Vector3.forward);
                break;

            case Direction.Back:
                Raycasting(transform.position, Vector3.back);
                break;

            case Direction.Left:
                Raycasting(transform.position, Vector3.left);
                break;

            case Direction.Right:
                Raycasting(transform.position, Vector3.right);
                break;
        }

        _canMove = true;
    }

    private void Raycasting(Vector3 startRay, Vector3 dirUnit)
    {
        RaycastHit hit;
        if (Physics.Raycast(startRay, dirUnit, out hit, 1f))
        {
            //Debug.Log("Hit " + hit.transform.name);

            if (hit.transform.CompareTag(Constant.EDIBLE_BLOCK_TAG) || 
                hit.transform.CompareTag(Constant.INEDIBLE_BLOCK_TAG) ||
                hit.transform.CompareTag(Constant.WALKABLE_BLOCK_TAG))
            {
                targetPos = hit.transform.position;
                startRay += dirUnit;

                Raycasting(startRay, dirUnit);
            }
            else
            {
                //Debug.Log("Hit Unidentify object" + hit.transform.name);
            }
        }
        else
        {
            //Debug.Log("No hit" );
        }
    }
}
