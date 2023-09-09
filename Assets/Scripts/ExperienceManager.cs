using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Audio;
using DG.Tweening;
using HealthControllers;
using PowerUps;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ExperienceManager : MonoBehaviour
{
    public static ExperienceManager Instance { get; private set; }

    [Header("Gameplay")]
    public bool isLevelingUp;
    [SerializeField] 
    private AnimationCurve experienceCurve;
    [SerializeField] 
    private List<GameObject> powerUps;

    public Dictionary<GameObject, float> PowerUpsWithCounters;
    public float TotalExperiencePoints { get; private set; }

    private int _currentLevel = 1;
    private float _experienceToLevelUp;
    private float _previousExperienceToLevelUp = 0.0f;

    [Header("Audio")]
    [SerializeField]
    private AudioClip levelUpSoundEffect;
    [SerializeField]
    private float levelUpSoundEffectDelay = 0.438f;

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
    [SerializeField]
    private TMP_Text experiencePercentageText;
    [SerializeField]
    private RectTransform experienceCanvas;
    
    private VerticalLayoutGroup _powerUpSelectionLayoutGroup;
    private float _powerUpButtonSpacingY;

    [Header("Animation")]
    [SerializeField]
    private float experienceGainAnimationMaxDuration;
    [SerializeField] 
    private GameObject experienceCirclePrefab;
    [SerializeField] 
    private RectTransform experienceCircleDestination;
    [SerializeField] 
    private float circleAnimationDuration;
    [SerializeField]
    private float powerUpAppearanceDuration;
    [SerializeField]
    private ParticleSystem[] appearanceParticles;
    [SerializeField]
    private RectTransform uiParticlesTransform;

    private Queue<RectTransform> _experienceCircles = new();

    private PlayerHealth playerHealth;

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
        PowerUpsWithCounters = powerUps.ToDictionary(powerUp => powerUp, _ => 0f);
        // This is required to prevent the following power-ups from appearing too soon 
        PowerUpsWithCounters[powerUps[^2]] = 0.5f; // shotgun power up
        PowerUpsWithCounters[powerUps[^1]] = 0.5f; // magic bullets power up

        _powerUpSelectionLayoutGroup = powerUpSelection.GetComponent<VerticalLayoutGroup>();
        _powerUpButtonSpacingY = powerUps[0].GetComponent<RectTransform>().sizeDelta.y + 
                                 _powerUpSelectionLayoutGroup.spacing;
        
        playerHealth = FindObjectOfType<PlayerHealth>();
    }

    // Start is called before the first frame update
    void Start()
    {
        ReevaluateExpGoal();
    }

    public void SelectPowerUp()
    {
        isLevelingUp = false;
        
        experienceBarFill.fillAmount = 0.0f;
        
        powerUpSelection.SetActive(false);
        foreach (Transform powerUp in powerUpSelection.transform)
        {
            Destroy(powerUp.gameObject);
        }

        ReevaluateExpGoal();
        StartCoroutine(FillExperienceBar());
        
        playerHealth.GrantInvincibilityOnLevelUp();

        ChatManager.Instance.EnableGeneralChatInfo();
        GameManager.Instance.Unpause();
        PixelPerfectCursor.Instance.Toggle();
        
        if (TextManager.Instance != null)
        {
            FindObjectOfType<TutorialUI>().ToggleDialogueBox(true, 0.0f)
                                          .OnComplete(TextManager.Instance.ContinueStory);
        }
    }

    public void RemoveFromPowerUps(PowerUpBase powerUpBase)
    {
        var powerUpName = powerUpBase.GetType().Name;
        Debug.Log(powerUpName);
        powerUps.RemoveAll(powerUp => powerUp.name.Contains(powerUpName));
    }

    public void AddExperience(float expToAdd)
    {
        ChatManager.Instance.DisplayComment(ChatManager.CommentType.Donation, ((int)(expToAdd * 100)).ToString());
        AnimateExperienceGain(expToAdd);
    }

    private void LevelUp()
    {
        if (isLevelingUp || GameManager.IsGameOver) return;
        isLevelingUp = true;

        _currentLevel++;
        if (_currentLevel > experienceCurve[experienceCurve.length - 1].time)
        {
            Debug.LogWarning("Maximum level has been reached. The EXP curve is now a flat line");
        }
        
        // Shuffle our list and sort it by usage
        powerUps.Shuffle();
        powerUps = powerUps.OrderBy(powerUp => PowerUpsWithCounters[powerUp]).ToList();

        var buttons = new List<Button>();
        for (var i = 2; i >= 0; i--)
        {
            try
            {
                var button = Instantiate(powerUps[i], powerUpSelection.transform);
                button.transform.SetAsFirstSibling();
                buttons.Add(button.GetComponent<Button>());
            }
            catch (IndexOutOfRangeException)
            {
                continue;
            }
        }
        
        // Animate level up screen & set up navigation
        _powerUpSelectionLayoutGroup.enabled = false;
        
        var currentPosY = -_powerUpButtonSpacingY;
        Tween fadeTween = null;
        for (var i = 0; i < buttons.Count; i++)
        {
            var navigation = new Navigation
            {
                mode = Navigation.Mode.Explicit,
                selectOnUp = buttons[(i + 1).Modulo(buttons.Count)],
                selectOnDown = buttons[(i - 1).Modulo(buttons.Count)]
            };
            buttons[i].navigation = navigation;
            var rect = buttons[i].GetComponent<RectTransform>();
            rect.DOAnchorPosY(currentPosY, powerUpAppearanceDuration)
                .SetUpdate(true);
                
            var buttonGroup = buttons[i].GetComponent<CanvasGroup>();
            buttonGroup.alpha = 0.0f;
            fadeTween = buttonGroup.DOFade(1.0f, powerUpAppearanceDuration)
                                   .SetUpdate(true);
            
            currentPosY += _powerUpButtonSpacingY;
        }

        fadeTween.OnComplete(() =>
        {
            foreach (var particle in appearanceParticles)
            {
                particle.Play();
            }
            _powerUpSelectionLayoutGroup.enabled = true;
            buttons[^2].Select();
        });

        powerUpSelection.SetActive(true);
        
        ChatManager.Instance.EnableLevelUpChatInfo();
        GameManager.Instance.Pause();
        PixelPerfectCursor.Instance.Toggle();
    }

    private void ReevaluateExpGoal()
    {
        _previousExperienceToLevelUp = _experienceToLevelUp;
        _experienceToLevelUp = experienceCurve.Evaluate(_currentLevel);

        experiencePercentageText.text = "0";
        
        experienceGoalText.DOCounter(
            (int)(_previousExperienceToLevelUp * 100), 
            (int)(_experienceToLevelUp * 100), 
            experienceGainAnimationMaxDuration);
    }

    private IEnumerator FillExperienceBar()
    {
        var levelUpCondition = TotalExperiencePoints >= _experienceToLevelUp;
        var fillAmount =  levelUpCondition
            ? 1.0f 
            : 1.0f - (_experienceToLevelUp - TotalExperiencePoints) / 
                     (_experienceToLevelUp - _previousExperienceToLevelUp);
        var animationDuration = (fillAmount - experienceBarFill.fillAmount) * experienceGainAnimationMaxDuration;
        var tween = experienceBarFill.DOFillAmount(fillAmount, animationDuration);
        if (levelUpCondition)
        {
            GameManager.CanPause = false;
            tween.SetUpdate(true);
            tween.OnComplete(() =>
            {
                GameManager.CanPause = true;
                LevelUp();
            });
            yield return new WaitForSeconds(animationDuration - levelUpSoundEffectDelay);
            if (!GameManager.IsGameOver && !isLevelingUp)
            {
                AudioManager.Instance.sfxSource.PlayOneShot(levelUpSoundEffect);
            }
        }
    }

    private void AnimateExperienceGain(float expToAdd)
    {
        if (!_experienceCircles.TryDequeue(out var result))
        {
            result = Instantiate(experienceCirclePrefab, experienceCanvas).GetComponent<RectTransform>();
        }
        
        result.anchoredPosition = experienceCanvas.rect.min + new Vector2(80.0f, -5f);
        result.gameObject.SetActive(true);

        var sequence = DOTween.Sequence();

        sequence.Append(result.DOAnchorPos(experienceCanvas.rect.min + new Vector2(80.0f, 5f), 0.5f));

        sequence.Append(result.DOAnchorPos(experienceCircleDestination.anchoredPosition, 
                                           circleAnimationDuration - 0.5f)
            .SetEase(Ease.InExpo)
            .OnComplete(() => StartCoroutine(AnimateExperienceText(expToAdd, result))));
    }

    private IEnumerator AnimateExperienceText(float expToAdd, RectTransform result)
    {
        var fromValue = TotalExperiencePoints * 100;
        var endValue = (TotalExperiencePoints + expToAdd) * 100;
        var duration = expToAdd / _experienceToLevelUp * experienceGainAnimationMaxDuration;
        if (!GameManager.CanPause)
        {
            // if we can't pause, that means some other exp ball already on its way to trigger leveling up
            // used to prevent a visual bug
            yield return new WaitUntil(() => GameManager.CanPause);
            experiencePercentageText.DOCounter(
                0,
                (int) ((endValue - _previousExperienceToLevelUp * 100) / 
                                                      (_experienceToLevelUp - _previousExperienceToLevelUp)) - 100,
                duration);
        }
        else
        {
            experiencePercentageText.DOCounter(
                (int) ((fromValue - _previousExperienceToLevelUp * 100) / 
                                                      (_experienceToLevelUp - _previousExperienceToLevelUp)),
                (int) ((endValue - _previousExperienceToLevelUp * 100) / 
                                                      (_experienceToLevelUp - _previousExperienceToLevelUp)),
                duration);
        }
        TotalExperiencePoints += expToAdd;

        currentExperienceText.DOCounter((int) fromValue, (int) endValue, duration);

        StartCoroutine(FillExperienceBar());
        result.gameObject.SetActive(false);
        _experienceCircles.Enqueue(result);
    }

    public int GetCurrentLevel()
    {
        return _currentLevel;
    }
}
