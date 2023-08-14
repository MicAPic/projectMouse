using System;
using System.Globalization;
using Dan.Main;
using DG.Tweening;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Input")]
    public PlayerInput[] playerInputs;
    public InputAction pauseInputAction;

    [Header("UI")] 
    [SerializeField] 
    private InGameUI ui;

    [Header("Animation")]
    [SerializeField]
    private float scoreCountDuration;

    [Header("Leaderboards")]
    [SerializeField]
    private string publicKey;
    
    private bool _isPaused;
    private int _highScore;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        playerInputs = FindObjectsOfType<PlayerInput>();
        pauseInputAction.performed += _ => TogglePauseScreen();

        _highScore = PlayerPrefs.GetInt("HighScore", 0);
    }
    
    void OnEnable()
    {
        pauseInputAction.Enable();
    }

    // Start is called before the first frame update
    // void Start()
    // {
    //     
    // }

    // Update is called once per frame
    // void Update()
    // {
    //     
    // }

    public void Pause()
    {
        Time.timeScale = 0.0f;
        
        foreach (var playerInput in playerInputs)
        {
            playerInput.enabled = false;
            CameraController.Instance.focusPoint = 0.0f;
        }
    }
    
    public void Unpause()
    {
        Time.timeScale = 1.0f;
        
        foreach (var playerInput in playerInputs)
        {
            playerInput.enabled = true;
            CameraController.Instance.focusPoint = CameraController.Instance.defaultFocusPoint;
        }
    }

    public void GameOver()
    {
        pauseInputAction.Disable();
        
        Pause();
        ui.gameOverScreen.SetActive(true);

        var score = (int)(ExperienceManager.Instance.TotalExperiencePoints * 100);

        if (score > _highScore)
        {
            _highScore = score;
            PlayerPrefs.SetInt("HighScore", _highScore);
        }
        
        ui.scoreText.DOCounter(
            0,
            score,
            scoreCountDuration
            ).SetUpdate(true);

        ui.highScoreText.text = "High score: " + _highScore.ToString("N0", CultureInfo.InvariantCulture);
        
        PingLeaderboard();
    }

    public void SubmitHighScore()
    {
        var nickname = ui.nicknameInputField.text;
        if (nickname == string.Empty) return;
        
        ui.ToggleButtons(false);
        
        LeaderboardCreator.UploadNewEntry(publicKey, nickname, _highScore,
            _ =>
            {
                ui.UpdateLeaderboardContent(publicKey);
                ui.nicknameInputField.text = string.Empty;
                ui.ToggleButtons(true);
            },
            error =>
            {
                if (error != null)
                    Debug.LogError(error);
                ui.ToggleButtons(true);
                ui.ToggleOfflineMode();
            });
    }

    private void TogglePauseScreen()
    {
        _isPaused = !_isPaused;
        ui.pauseScreen.SetActive(_isPaused);

        if (_isPaused)
        {
            Pause();
        }
        else
        {
            Unpause();
        }
    }

    private void PingLeaderboard()
    {
        LeaderboardCreator.Ping(isOnline =>
        {
            if (!isOnline)
            {
                ui.ToggleOfflineMode();
                ui.buttons[1].Select();
            }
            else
            {
                ui.UpdateLeaderboardContent(publicKey);
                ui.buttons[0].interactable = true;
                ui.nicknameInputField.interactable = true; 
                ui.nicknameInputField.Select();
            }
        });
    }
}
