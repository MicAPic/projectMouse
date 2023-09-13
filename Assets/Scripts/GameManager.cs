using System.Globalization;
using Audio;
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
    [SerializeField] 
    private float shakeDuration = 0.485f;
    [SerializeField] 
    private Vector2 shakeStrength = new(0.0f, 0.004341f);
    [SerializeField] 
    private int shakeVibratio = 12;
    [SerializeField] 
    private float shakeRandomness;

    [Header("Leaderboards")]
    [SerializeField]
    private string publicKey;

    public static bool IsGameOver;
    public static bool IsPaused;
    public static bool CanPause = true;
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
        PlayerController.Instance.playerInput.SwitchCurrentActionMap("UI");
        AudioManager.Instance.ToggleLowpass(true);

        Time.timeScale = 0.0f;
        CameraController.Instance.focusPoint = 0.0f;
    }
    
    public void Unpause()
    {
        PlayerController.Instance.playerInput.SwitchCurrentActionMap("InGame");
        AudioManager.Instance.ToggleLowpass(false);

        Time.timeScale = 1.0f;
        CameraController.Instance.focusPoint = CameraController.Instance.DefaultFocusPoint;
    }

    public void GameOver()
    {
        IsGameOver = true;
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
        
        PingLeaderboard(score);
    }

    public void TogglePauseScreen()
    {
        if (ExperienceManager.Instance.isLevelingUp || !CanPause) return;
        
        IsPaused = !IsPaused;
        ui.pauseScreen.SetActive(IsPaused);

        if (IsPaused)
        {
            Pause();
            ui.cancelButton.Select();
        }
        else
        {
            Unpause();
        }
    }

    public void SubmitHighScore()
    {
        var nickname = ui.nicknameInputField.text.Trim();
        if (nickname == string.Empty) return;
        
        ui.ToggleButtons(false);
        
        LeaderboardCreator.UploadNewEntry(publicKey, nickname, _highScore,
            _ =>
            {
                ui.UpdateLeaderboardContent(publicKey, updateStatistics:false);
                ui.nicknameInputField.text = string.Empty;
                ui.ToggleButtons(true);
            },
            error =>
            {
                switch (error)
                {
                    case null:
                        break;
                    case "403: Username is profane!":
                    case "409: Username already exists!":
                        ui.nicknameInputField.GetComponent<RectTransform>()
                                             .DOShakeAnchorPos(
                                                 shakeDuration,
                                                 shakeStrength,
                                                 shakeVibratio,
                                                 shakeRandomness,
                                                 false,
                                                 true,
                                                 ShakeRandomnessMode.Harmonic
                                            )
                                            .SetUpdate(true);
                        break;
                    default:
                        Debug.LogError(error); 
                        ui.ToggleOfflineMode();
                        break;
                }
                ui.ToggleButtons(true);
            });
    }

    private void PingLeaderboard(int currentScore)
    {
        LeaderboardCreator.Ping(isOnline =>
        {
            if (!isOnline)
            {
                ui.ToggleOfflineMode();
            }
            else
            {
                ui.UpdateLeaderboardContent(publicKey, currentScore);
                ui.buttons[0].interactable = true;
                ui.nicknameInputField.interactable = true;
            }
            
            ui.buttons[1].Select();
        });
    }
}
