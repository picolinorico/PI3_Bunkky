using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int currentHealth;

    [Header("Conexões")]
    public VidaUI controleDeUI; // Para os corações
    private PlayerMovement playerMovement; // Para o knockback e checkpoint

    void Start()
    {
        currentHealth = maxHealth;
        playerMovement = GetComponent<PlayerMovement>();

        // Atualiza a UI logo que o jogo começa
        if (controleDeUI != null) controleDeUI.AtualizarCoracoes(currentHealth, maxHealth);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy)
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int damage) // Garanta que o nome e tipo batem com a interface
    {
        Debug.Log("SOU A COELHA E ESTOU EXECUTANDO TAKEDAMAGE!");

        if (playerMovement != null && playerMovement.isKnockback) return;

        currentHealth -= damage;
        Debug.Log("Vida restante: " + currentHealth);

        if (controleDeUI != null) controleDeUI.AtualizarCoracoes(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else if (playerMovement != null)
        {
            playerMovement.AplicarKnockback();
        }
    }

    public void Die()
    {
        Debug.Log("Morreu! Voltando ao checkpoint...");
        currentHealth = maxHealth; // Enche a vida

        if (controleDeUI != null) controleDeUI.AtualizarCoracoes(currentHealth, maxHealth);

        // Em vez de destruir, chama o respawn do seu script de movimento
        if (playerMovement != null) playerMovement.Respawnar();
    }
}