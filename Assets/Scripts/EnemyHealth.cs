public class EnemyHealth : Health
{
    // TODO: uncomment everything when the Enemy class is added
    // private Enemy _enemy;
    // void Awake()
    // { 
    //     _enemy = GetComponent<Enemy>();
    // }

    protected override void Die()
    {
        base.Die();
        // ExperienceManager.Instance.AddExperience(_enemy.experiencePointWorth);
    }
}