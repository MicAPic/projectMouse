using System.Globalization;
using DG.Tweening;
using TMPro;
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
    private GameObject pauseScreen;
    [SerializeField] 
    private GameObject gameOverScreen;
    [SerializeField] 
    private TMP_Text scoreText;
    [SerializeField] 
    private TMP_Text highScoreText;

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

    void OnDisable()
    {
        pauseInputAction.Disable();
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
        Pause();
        gameOverScreen.SetActive(true);

        var score = (int)(ExperienceManager.Instance.TotalExperiencePoints * 100);

        if (score > _highScore)
        {
            _highScore = score;
            PlayerPrefs.SetInt("HighScore", _highScore);
        }
        
        scoreText.DOCounter(
            0,
            score,
            scoreCountDuration
            ).SetUpdate(true);

        highScoreText.text = "High score: " + _highScore.ToString("N0", CultureInfo.InvariantCulture);
    }

    private void TogglePauseScreen()
    {
        _isPaused = !_isPaused;
        pauseScreen.SetActive(_isPaused);

        if (_isPaused)
        {
            Pause();
        }
        else
        {
            Unpause();
        }
    }
}
