using DG.Tweening;

namespace Enemy
{
    public class TutorialEnemySimpleController : EnemySimpleController
    {
        protected override void Awake()
        {
            base.Awake();
            
            _spriteRenderer.material.SetFloat("_Threshold", 1.01f);
            _spriteRenderer.material.DOFloat(0.0f, "_Threshold", dissolveDuration * 2.22f);
        }
    }
}
