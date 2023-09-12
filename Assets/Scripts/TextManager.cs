using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using TMPro;
using UI;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    public static TextManager Instance { get; private set; }
    
    public GameObject eventSystem;

    [Header("Parameters")] 
    [SerializeField]
    protected float initialDelay = 1.05f;
    [SerializeField]
    protected float textSpeed = 0.08f;
    [SerializeField]
    protected float autoModeWaitTime = 1.25f;
    [SerializeField] 
    private string nextSceneName;
    
    [Header("UI")]
    [SerializeField] 
    private TMP_Text dialogueText;
    [SerializeField] 
    private TransitionController transitionController;
    
    [Header("Ink")]
    [SerializeField] 
    private TextAsset inkScript;
    public Story story;
    
    private const string AutoTag = "auto";
    private const string PortraitTag = "portrait";
    
    [Header("Audio")]
    [SerializeField]
    private DialogueAudioInfo audioInfo;
    [Range(1, 5)]
    [SerializeField] 
    protected int frequencyLevel = 2;
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private bool stopAudioSource;
    
    [Header("Speaker Data")]
    protected Dictionary<string, Sprite> speakerSpriteDictionary;
    
    public bool isPlaying;
    protected bool canContinue;
    protected bool isDisplayingRichText;
    private int _maxLineLength;
    protected Coroutine _currentDisplayLineCoroutine;
    private Coroutine _currentFinishDisplayLineCoroutine;

    protected virtual void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public virtual void StartDialogue()
    {
        story = new Story(inkScript.text);

        StartCoroutine(WaitBeforeDisplayingText());
    }

    public void ContinueStory()
    {
        if (_currentFinishDisplayLineCoroutine != null)
        {
            StopCoroutine(_currentFinishDisplayLineCoroutine);
        }
        
        if (story.canContinue)
        {
            string nextLine = story.Continue();
            HandleTags(story.currentTags);

            if (isPlaying)
            {
                StopCoroutine(_currentDisplayLineCoroutine);
            }
            isPlaying = true;
            _currentDisplayLineCoroutine = StartCoroutine(DisplayLine(nextLine));
        }
        else
        {
            if (transitionController != null)
            {
                Transition();
            }
            else
            {
                TransitionController.Instance.TransitionAndLoadScene(nextSceneName);
            }
        }
    }

    public void Transition()
    {
        transitionController.TransitionAndLoadScene(nextSceneName);
    }

    protected virtual IEnumerator FinishDisplayingLine()
    {
        _currentDisplayLineCoroutine = null;
        
        isPlaying = false;
        isDisplayingRichText = false;
        if (canContinue)
        {
            canContinue = false;
            yield return new WaitForSecondsRealtime(autoModeWaitTime);
            ContinueStory();
        }
    }

    protected virtual IEnumerator WaitBeforeDisplayingText()
    {
        yield return new WaitForSeconds(initialDelay);
        ContinueStory();
        // _isPlaying = true;
    }

    protected virtual void SwitchPortrait(string title)
    {
        Debug.LogWarning("Function should not be called in this context");
    }

    private IEnumerator DisplayLine(string line)
    {
        dialogueText.maxVisibleCharacters = 0;
        dialogueText.text = line;
        _maxLineLength = line.Length;

        for (var i = 0; i < _maxLineLength; i++)
        {
            // rich text
            switch (dialogueText.text[i])
            {
                case '<':
                    isDisplayingRichText = true;
                    break;
                case '>':
                    isDisplayingRichText = false;
                    _maxLineLength--;
                    break;
            }
        
            if (isDisplayingRichText)
            {
                _maxLineLength--;
            }
            
            PlayDialogueSound(i, dialogueText.text[i]);
            dialogueText.maxVisibleCharacters++;
            yield return dialogueText.text[i] == ' ' && dialogueText.text[i - 1] == '.'
                ? new WaitForSecondsRealtime(autoModeWaitTime)
                : new WaitForSecondsRealtime(textSpeed);
            
            if (GameManager.IsPaused)
            {
                yield return new WaitUntil(() => !GameManager.IsPaused);
            }
        }

        _currentFinishDisplayLineCoroutine = StartCoroutine(FinishDisplayingLine());
    }

    private void PlayDialogueSound(int currentLineLength, char currentCharacter)
    {
        if (currentLineLength % frequencyLevel != 0 || audioInfo == null) return;
        if (!char.IsLetter(currentCharacter)) return;
        
        var typingAudioClips = audioInfo.typingAudioClips;
        var minPitch = audioInfo.minPitch;
        var maxPitch = audioInfo.maxPitch;
    
        if (stopAudioSource)
        {
            audioSource.Stop();
        }
    
        // clip
        var characterHash = currentCharacter.GetHashCode();
        var audioClip = typingAudioClips[characterHash % typingAudioClips.Length];
        
        // pitch
        var maxPitchInt = (int) maxPitch * 100;
        var minPitchInt = (int) minPitch * 100;
        var pitchRange = maxPitchInt - minPitchInt;
        if (pitchRange != 0)
        {
            audioSource.pitch = (characterHash % pitchRange + minPitchInt) / 100f; 
        }
        else
        {
            audioSource.pitch = minPitch;
        }
        
        audioSource.PlayOneShot(audioClip);
    }

    private void HandleTags(List<string> currentTags)
    {
        foreach (var tag in currentTags)
        {
            string[] pair = tag.Split(':');
            if (pair.Length != 2)
            {
                Debug.LogError("Tag couldn't be parsed:" + tag);
            }

            string key = pair[0];
            string value = pair[1];

            switch (key)
            {
                case AutoTag:
                    canContinue = true;
                    break;
                case PortraitTag:
                    SwitchPortrait(value);
                    break;
                default:
                    Debug.LogWarning("Given tag is not implemented:" + key);
                    break;
            }
        }
    }
}
