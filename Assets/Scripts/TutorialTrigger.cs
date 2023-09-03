using System.Collections;
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

    private IEnumerator OnTriggerStay2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) yield break;
        
        Toggle();
        yield return new WaitForSeconds(0.1f); //small delay to make sure the player's hp is all set
        var playerHealth = col.GetComponent<PlayerHealth>();
        if (tutorialManager.story.currentChoices.Count == 0) yield break;
        tutorialManager.story.ChooseChoiceIndex(playerHealth.GetCurrentHealth() < playerHealth.MaxHealth ? 0 : 1);
        tutorialManager.ContinueStory();

        enabled = false;
    }

    public void Toggle()
    {
        _collider.enabled = !_collider.enabled;
    }
}
