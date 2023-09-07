using System;
using DG.Tweening;
using Enemy;
using HealthControllers;
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
    private InputAction fastForwardInputAction;
    [SerializeField]
    private float fastForwardTextSpeed = 0.04f;
    private float _fastForwardAutoModeWaitTime;
    private float _defaultTextSpeed;
    private float _defaultAutoModeWaitTime;
    
    private TutorialUI _tutorialUI;

    private Action<InputAction.CallbackContext> _inputHandler;
    private GameObject _miguel;

    protected override void Awake()
    {
        base.Awake();
        _tutorialUI = FindObjectOfType<TutorialUI>();
        _defaultTextSpeed = textSpeed;
        _defaultAutoModeWaitTime = autoModeWaitTime;
        _fastForwardAutoModeWaitTime = autoModeWaitTime / 2.0f;
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
            autoModeWaitTime = _fastForwardAutoModeWaitTime;
        }
        else if (fastForwardInputAction.WasReleasedThisFrame())
        {
            textSpeed = _defaultTextSpeed;
            autoModeWaitTime = _defaultAutoModeWaitTime;
        }
    }

    public override void StartDialogue()
    {
        base.StartDialogue();
        story.BindExternalFunction("UnlockControls", () =>
        {
            playerInput.enabled = true;
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
    }
}
