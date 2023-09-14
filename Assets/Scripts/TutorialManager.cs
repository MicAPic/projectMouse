using System;
using System.Linq;
using DG.Tweening;
using Enemy;
using HealthControllers;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TutorialManager : TextManager
{
    [Header("Tutorial")]
    [SerializeField]
    private PlayerInput playerInput;
    [SerializeField]
    private GameObject miguelPrefab;
    [SerializeField]
    private Transform miguelSpawnPoint;
    
    [Space]
    
    [SerializeField]
    private RectTransform tipPopUp;
    [SerializeField]
    private float tipPopUpDuration;
    [SerializeField]
    private float tipPopUpDelay = 0.0f;
    [SerializeField]
    private float tipToggledPosY;
    private float tipDefaultPosY;
    
    [Space]
    
    [SerializeField]
    private Sprite[] bubiSprites;
    [SerializeField]
    private Sprite[] bubiEyelids;
    [SerializeField]
    private string[] bubiSpriteNames = {"bubi_default", "bubi_smug", "bubi_surprised"};

    [Space]
    
    [SerializeField]
    private InputAction fastForwardInputAction;
    [SerializeField]
    private float fastForwardTextSpeed = 0.04f;
    [SerializeField]
    private int fastForwardSoundFrequency = 3;
    private float _fastForwardAutoModeWaitTime;
    private float _defaultTextSpeed;
    private int _defaultSoundFrequency;
    private float _defaultAutoModeWaitTime;
    
    private TutorialUI _tutorialUI;

    private Action<InputAction.CallbackContext> _inputHandler;
    private GameObject _miguel;

    protected override void Awake()
    {
        base.Awake();
        _tutorialUI = FindObjectOfType<TutorialUI>();
        _defaultTextSpeed = textSpeed;
        _defaultSoundFrequency = frequencyLevel;
        _defaultAutoModeWaitTime = autoModeWaitTime;
        _fastForwardAutoModeWaitTime = autoModeWaitTime / 2.0f;

        tipDefaultPosY = tipPopUp.anchoredPosition.y;
        
        speakerSpriteDictionary = Enumerable.Range(0, bubiSprites.Length)
                                            .ToDictionary(i => bubiSpriteNames[i], j => bubiSprites[j]);
        speakerEyelidDictionary = Enumerable.Range(0, bubiEyelids.Length)
                                            .ToDictionary(i => bubiSpriteNames[i], j => bubiEyelids[j]);
    }
    
    void OnEnable()
    {
        fastForwardInputAction.Enable();
    }
        
    void OnDisable()
    {
        fastForwardInputAction.Disable();
    }

    void Start()
    {
        StartDialogue();
    }
    
    void Update() 
    {
        // Fast-forwarding the text
        if (fastForwardInputAction.WasPressedThisFrame())
        {
            textSpeed = fastForwardTextSpeed;
            frequencyLevel = fastForwardSoundFrequency;
            autoModeWaitTime = _fastForwardAutoModeWaitTime;
        }
        else if (fastForwardInputAction.WasReleasedThisFrame())
        {
            textSpeed = _defaultTextSpeed;
            frequencyLevel = _defaultSoundFrequency;
            autoModeWaitTime = _defaultAutoModeWaitTime;
        }
    }

    public override void StartDialogue()
    {
        base.StartDialogue();
        story.BindExternalFunction("GetBindingNameFor", (string actionName) =>
        {
            return InputManager.Instance.GetBindingNameFor(actionName);
        });
        story.BindExternalFunction("UnlockControls", () =>
        {
            playerInput.SwitchCurrentActionMap("InGame");
        });
        story.BindExternalFunction("UnlockAim", () =>
        {
            CameraController.Instance.focusPoint = 0.5f;
        });
        story.BindExternalFunction("UnlockSelection", () =>
        {
            _tutorialUI.ToggleDialogueBox(false, 0.0f).OnComplete(() =>
            {
                eventSystem.SetActive(true);
                FindFirstObjectByType<Button>().Select();
            });
        });
        story.BindExternalFunction("WaitForInput", (string inputName) =>
        {
            _inputHandler = _ =>
            {
                if (isPlaying)
                {
                    canContinue = true;
                }
                else
                {
                    ContinueStory();
                }
                playerInput.actions[inputName].performed -= _inputHandler;
            };
            playerInput.actions[inputName].performed += _inputHandler;
        });
        story.BindExternalFunction("ActivateTutorialTrigger", () =>
        {
            FindObjectOfType<TutorialCollider>().Toggle();
        });
        story.BindExternalFunction("EngendrarMiguel", () =>
        {
            miguelSpawnPoint.gameObject.SetActive(false);
            _miguel = Instantiate(miguelPrefab, miguelSpawnPoint.position, Quaternion.identity);
        });
        story.BindExternalFunction("ActivarMiguel", () =>
        {
            _miguel.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            _miguel.GetComponent<EnemyController>().enabled = true;
            _miguel.GetComponent<Health>().enabled = true;
        });
        story.BindExternalFunction("ToggleTip", (bool state) =>
        {
            var tipTMPText = tipPopUp.GetComponent<TMP_Text>();
            if (state)
            {
                tipTMPText.text = tipTMPText.text.Replace(
                    "F",
                    InputManager.Instance.GetBindingNameFor("FastForward"));
                
                autoModeWaitTime = 0.0f;
                tipPopUp.DOAnchorPosY(tipToggledPosY, tipPopUpDuration).SetDelay(tipPopUpDelay);
                tipTMPText.DOFade(1.0f, tipPopUpDuration).SetDelay(tipPopUpDelay);
            }
            else
            {
                autoModeWaitTime = _defaultAutoModeWaitTime;
                tipPopUp.DOAnchorPosY(tipDefaultPosY, tipPopUpDuration);
                tipTMPText.DOFade(0.0f, tipPopUpDuration);
            }
        });
    }

    protected override void SwitchPortrait(string title)
    {
        _tutorialUI.bubiImage.sprite = speakerSpriteDictionary[title];
        _tutorialUI.bubiEyelids.sprite = speakerEyelidDictionary[title];
    }
}
