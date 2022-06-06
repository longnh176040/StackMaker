using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameState GameState { get => gameState; }
    private GameState gameState;

    public LevelManager levelManager;

    #region Singleton
    public static GameManager Ins;
    private void Awake()
    {
        Ins = this;
    }
    #endregion

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    public void SetGameState(GameState state)
    {
        if (gameState != state)
        {
            gameState = state;
        }
    }
}

public enum GameState
{
    MainMenu,
    GamePlay,
    Pause,
    EndGame
}