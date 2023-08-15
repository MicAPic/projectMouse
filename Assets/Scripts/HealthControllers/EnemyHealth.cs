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
            healthPoints = SpawnManager.Instance.GetEnemyHealth();
        }

        protected override void Die()
        {
            base.Die();
            ExperienceManager.Instance.AddExperience(_enemyController.experiencePointWorth);
            SpawnManager.Instance.EnemyDeath();
        }
    }
}