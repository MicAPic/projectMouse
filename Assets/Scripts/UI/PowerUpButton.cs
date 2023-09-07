using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class PowerUpButton : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        [SerializeField]
        private GameObject outline;

        public void OnSelect(BaseEventData eventData)
        {
            outline.SetActive(true);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            outline.SetActive(false);
        }
    }
}
