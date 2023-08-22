using System.Collections;
using System.Threading.Tasks;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UI
{
    public class ChatManager : MonoBehaviour
    {
        public static ChatManager Instance { get; private set; }
        
        public enum CommentType
        {
            Random,
            Donation
        }

        [SerializeField]
        private bool generateComments = true;

        [Header("UI & Animation")]
        [Tooltip("Delay between comments in seconds")]
        public float commentDelay;
        [SerializeField] 
        private float spedUpCommentDelay;
        [SerializeField] 
        private GameObject commentPrefab;
        [SerializeField] 
        private Transform chatContainer;
        [SerializeField] 
        private int maxCommentsOnScreen = 16;
        private GameObject _queuedComment;
        private float _defaultCommentDelay;
        
        [Header("Data")]
        [SerializeField] 
        private ChatPlayerInfo chatPlayerInfo;
        [SerializeField] 
        private ChatTextInfo generalChatTextInfo;
        [SerializeField] 
        private ChatTextInfo startChatTextInfo;
        [SerializeField] 
        private ChatTextInfo gameOverChatTextInfo;
        [SerializeField] 
        private ChatTextInfo levelUpChatTextInfo;
        [SerializeField] 
        private ChatTextInfo criticalHealthChatTextInfo;

        private ChatTextInfo _currentCTI;

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            _defaultCommentDelay = commentDelay;
        }

        // Start is called before the first frame update
        void Start()
        {
            _currentCTI = startChatTextInfo;
            commentDelay = spedUpCommentDelay;
            
            if (!generateComments) return;

            TransitionToChatInfo(generalChatTextInfo, 10.0f, _defaultCommentDelay);
            StartCoroutine(ShowRandomComments());
        }

        // Update is called once per frame
        // void Update()
        // {
        //
        // }
        
        public void DisplayComment(CommentType commentType=CommentType.Random, string additionalInfo = "")
        {
            GameObject comment;
            if (_queuedComment == null)
            {
                comment = Instantiate(commentPrefab, chatContainer);
            }
            else
            {
                comment = _queuedComment;
                _queuedComment.SetActive(true);
            }
            
            GenerateCommentContent(comment, commentType, additionalInfo);

            if (chatContainer.childCount > maxCommentsOnScreen)
            {
                var queuedCommentTransform = chatContainer.GetChild(0);
                queuedCommentTransform.SetAsLastSibling();
                _queuedComment = queuedCommentTransform.gameObject;
                _queuedComment.SetActive(false);
            }
        }

        public void EnableLevelUpChatInfo()
        {
            TransitionToChatInfo(levelUpChatTextInfo, 0.0f, spedUpCommentDelay);
        }
        
        public void EnableGeneralChatInfo()
        {
            TransitionToChatInfo(generalChatTextInfo, 0.0f, _defaultCommentDelay);
        }
        
        public void EnableGameOverChatInfo()
        {
            TransitionToChatInfo(gameOverChatTextInfo, 0.0f, spedUpCommentDelay);
        }
        
        public void ToggleCriticalHealthChatInfo()
        {
            (generalChatTextInfo, criticalHealthChatTextInfo) = (criticalHealthChatTextInfo, generalChatTextInfo);
            EnableGeneralChatInfo();
        }

        private IEnumerator ShowRandomComments()
        {
            yield return new WaitForSecondsRealtime(commentDelay);
            DisplayComment();
            StartCoroutine(ShowRandomComments());
        }
        
        private void GenerateCommentContent(GameObject comment, CommentType commentType=CommentType.Random, string additionalInfo = "")
        {
            var tmp = comment.GetComponent<TMP_Text>();
            var randomIndex = Random.Range(0, chatPlayerInfo.nicknames.Length);

            var nickname = chatPlayerInfo.nicknames[randomIndex];
            var colourCode = ColorUtility.ToHtmlStringRGBA(chatPlayerInfo.nicknameColours[randomIndex]);
            
            string message;
            switch (commentType)
            {
                case CommentType.Donation:
                    message = $" just donated ${additionalInfo}";
                    break;
                case CommentType.Random:
                default:
                    message = ": " + _currentCTI.messages[Random.Range(0, _currentCTI.messages.Length)];
                    break;
            }

            tmp.text = $"<color=#{colourCode}>{nickname}</color>{message}";
        }

        private async void TransitionToChatInfo(ChatTextInfo chatTextInfo, float transitionTime, float newChatSpeed)
        {
            await Task.Delay((int)(transitionTime * 1000));
            _currentCTI = chatTextInfo;
            commentDelay = newChatSpeed;
        }
    }
}
