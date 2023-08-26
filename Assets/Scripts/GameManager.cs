using System.Globalization;
using Dan.Main;
using DG.Tweening;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Input")]
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

    public static bool isGameOver;
    public static bool isPaused;
    private int _highScore;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        pauseInputAction.performed += _ => TogglePauseScreen();
        
        _highScore = PlayerPrefs.GetInt("HighScore", 0);
    }
    
    void OnEnable()
    {
        pauseInputAction.Enable();
    }

    void OnDisable()
    {
        pauseInputAction.Disable();
    }

    public void Pause()
    {
        PlayerController.Instance.playerInput.enabled = false;

        Time.timeScale = 0.0f;
        CameraController.Instance.focusPoint = 0.0f;
        
        ui.cancelButton.Select();
    }
    
    public void Unpause()
    {
        PlayerController.Instance.playerInput.enabled = true;

        Time.timeScale = 1.0f;
        CameraController.Instance.focusPoint = CameraController.Instance.defaultFocusPoint;
    }

    public void GameOver()
    {
        isGameOver = true;
        pauseInputAction.Disable();
        
        ChatManager.Instance.EnableGameOverChatInfo();
        
        Pause();
        if (ui != null)
        {
            ui.gameOverScreen.SetActive(true);
        }
        else
        {
            TransitionController.Instance.TransitionAndLoadScene(SceneManager.GetActiveScene().name);
            return;
        }

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

    public void TogglePauseScreen()
    {
        if (ExperienceManager.Instance.isLevelingUp) return;
        
        isPaused = !isPaused;
        ui.pauseScreen.SetActive(isPaused);

        if (isPaused)
        {
            Pause();
        }
        else
        {
            Unpause();
        }
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

    private void PingLeaderboard()
    {
        LeaderboardCreator.Ping(isOnline =>
        {
            if (!isOnline)
            {
                ui.ToggleOfflineMode();
            }
            else
            {
                ui.UpdateLeaderboardContent(publicKey);
                ui.buttons[0].interactable = true;
                ui.nicknameInputField.interactable = true;
            }
            
            ui.buttons[1].Select();
        });
    }
}
