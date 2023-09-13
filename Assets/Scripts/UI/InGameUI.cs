using Dan.Main;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class InGameUI : UI
    {
        [Header("Pause")]
        public GameObject pauseScreen;
        public Button cancelButton;

        [Header("Game Over")]
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
        [SerializeField]
        private CanvasGroup statistics;
        [SerializeField]
        private TMP_Text statisticsPopUp;

        protected virtual void Start()
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
            buttons[1].Select();
        }

        public void UpdateLeaderboardContent(string publicKey, int score=0, bool updateStatistics=true)
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
                
                if (!updateStatistics) return;
                // Calculate statistics
                var percentage = 0;
                if (score != 0)
                {
                    var noOfPeople = 0;
                    for (var j = entries.Length - 1; j >= 0; j--)
                    {
                        if (score >= entries[j].Score)
                        {
                            noOfPeople++;
                        }
                        else
                        {
                            percentage = Mathf.RoundToInt(noOfPeople / (float)entries.Length * 100.0f);
                            break;
                        }
                    }
                }
                
                statisticsPopUp.DOCounter(
                    0, 
                    percentage, 
                    1.0f).SetUpdate(true).SetEase(Ease.OutCirc);
            });
        }

        public void UpdateInputField(string text)
        {
            nicknameInputField.text = text.Trim('\u001b');
        }

        public virtual void UnpauseGame()
        {
            GameManager.Instance.TogglePauseScreen();
        }
    }
}
