using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWithMagicHealth : Health
{
    private EnemyWithMagicController _enemyController;
    void Awake()
    {
        _enemyController = GetComponent<EnemyWithMagicController>();
    }

    protected override void Die()
    {
        base.Die();
        ExperienceManager.Instance.AddExperience(_enemyController.experiencePointWorth);
        _enemyController.EnableAllBullets();
    }
}
