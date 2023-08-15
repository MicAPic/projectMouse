using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "ChatTextInfo", menuName = "ScriptableObject/ChatTextInfo", order = 1)]
    public class ChatTextInfo : ScriptableObject
    {
        public string[] messages;
    }
}
