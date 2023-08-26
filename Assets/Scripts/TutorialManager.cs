using System;
using Enemy;
using HealthControllers;
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

    private Action<InputAction.CallbackContext> _inputHandler;
    private GameObject _miguel;

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
            eventSystem.SetActive(true);
            FindFirstObjectByType<Button>().Select();
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
        story.BindExternalFunction("EngendrarMiguel", () =>
        {
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
