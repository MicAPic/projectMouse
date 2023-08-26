using System.Collections;
using System.Collections.Generic;
using CameraShake;
using DG.Tweening;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace HealthControllers
{
    public class PlayerHealth : Health
    {
        [Header("HP")]
        public float defenceModifier = 1.0f;
        public float MaxHealth { get; private set; }
        
        [SerializeField] 
        [Tooltip("Everything below this percentage is considered low HP")]
        [Range(0, 1)]
        private float criticalThreshold = 0.1f;
        [SerializeField]
        private bool canDie = true;
        private bool _inCriticalCondition;

        [Header("UI")]
        [SerializeField] 
        private GameObject heartPrefab;
        [SerializeField] 
        private Sprite heartSprite;
        [SerializeField] 
        private Sprite brokenHeartSprite;
        [SerializeField] 
        private RectTransform heartsContainer;

        private List<Image> _currentHearts;
        private List<Image> _currentBrokenHearts;
        
        [Header("Animation")]
        [SerializeField] 
        private float shakeDuration = 0.485f;
        [SerializeField] 
        private Vector2 shakeStrength = new(0.0f, 0.004341f);
        [SerializeField] 
        private int shakeVibratio = 12;
        [SerializeField] 
        private float shakeRandomness = 0.0f;
        [Space]
        [SerializeField] 
        private float intervalBetweenHeartFills = 0.5f;
        private Sequence _fullHealSequence;

        private const int HealthPointToHeartRatio = 30;

        // Start is called before the first frame update
        void Start()
        {
            MaxHealth = healthPoints;

            _currentHearts = new List<Image>();
            _currentBrokenHearts = new List<Image>();
            for (var i = 0; i < healthPoints; i += HealthPointToHeartRatio)
            {
                AddAHeart();
            }
        }

        public void GrantInvincibility(float duration, float flashingTime)
        {
            PlayerController.Instance.ToggleInvincibilityMaterial();
            defenceModifier = 0.0f;
            
            StartCoroutine(WaitAndDisableInvincibility(duration, flashingTime));
        }

        public void FullHeal()
        {
            healthPoints = MaxHealth;

            _fullHealSequence = DOTween.Sequence();
            for (var i = _currentBrokenHearts.Count - 1; i >= 0; i--)
            {
                var localIdx = i;
                _fullHealSequence.AppendCallback(() =>
                {
                    // TODO: play a sound
                    var heartToHeal = _currentBrokenHearts[localIdx];
                    _currentBrokenHearts.RemoveAt(localIdx);
            
                    heartToHeal.sprite = heartSprite;
                    heartToHeal.SetNativeSize();

                    _currentHearts.Add(heartToHeal);
                });
                _fullHealSequence.AppendInterval(intervalBetweenHeartFills);
            }

            if (_inCriticalCondition)
            {
                ChatManager.Instance.ToggleCriticalHealthChatInfo();
            }
        }

        public void IncreaseMaxHealth(float increment)
        {
            healthPoints += increment;
            MaxHealth += increment;
            
            AddAHeart();
        }

        public override void TakeDamage(float damagePoints)
        {
            if (defenceModifier <= 0.0f) return;
            
            if (_fullHealSequence is { active: true })
            {
                _fullHealSequence.Complete();
            }
            
            if (canDie || healthPoints - damagePoints * defenceModifier > 0)
            {
                base.TakeDamage(damagePoints * defenceModifier);
                for (var i = 1; i <= damagePoints / HealthPointToHeartRatio; i++)
                {
                    BreakAHeartAt(i);
                }
            }
            else
            {
                heartsContainer.GetComponent<HorizontalLayoutGroup>().enabled = false;
                _currentHearts[0].rectTransform.DOShakeAnchorPos(shakeDuration,
                    shakeStrength,
                    shakeVibratio,
                    shakeRandomness, 
                    false, 
                    true, 
                    ShakeRandomnessMode.Harmonic
                );
            }
            
            CameraShaker.Presets.Explosion2D(rotationStrength:0.1f);

            if (healthPoints <= criticalThreshold * MaxHealth && !_inCriticalCondition && !GameManager.isGameOver)
            {
                ChatManager.Instance.ToggleCriticalHealthChatInfo();
                _inCriticalCondition = true;
            }
        }

        protected override void Die() 
        {
            GameManager.Instance.GameOver();
        }

        private void AddAHeart()
        {
            var heart = Instantiate(heartPrefab, heartsContainer);
            heart.transform.SetSiblingIndex(_currentHearts.Count);
            _currentHearts.Add(heart.GetComponent<Image>());
            
            // TODO: play a sound
        }

        private void BreakAHeartAt(int reverseIndex)
        {
            var heartToBreak = _currentHearts[^reverseIndex];
            _currentHearts.RemoveAt(_currentHearts.Count - reverseIndex);
            
            heartToBreak.sprite = brokenHeartSprite;
            heartToBreak.SetNativeSize();

            _currentBrokenHearts.Add(heartToBreak);
        }

        private IEnumerator WaitAndDisableInvincibility(float effectTime, float flashingTime)
        {
            yield return new WaitForSeconds(effectTime - flashingTime);
            
            PlayerController.Instance.ActivateFlashing(flashingTime);
            yield return new WaitUntil(() => PlayerController.Instance.isFlashing == false);
            
            PlayerController.Instance.ToggleInvincibilityMaterial();
            defenceModifier = 1.0f;
        }
    }
}