using Audio;
using Enemy;
using UnityEngine;

namespace HealthControllers
{
    public class EnemyWithMagicHealth : Health
    {
        [SerializeField]
        private AudioClip hitSoundEffect;
        private EnemyWithMagicController _enemyController;
        void Awake()
        {
            _enemyController = GetComponent<EnemyWithMagicController>();
        }
        
        public override void TakeDamage(float damagePoints)
        {
            base.TakeDamage(damagePoints);
            AudioManager.Instance.sfxSource.PlayOneShot(hitSoundEffect);
            _enemyController.Flash();
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
