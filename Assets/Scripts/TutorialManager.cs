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
    private TutorialUI tutorialUI;

    private Action<InputAction.CallbackContext> _inputHandler;
    private GameObject _miguel;

    protected override void Awake()
    {
        base.Awake();
        tutorialUI = FindObjectOfType<TutorialUI>();
    }

    void Start()
    {
        StartDialogue();
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
            tutorialUI.ToggleDialogueBox(false, 0.0f).OnComplete(() =>
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
