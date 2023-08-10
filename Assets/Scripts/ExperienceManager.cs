using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ExperienceManager : MonoBehaviour
{
    public static ExperienceManager Instance { get; private set; }
    
    [Header("Gameplay")]
    [SerializeField] 
    private AnimationCurve experienceCurve;
    [SerializeField] 
    private List<GameObject> powerUps;

    private int _currentLevel = 1;
    private float _experienceToLevelUp;
    private float _totalExperiencePoints;
    
    [Header("UI")]
    [FormerlySerializedAs("levelUpMenu")]
    [SerializeField]
    private GameObject powerUpSelection;
    [SerializeField]
    private Image experienceBarFill;
    [SerializeField]
    private TMP_Text currentExperienceText;
    [SerializeField]
    private TMP_Text experienceGoalText;
    
    [Header("Animation")]
    [SerializeField]
    private float experienceGainAnimationDuration;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (powerUps.Count < 3)
        {
            Debug.LogError("Not enough power-ups were added in the Editor");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ReevaluateExpGoal();
    }

    public void SelectPowerUp()
    {
        powerUpSelection.SetActive(false);
        foreach (Transform powerUp in powerUpSelection.transform)
        {
            Destroy(powerUp.gameObject);
        }
        
        GameManager.Instance.Unpause();
    }

    public void AddExperience(float expToAdd)
    {
        currentExperienceText.DOCounter(
            (int)(_totalExperiencePoints * 100),
            (int)((_totalExperiencePoints + expToAdd) * 100),
            experienceGainAnimationDuration);
        
        _totalExperiencePoints += expToAdd;
        
        if (_totalExperiencePoints >= _experienceToLevelUp)
        {
            LevelUp();
        }
        
        // TODO: animate this stuff more!
        FillExperienceBar();
    }

    private void LevelUp()
    {
        _currentLevel++;
        
        powerUps.Shuffle();
        for (var i = 0; i < 3; i++)
        {
            Instantiate(powerUps[i], powerUpSelection.transform);
        }
        powerUpSelection.SetActive(true);
        
        GameManager.Instance.Pause();
        if (_currentLevel > experienceCurve[experienceCurve.length - 1].time)
        {
            Debug.LogWarning("Maximum level has been reached. The EXP curve is now a flat line");
        }
        ReevaluateExpGoal();
    }

    private void ReevaluateExpGoal()
    {
        _experienceToLevelUp = experienceCurve.Evaluate(_currentLevel);
        
        experienceGoalText.DOCounter(
            (int)(experienceCurve.Evaluate(_currentLevel - 1) * 100), 
            (int)(experienceCurve.Evaluate(_currentLevel) * 100), 
            experienceGainAnimationDuration);
    }

    private void FillExperienceBar()
    {
        experienceBarFill.DOFillAmount(_totalExperiencePoints / _experienceToLevelUp, experienceGainAnimationDuration);
    }
}
