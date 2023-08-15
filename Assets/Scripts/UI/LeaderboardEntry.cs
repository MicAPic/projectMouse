using TMPro;
using UnityEngine;

namespace UI
{
    public class LeaderboardEntry : MonoBehaviour
    {
        [SerializeField] 
        private TMP_Text nicknameField;
        [SerializeField] 
        private TMP_Text scoreField;

        public void PopulateEntryFields(string nickname, int score)
        {
            nicknameField.text = nickname;
            scoreField.text = score.ToString();
        }
    }
}
