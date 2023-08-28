using DG.Tweening;
using UnityEngine;

namespace Enemy
{
    public class TutorialEnemySimpleController : EnemySimpleController
    {
        protected override void Awake()
        {
            base.Awake();
            
            var defaultMaterial = _spriteRenderer.material;
            _spriteRenderer.material = new Material(dissolveMaterial);
            _spriteRenderer.material.SetFloat("_Threshold", 1.01f);
            _spriteRenderer.material.DOFloat(0.39f, "_Threshold", dissolveDuration * 2.0f)
                .OnComplete(() =>
                {
                    _spriteRenderer.material = defaultMaterial;
                });
        }
    }
}
