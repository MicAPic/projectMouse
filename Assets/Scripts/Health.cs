using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField]
    private float healthPoints;

    // Start is called before the first frame update
    // void Start()
    // {
    //     
    // }

    // Update is called once per frame
    // void Update()
    // {
    //     
    // }

    public float GetCurrentHealth()
    {
        return healthPoints;
    }

    public virtual void TakeDamage(float damagePoints)
    {
        healthPoints -= damagePoints;
        if (healthPoints <= 0)
        {
            Die();
        }
        
        //TODO: animate taking damage here
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
