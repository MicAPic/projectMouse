using Enemy;

public class EnemyHealth : Health
{
    private EnemyController _enemyController;
    void Awake()
    { 
        _enemyController = GetComponent<EnemyController>();
    }

    protected override void Die()
    {
        base.Die();
        ExperienceManager.Instance.AddExperience(_enemyController.experiencePointWorth);
    }
}