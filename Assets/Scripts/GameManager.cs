using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public PlayerInput[] playerInputs;
    public InputAction pauseInputAction;
    
    [SerializeField] 
    public GameObject pauseScreen;
    [SerializeField] 
    public GameObject gameOverScreen;
    private bool _isPaused;

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
