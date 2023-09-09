using UnityEngine.EventSystems;

namespace UI
{
    public class GamepadFriendlyInputField : GamepadFriendlyButton
    {
        protected override void Awake()
        {
            currentClip = selectSoundEffect;
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            
        }
    }
}
