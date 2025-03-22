using UnityEngine;

public class DamageSource : MonoBehaviour
{
    [SerializeField] private int damageAmount = 1;

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("HIT!");
        if (collision.gameObject.GetComponent<EnemyAI>())
        {
            Debug.Log("HIT!");
            EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
            enemyHealth.TakeDamage(damageAmount);
        }       
    }
}
