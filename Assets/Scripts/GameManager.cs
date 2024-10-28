using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Camera Camera;
    public Pipes Pipes;
    public Aquarium Aquarium;
    
    private bool _gameRunning;

    private void Start()
    {
        Pipes.Init(Camera);
        Aquarium.Init(Pipes, Camera);
        StartGame();
    }
    
    public void StartGame()
    {
        _gameRunning = true;
        Pipes.StartGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleGamePaused();
        }
        
        if (!_gameRunning) return;
        
        var input = Utility.GetInput();
        Pipes.HandleInput(input);
        
        Aquarium.UpdateProgress();
        Aquarium.HandleInput(input);
    }

    public void ToggleGamePaused()
    {
        if (_gameRunning) PauseGame();
        else UnPauseGame();
    }

    public void PauseGame()
    {
        _gameRunning = false;
    }

    public void UnPauseGame()
    {
        _gameRunning = true;
    }

}
