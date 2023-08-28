using Enemy;

namespace HealthControllers
{
    public class EnemyWithMagicHealth : Health
    {
        private EnemyWithMagicController _enemyController;
        void Awake()
        {
            _enemyController = GetComponent<EnemyWithMagicController>();
        }

        protected override void Die()
        {
            if (SpawnManager.Instance != null)
                SpawnManager.Instance.EnemyDeath();
            ExperienceManager.Instance.AddExperience(_enemyController.experiencePointWorth);
            _enemyController.EnableAllBullets();
            _enemyController.Dissolve();
            enabled = false;
        }
    }
}
