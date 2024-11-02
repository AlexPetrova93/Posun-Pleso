using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Camera Camera;
    public Pipes Pipes;
    public Aquarium Aquarium;

    public GameObject PauseMenu;
    public GameObject GameOverScreen;
    public GameObject GameWonScreen;
    public GameObject GameKindOfWonScreen;
    
    private bool _gameRunning;

    private void Start()
    {
        Pipes.Init(Camera);
        Aquarium.Init(Pipes, Camera);
        PauseMenu.SetActive(false);
        GameOverScreen.SetActive(false);
        GameWonScreen.SetActive(false);
        GameKindOfWonScreen.SetActive(false);
        
        StartGame();
    }
    
    public void StartGame()
    {
        _gameRunning = true;
        Pipes.StartGame();
        Aquarium.OnTankFull.AddListener(GameWon);
        Aquarium.OnLeakedTooMuch.AddListener(GameOver);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ToMenu()
    {
        SceneManager.LoadScene("MainMenu");
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

    private void GameOver()
    {
        _gameRunning = false;
        GameOverScreen.SetActive(true);
    }

    private void GameWon()
    {
        _gameRunning = false;
        GameWonScreen.SetActive(true);
    }

    private void GameKindOfWon()
    {
        _gameRunning = false;
        GameKindOfWonScreen.SetActive(true);
    }

    public void ToggleGamePaused()
    {
        if (_gameRunning) PauseGame();
        else UnPauseGame();
    }

    public void PauseGame()
    {
        PauseMenu.SetActive(true);
        _gameRunning = false;
    }

    public void UnPauseGame()
    {
        PauseMenu.SetActive(false);
        _gameRunning = true;
    }

}
