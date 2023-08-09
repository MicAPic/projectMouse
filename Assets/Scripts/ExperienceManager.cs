using UnityEngine;

public class ExperienceManager : MonoBehaviour
{
    public static ExperienceManager Instance { get; private set; }
    
    [SerializeField] 
    private AnimationCurve experienceCurve;
    
    private int _currentLevel = 1;
    private float _experienceToNextLevel;
    private float _totalExperiencePoints;

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
    void Start()
    {
        ReevaluateExpGoal();
    }

    public void AddExperience(float expToAdd)
    {
        _totalExperiencePoints += expToAdd;
        if (_totalExperiencePoints >= _experienceToNextLevel)
        {
            LevelUp();
        }
        
        // TODO: animate this stuff
    }

    private void LevelUp()
    {
        _currentLevel++;
        if (_currentLevel > experienceCurve[experienceCurve.length - 1].time)
        {
            Debug.LogWarning("Maximum level has been reached. The EXP curve is now a flat line");
        }
        ReevaluateExpGoal();
        
        // TODO: animate this stuff
    }

    private void ReevaluateExpGoal()
    {
        _experienceToNextLevel = experienceCurve.Evaluate(_currentLevel);
    }
}
