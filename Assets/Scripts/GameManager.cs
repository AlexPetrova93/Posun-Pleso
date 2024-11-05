using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public SoundManager SoundManager;
    public Camera Camera;
    public Pipes Pipes;
    public Aquarium Aquarium;

    public GameObject PauseButton;
    public RectTransform CheatButton;
    public RectTransform CheatInput;
    
    public GameObject PauseMenu;
    public GameObject GameOverScreen;
    public GameObject GameWonScreen;
    public GameObject GameSuperWonScreen;
    public GameObject GameKindOfWonScreen;
    public GameObject CheatScreen;
    
    private bool _gameRunning;
    private bool _cheatGameRunning;

    private float _cheatBtnX;

    private void Start()
    {
        SoundManager.Init();
        Pipes.Init(SoundManager, Camera);
        Aquarium.Init(SoundManager, Pipes, Camera);
        Aquarium.Guy.Init(SoundManager);
        PauseMenu.SetActive(false);
        GameOverScreen.SetActive(false);
        GameWonScreen.SetActive(false);
        GameSuperWonScreen.SetActive(false);
        GameKindOfWonScreen.SetActive(false);
        CheatInput.localScale = new Vector3(0, 1, 1);
        _cheatBtnX = CheatButton.anchoredPosition.x;
        CheatButton.anchoredPosition = Vector2.zero;
        
        StartGame();
    }
    
    public void StartGame()
    {
        SoundManager.PlayMusic(AudioRef.Music);
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
        SoundManager.PlaySound(AudioRef.GameOver);
        _gameRunning = false;
        GameOverScreen.SetActive(true);
    }

    private void GameWon()
    {
        SoundManager.PlaySound(AudioRef.Win);
        _gameRunning = false;

        if (Aquarium.CrackedAtLeastOnce) GameWonScreen.SetActive(true);
        else GameSuperWonScreen.SetActive(true);
    }

    private void GameKindOfWon()
    {
        StartCoroutine(GameKindOfWonRoutine());
    }

    private IEnumerator GameKindOfWonRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        _gameRunning = false;
        _cheatGameRunning = false;
        GameKindOfWonScreen.SetActive(true);
    }

    private bool _cheatFieldVisible;

    public void ToggleCheatWindow()
    {
        if (!_gameRunning) return;
        if (_cheatFieldVisible) HideCheatWindow();
        else ShowCheatWindow();
    }

    private void ShowCheatWindow()
    {
        _cheatFieldVisible = true;
        CheatInput.DOKill();
        CheatInput.DOScaleX(1, .3f);
        CheatButton.DOKill();
        CheatButton.DOAnchorPosX(_cheatBtnX, .3f);
    }

    private void HideCheatWindow()
    {
        _cheatFieldVisible = false;
        CheatInput.DOKill();
        CheatInput.DOScaleX(0, .3f);
        CheatButton.DOKill();
        CheatButton.DOAnchorPosX(0, .3f);
    }

    public void CheckCheatCode(TMP_InputField inputField)
    {
        if (!_gameRunning) return;
        if (inputField.text.Equals("MoneyRulez999", StringComparison.InvariantCultureIgnoreCase))
        {
            inputField.text = "";
            CheatScreen.SetActive(false);
            PauseButton.SetActive(false);
            CheatScreen.SetActive(false);
            _gameRunning = false;
            StartCoroutine(Aquarium.Guy.Leave());
            Aquarium.SetUnlimitedWater();
            Aquarium.OnTankFull.RemoveAllListeners();
            Aquarium.OnLeakedTooMuch.RemoveAllListeners();
            Aquarium.OnLeakedAll.AddListener(GameKindOfWon);
            StartCoroutine(Pipes.FillAllWithPipes());
            SoundManager.PlaySound(AudioRef.MoneyRulez);
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
