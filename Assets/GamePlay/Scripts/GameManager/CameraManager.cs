using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Vector3 offset;
    public float speedMove;

    private Transform mainCamera;

    private PlayerStack playerStack;
    private PlayerController player;
    private Transform playerTransform;

    private Vector3 _endGameOffset = new Vector3(-5, -2, 5);

    private void Start()
    {
        playerStack = PlayerStack.Ins;
        player = PlayerController.Ins;
        playerTransform = player.transform;

        mainCamera = transform;
    }

    private void LateUpdate()
    {
        if (playerStack.IsWin)
        {
            offset = Vector3.Lerp(offset, _endGameOffset, speedMove * Time.deltaTime);
        }

        mainCamera.position = playerTransform.position - offset;
        mainCamera.LookAt(playerTransform.position + Vector3.up);
    }

    public void SetCamEndGame()
    {
        offset = _endGameOffset;
    }
}
