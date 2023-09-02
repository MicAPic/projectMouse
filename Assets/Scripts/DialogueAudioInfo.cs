using UnityEngine;

[CreateAssetMenu(fileName = "DialogueAudioInfo", menuName = "ScriptableObject/DialogueAudioInfo", order = 1)]
public class DialogueAudioInfo : ScriptableObject
{
    public string id;
    public AudioClip[] typingAudioClips;
    [Range(-3, 3)]
    public float minPitch = 0.5f;
    [Range(-3, 3)]
    public float maxPitch = 0.5f;
}
