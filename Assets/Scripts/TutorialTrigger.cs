using HealthControllers;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] 
    private TutorialManager tutorialManager;
    private Collider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (tutorialManager.story.currentChoices.Count == 0 || !col.CompareTag("Player")) return;
        
        _collider.enabled = false;
        var playerHealth = col.GetComponent<PlayerHealth>();
        tutorialManager.story.ChooseChoiceIndex(playerHealth.GetCurrentHealth() < playerHealth.MaxHealth ? 0 : 1);
        tutorialManager.ContinueStory();

        enabled = false;
    }
}
