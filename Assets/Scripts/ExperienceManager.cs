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
    private float _previousExperienceToLevelUp = 0.0f;
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
    private float experienceGainAnimationMaxDuration;

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
        ReevaluateExpGoal();
        FillExperienceBar();
    }

    public void AddExperience(float expToAdd)
    {
        currentExperienceText.DOCounter(
            (int)(_totalExperiencePoints * 100),
            (int)((_totalExperiencePoints + expToAdd) * 100),
            (expToAdd / _experienceToLevelUp) * experienceGainAnimationMaxDuration);
        
        _totalExperiencePoints += expToAdd;

        // TODO: animate this stuff more!
        FillExperienceBar();
    }

    private void LevelUp()
    {
        experienceBarFill.fillAmount = 0.0f;
        
        _currentLevel++;
        
        powerUps.Shuffle();
        for (var i = 0; i < 3; i++)
        {
            Instantiate(powerUps[i], powerUpSelection.transform);
        }
        powerUpSelection.transform.GetChild(0).GetComponent<Button>().Select();
        powerUpSelection.SetActive(true);
        
        GameManager.Instance.Pause();
        if (_currentLevel > experienceCurve[experienceCurve.length - 1].time)
        {
            Debug.LogWarning("Maximum level has been reached. The EXP curve is now a flat line");
        }
    }

    private void ReevaluateExpGoal()
    {
        _previousExperienceToLevelUp = _experienceToLevelUp;
        _experienceToLevelUp = experienceCurve.Evaluate(_currentLevel);
        
        experienceGoalText.DOCounter(
            (int)(_previousExperienceToLevelUp * 100), 
            (int)(_experienceToLevelUp * 100), 
            experienceGainAnimationMaxDuration);
    }

    private void FillExperienceBar()
    {
        var levelUpCondition = _totalExperiencePoints >= _experienceToLevelUp;
        var fillAmount =  levelUpCondition
            ? 1.0f 
            : 1.0f - (_experienceToLevelUp - _totalExperiencePoints) / 
                     (_experienceToLevelUp - _previousExperienceToLevelUp);
        var animationDuration = (fillAmount - experienceBarFill.fillAmount) * experienceGainAnimationMaxDuration;
        var tween = experienceBarFill.DOFillAmount(fillAmount, animationDuration);
        if (levelUpCondition)
        {
            tween.SetUpdate(true);
            tween.OnComplete(LevelUp);
        }
    }
}
