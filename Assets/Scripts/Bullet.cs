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

    public void Enable(Vector2 direction, float firePower, float damageToDeal)
    {
        damage = damageToDeal;
        gameObject.SetActive(true);
        _rb.AddForce(direction * firePower, ForceMode2D.Impulse);
        
        //TODO: if we make the levels with no borders, call Disable on a timer
    }

    private void Disable()
    {
        BulletPool.Instance.ObjectPools[poolIndex].Enqueue(this);
        gameObject.SetActive(false);
    }
}
