using System.Collections;
using ScriptableObjects;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ChatManager : MonoBehaviour
    {
        public static ChatManager Instance { get; private set; }

        [Tooltip("Delay between comments in seconds")]
        public float commentDelay;
        [SerializeField] 
        private ChatTextInfo chatTextInfo;
        [SerializeField] 
        private GameObject commentPrefab;
        [SerializeField] 
        private Transform chatContainer;
        [SerializeField] 
        private int maxCommentsOnScreen = 16;
        private GameObject _queuedComment;

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
            StartCoroutine(GenerateComments());
        }

        // Update is called once per frame
        // void Update()
        // {
        //
        // }

        private IEnumerator GenerateComments()
        {
            yield return new WaitForSeconds(commentDelay);
            DisplayRandomComment();
            StartCoroutine(GenerateComments());
        }

        private void DisplayRandomComment()
        {
            if (_queuedComment == null)
            {
                var newComment = Instantiate(commentPrefab, chatContainer);
                PopulateCommentContent(newComment);
            }
            else
            {
                _queuedComment.SetActive(true);
                PopulateCommentContent(_queuedComment);
            }

            if (chatContainer.childCount > maxCommentsOnScreen)
            {
                var queuedCommentTransform = chatContainer.GetChild(0);
                queuedCommentTransform.SetAsLastSibling();
                _queuedComment = queuedCommentTransform.gameObject;
                _queuedComment.SetActive(false);
            }
        }

        private void PopulateCommentContent(GameObject comment)
        {
            var tmp = comment.GetComponent<TMP_Text>();
            var randomIndex = Random.Range(0, chatTextInfo.nicknames.Length);

            var nickname = chatTextInfo.nicknames[randomIndex];
            var colourCode = ColorUtility.ToHtmlStringRGBA(chatTextInfo.nicknameColours[randomIndex]);
            var message = chatTextInfo.messages[Random.Range(0, chatTextInfo.messages.Length)];

            tmp.text = $"<color=#{colourCode}>{nickname}</color>: {message}";
        }
    }
}
