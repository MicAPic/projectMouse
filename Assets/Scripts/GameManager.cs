using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

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
    }
    
    public void Unpause()
    {
        // TODO: unpause the game here
    }
}
