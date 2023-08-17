using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using TMPro;
using UI;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    public static TextManager Instance { get; private set; }

    [Header("Parameters")] 
    [SerializeField]
    private float textSpeed = 0.08f;
    [SerializeField]
    private float autoModeWaitTime = 1.25f;
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
    
    // [Header("Audio")]
    // [SerializeField]
    // private DialogueAudioInfo audioInfo;
    // [Range(1, 5)]
    // [SerializeField] 
    // private int frequencyLevel = 2;
    // private AudioSource _audioSource;
    // [SerializeField]
    // private bool stopAudioSource;
    
    protected bool isPlaying;
    protected bool canContinue;
    private bool _isDisplayingRichText;
    private int _maxLineLength;

    void Awake()
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
        Debug.Log('?');
        if (story.canContinue)
        {
            string nextLine = story.Continue();
            HandleTags(story.currentTags);

            isPlaying = true;
            StartCoroutine(DisplayLine(nextLine));
        }
        else
        {
            if (transitionController != null)
            {
                transitionController.TransitionAndLoadScene(nextSceneName);
            }
            else
            {
                TransitionController.Instance.TransitionAndLoadScene(nextSceneName);
            }
        }
    }

    private IEnumerator WaitBeforeDisplayingText()
    {
        yield return new WaitForSeconds(1.05f);
        ContinueStory();
        // _isPlaying = true;
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
                    _isDisplayingRichText = true;
                    break;
                case '>':
                    _isDisplayingRichText = false;
                    _maxLineLength--;
                    break;
            }
        
            if (_isDisplayingRichText)
            {
                _maxLineLength--;
            }
            //
            // PlayDialogueSound(i, dialogueText.text[i]);
            dialogueText.maxVisibleCharacters++;
            yield return dialogueText.text[i] == ' ' && dialogueText.text[i - 1] == '.'
                ? new WaitForSecondsRealtime(autoModeWaitTime)
                : new WaitForSecondsRealtime(textSpeed);
        }

        StartCoroutine(FinishDisplayingLine());
    }

    private IEnumerator FinishDisplayingLine()
    {
        isPlaying = false;
        _isDisplayingRichText = false;
        if (canContinue)
        {
            canContinue = false;
            yield return new WaitForSecondsRealtime(autoModeWaitTime);
            ContinueStory();
        }
    }

    // private void PlayDialogueSound(int currentLineLength, char currentCharacter)
    // {
    //     if (currentLineLength % frequencyLevel != 0) return;
    //     
    //     var typingAudioClips = _currentAudioInfo.typingAudioClips;
    //     var minPitch = _currentAudioInfo.minPitch;
    //     var maxPitch = _currentAudioInfo.maxPitch;
    //
    //     if (stopAudioSource)
    //     {
    //         _audioSource.Stop();
    //     }
    //
    //     // clip
    //     var characterHash = currentCharacter.GetHashCode();
    //     var audioClip = typingAudioClips[characterHash % typingAudioClips.Length];
    //     
    //     // pitch
    //     var maxPitchInt = (int) maxPitch * 100;
    //     var minPitchInt = (int) minPitch * 100;
    //     var pitchRange = maxPitchInt - minPitchInt;
    //     if (pitchRange != 0)
    //     {
    //         _audioSource.pitch = (characterHash % pitchRange + minPitchInt) / 100f; 
    //     }
    //     else
    //     {
    //         _audioSource.pitch = minPitch;
    //     }
    //     
    //     _audioSource.PlayOneShot(audioClip);
    // }

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
                default:
                    Debug.LogWarning("Given tag is not implemented:" + key);
                    break;
            }
        }
    }
}
