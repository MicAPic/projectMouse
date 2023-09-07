namespace UI
{
    public class GamepadFriendlyInputField : GamepadFriendlyButton
    {
        protected override void Awake()
        {
            currentClip = selectSoundEffect;
        }
    }
}
