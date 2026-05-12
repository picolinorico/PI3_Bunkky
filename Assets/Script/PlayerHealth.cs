using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int currentHealth;
    private SpriteRenderer _sr;
    private Color _originalColor;

    [Header("Conexões")]
    public VidaUI controleDeUI; // Para os corações
    private PlayerMovement playerMovement; // Para o knockback e checkpoint

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _originalColor = _sr.color;
    }

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
        StopCoroutine(nameof(DamageFlash)); // Com S maiúsculo
        StartCoroutine(nameof(DamageFlash)); // Com S maiúsculo
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

    private IEnumerator DamageFlash()
    {
        _sr.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        _sr.color = _originalColor;
    }
}