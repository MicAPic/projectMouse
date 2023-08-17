namespace HealthControllers
{
    public class TutorialEnemyHealth : EnemyHealth
    {
        protected override void Die()
        {
            base.Die();
            TextManager.Instance.ContinueStory();
        }
    }
}