using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "ChatPlayerInfo", menuName = "ScriptableObject/ChatPlayerInfo", order = 1)]
    public class ChatPlayerInfo : ScriptableObject
    {
        public string[] nicknames;
        public Color[] nicknameColours;
    }
}
