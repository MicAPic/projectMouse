using Enemy;

namespace HealthControllers
{
    public class EnemyHealth : Health
    {
        private EnemyController _enemyController;
        void Awake()
        { 
            _enemyController = GetComponent<EnemyController>();
        }

        private void Start()
        {
            if (SpawnManager.Instance != null)
            {
                healthPoints = SpawnManager.Instance.GetEnemyHealth();
            }
            else
            {
                healthPoints = 100.0f;
            }
        }

        protected override void Die()
        {
            ExperienceManager.Instance.AddExperience(_enemyController.experiencePointWorth);
            if (SpawnManager.Instance != null)
                SpawnManager.Instance.EnemyDeath();
            _enemyController.Dissolve();
            enabled = false;
        }
    }
}