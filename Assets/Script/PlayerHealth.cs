using UnityEngine;


public class PlayerHealth : MonoBehaviour, IDamageable
{

    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        //Colocar UI
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy)
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Morreu!");
        Destroy(gameObject);
    }
}