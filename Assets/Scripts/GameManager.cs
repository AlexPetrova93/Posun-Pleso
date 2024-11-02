using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Camera Camera;
    public Pipes Pipes;
    public Aquarium Aquarium;

    public GameObject PauseButton;
    public GameObject CheatButton;
    
    public GameObject PauseMenu;
    public GameObject GameOverScreen;
    public GameObject GameWonScreen;
    public GameObject GameKindOfWonScreen;
    public GameObject CheatScreen;
    
    private bool _gameRunning;
    private bool _cheatGameRunning;

    private void Start()
    {
        Pipes.Init(Camera);
        Aquarium.Init(Pipes, Camera);
        PauseMenu.SetActive(false);
        GameOverScreen.SetActive(false);
        GameWonScreen.SetActive(false);
        GameKindOfWonScreen.SetActive(false);
        CheatScreen.SetActive(false);
        
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
        if (_cheatGameRunning)
        {
            Aquarium.UpdateProgress();
            return;
        }
        
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
        _cheatGameRunning = false;
        GameKindOfWonScreen.SetActive(true);
    }

    public void ShowCheatScreen()
    {
        _gameRunning = false;
        CheatScreen.SetActive(true);
    }

    public void HideCheatScreen()
    {
        _gameRunning = true;
        CheatScreen.SetActive(false);
    }

    public void CheckCheatCode(TMP_InputField inputField)
    {
        if (inputField.text.Equals("MoneyRulez999", StringComparison.InvariantCultureIgnoreCase))
        {
            inputField.text = "";
            CheatScreen.SetActive(false);
            PauseButton.SetActive(false);
            CheatButton.SetActive(false);
            _gameRunning = false;
            Aquarium.SetUnlimitedWater();
            Aquarium.OnTankFull.RemoveAllListeners();
            Aquarium.OnLeakedTooMuch.RemoveAllListeners();
            Aquarium.OnLeakedAll.AddListener(GameKindOfWon);
            StartCoroutine(Pipes.FillAllWithPipes());
            _cheatGameRunning = true;
        }
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
