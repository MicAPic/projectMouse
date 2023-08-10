using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] 
    public PlayerInput[] playerInputs; 

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
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
        // TODO: pause the game here
        foreach (var playerInput in playerInputs)
        {
            playerInput.enabled = false;
            CameraController.Instance.focusPoint = 0.0f;
        }
    }
    
    public void Unpause()
    {
        // TODO: unpause the game here
        foreach (var playerInput in playerInputs)
        {
            playerInput.enabled = true;
            CameraController.Instance.focusPoint = CameraController.Instance.defaultFocusPoint;
        }
    }
}
