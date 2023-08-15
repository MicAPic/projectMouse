using Dan.Main;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class InGameUI : UI
    {
        public GameObject pauseScreen;
        public GameObject gameOverScreen;
        public TMP_Text scoreText;
        public TMP_Text highScoreText;
        public TMP_InputField nicknameInputField;
        public Button[] buttons;
        [Space]
        [SerializeField]
        private GameObject offlineSign;
        [SerializeField]
        private LeaderboardEntry[] leaderboardEntries;

        void Start()
        {
            nicknameInputField.onSubmit.AddListener(_ => GameManager.Instance.SubmitHighScore());
            buttons[0].onClick.AddListener(GameManager.Instance.SubmitHighScore);
        }
        
        public void ToggleOfflineMode()
        {
            buttons[0].interactable = false;
            nicknameInputField.interactable = false;
            offlineSign.SetActive(true);
        }
        
        public void ToggleButtons(bool state)
        {
            nicknameInputField.interactable = state;
            foreach (var button in buttons)
            {
                button.interactable = state;
            }
        }

        public void UpdateLeaderboardContent(string publicKey)
        {
            LeaderboardCreator.GetLeaderboard(publicKey, entries =>
            {
                var numberOfEntries = entries.Length < leaderboardEntries.Length 
                    ? entries.Length 
                    : leaderboardEntries.Length;

                for (var i = 0; i < numberOfEntries; i++)
                {
                    leaderboardEntries[i].PopulateEntryFields(entries[i].Username, entries[i].Score);
                }
            });
        }
    }
}
