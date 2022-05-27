using System;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public readonly int TOUCH_THRESHOLD = 125;

    public static Action<Direction> OnSwipe; //0: Forward, 1: Back, 2: Left, 3: Right

    public Direction Direction { get { return direction; } }

    private Direction direction = Direction.None;
    private Direction lastDirection = Direction.None;

    private Vector2 _touchPosition;
    private Vector2 _touchDistance;

    private bool _isDragging;
    private float _touchX, _touchY;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _isDragging = true;
            _touchPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
            _touchPosition = Vector2.zero;
            _touchDistance = Vector2.zero;
        }

        _touchDistance = Vector2.zero;
        if (_isDragging)
        {
            if (Input.GetMouseButton(0))
            {
                _touchDistance = (Vector2)Input.mousePosition - _touchPosition;
            }
        }

        if (_touchDistance.magnitude > TOUCH_THRESHOLD)
        {
            _touchX = _touchDistance.x;
            _touchY = _touchDistance.y;

            if (Mathf.Abs(_touchX) > Mathf.Abs(_touchY))    //Vuốt trái hoặc phải
            {
                if (_touchX < 0)
                {
                    UpdateDirection(Direction.Left);
                    //Debug.Log("Left");
                }
                else
                {
                    UpdateDirection(Direction.Right);
                    //Debug.Log("Right");
                }
            }
            else  //Vuốt lên hoặc xuống
            {
                if (_touchY < 0)
                {
                    UpdateDirection(Direction.Back);
                    //Debug.Log("Back");
                }
                else
                {
                    UpdateDirection(Direction.Forward);
                    //Debug.Log("Forward");
                }
            }
        }
    }

    void UpdateDirection(Direction dir)
    {
        if (direction != dir)
        {
            lastDirection = direction;
            direction = dir;
            OnSwipe(direction);
        }
    }
}

public enum Direction
{
    Forward,
    Back,
    Left,
    Right, 
    None
}
