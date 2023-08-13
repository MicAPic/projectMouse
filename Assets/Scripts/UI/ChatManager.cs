using System.Collections;
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

        [Header("UI & Animation")]
        [Tooltip("Delay between comments in seconds")]
        public float commentDelay;
        [SerializeField] 
        private GameObject commentPrefab;
        [SerializeField] 
        private Transform chatContainer;
        [SerializeField] 
        private int maxCommentsOnScreen = 16;
        private GameObject _queuedComment;
        
        [Header("Data")]
        [SerializeField] 
        private ChatPlayerInfo chatPlayerInfo;
        [SerializeField] 
        private ChatTextInfo generalChatTextInfo;

        private ChatTextInfo _currentCTI;

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            _currentCTI = generalChatTextInfo;
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

        private IEnumerator ShowRandomComments()
        {
            yield return new WaitForSeconds(commentDelay);
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
                    message = ": " + _currentCTI.messages[Random.Range(0, generalChatTextInfo.messages.Length)];
                    break;
            }

            tmp.text = $"<color=#{colourCode}>{nickname}</color>{message}";
        }
    }
}
