using HealthControllers;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] 
    private float damage;
    [SerializeField] 
    private int poolIndex;
    private Rigidbody2D _rb;

    void Awake()
    {
        _rb = GetComponentInChildren<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.TryGetComponent(out Health healthController))
        {
            healthController.TakeDamage(damage);
        }
        Disable();
    }

    public void Enable(Vector3 direction, float firePower, float damageToDeal, float scaleModifer=1.0f)
    {
        var normalizedDirection = (Vector2) direction;
        normalizedDirection.Normalize();
        
        transform.localScale = Vector3.one * scaleModifer; 
        
        damage = damageToDeal;
        gameObject.SetActive(true);
        _rb.AddForce(normalizedDirection * firePower, ForceMode2D.Impulse);
        
        //TODO: if we make the levels with no borders, call Disable on a timer
    }

    public void EnableWithoutForce(float damageToDeal)
    {
        damage = damageToDeal;
        gameObject.SetActive(true);
    }

    private void Disable()
    {
        BulletPool.Instance.ObjectPools[poolIndex].Enqueue(this);
        gameObject.SetActive(false);
    }
}
