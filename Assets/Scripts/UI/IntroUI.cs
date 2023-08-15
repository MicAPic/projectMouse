using UnityEngine;

namespace UI
{
    public class IntroUI : UI
    {
        [SerializeField]
        private TextManager introTextManager;
        
        void Start()
        {
            introTextManager.StartDialogue();
        }
    }
}
