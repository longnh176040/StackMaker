using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool CanMove { get => _canMove; }

    private Vector3 targetPos;
    private float speed = 15f;
    private bool _canMove;
    private Transform _thisTransform;

    #region Singleton
    public static PlayerController Ins;
    private void Awake()
    {
        Ins = this;
    }
    #endregion

    private void Start()
    {
        _thisTransform = transform;
        targetPos = _thisTransform.position;
        InputHandler.OnSwipe += FindTarget;
    }

    private void Update()
    {
        if (_canMove)
        {
            if ((_thisTransform.position - targetPos).sqrMagnitude > 0.0001f)
            {
                _thisTransform.position = Vector3.MoveTowards(_thisTransform.position, targetPos, speed * Time.deltaTime);
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
                Raycasting(_thisTransform.position, Vector3.forward);
                break;

            case Direction.Back:
                Raycasting(_thisTransform.position, Vector3.back);
                break;

            case Direction.Left:
                Raycasting(_thisTransform.position, Vector3.left);
                break;

            case Direction.Right:
                Raycasting(_thisTransform.position, Vector3.right);
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
