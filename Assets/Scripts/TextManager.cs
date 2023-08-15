using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using TMPro;
using UI;
using UnityEngine;

public class TextManager : MonoBehaviour
{
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
    private Story _story;
    
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
    
    // private bool _isPlaying;
    private bool _isDisplayingRichText;
    private int _maxLineLength;
    private bool _canContinue;

    // Start is called before the first frame update
    // void Start()
    // {
    //     
    // }

    public void StartDialogue()
    {
        _story = new Story(inkScript.text);

        StartCoroutine(WaitBeforeDisplayingText());
    }
    
    private IEnumerator WaitBeforeDisplayingText()
    {
        yield return new WaitForSeconds(1.05f);
        ContinueStory();
        // _isPlaying = true;
    }
    
    private void ContinueStory()
    {
        if (_story.canContinue)
        {
            string nextLine = _story.Continue();
            HandleTags(_story.currentTags);
            StartCoroutine(DisplayLine(nextLine));
        }
        else
        {
            transitionController.TransitionAndLoadScene(nextSceneName);
        }
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
                ? new WaitForSeconds(autoModeWaitTime)
                : new WaitForSeconds(textSpeed);
        }

        StartCoroutine(FinishDisplayingLine());
    }

    private IEnumerator FinishDisplayingLine()
    {
        _isDisplayingRichText = false;
        if (_canContinue)
        {
            _canContinue = false;
            yield return new WaitForSeconds(autoModeWaitTime);
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
                    _canContinue = true;
                    break;
                default:
                    Debug.LogWarning("Given tag is not implemented:" + key);
                    break;
            }
        }
    }
}
