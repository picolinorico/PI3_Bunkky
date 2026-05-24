using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int currentHealth;
    private SpriteRenderer _sr;
    private Color _originalColor;

    [Header("Conexões")]
    public VidaUI controleDeUI; // Para os corações
    private PlayerMovement playerMovement; // Para o knockback e checkpoint
    public TransicaoMorte telaDeMorte;

    private CinemachineImpulseSource impulseSource;

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _originalColor = _sr.color;
    }

    void Start()
    {
        currentHealth = maxHealth;
        playerMovement = GetComponent<PlayerMovement>();

        // PUXA O COMPONENTE AQUI:
        impulseSource = GetComponent<CinemachineImpulseSource>();

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

        if (playerMovement != null && !playerMovement.enabled) return;

        if (playerMovement != null && playerMovement.isKnockback) return;

        currentHealth -= damage;
        StopCoroutine(nameof(DamageFlash)); // Com S maiúsculo
        StartCoroutine(nameof(DamageFlash)); // Com S maiúsculo
        Debug.Log("Vida restante: " + currentHealth);

        if (controleDeUI != null) controleDeUI.AtualizarCoracoes(currentHealth, maxHealth);

        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse(); // BOOM! Tela treme.
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else if (playerMovement != null)
        {
            playerMovement.AplicarKnockback();
        }
    }

    private void Die()
    {
        Debug.Log("Morreu!");

        // Zera a vida 
        currentHealth = maxHealth;
        if (controleDeUI != null) controleDeUI.AtualizarCoracoes(currentHealth, maxHealth);

        // Chama a transição (e ela vai cuidar do resto)
        if (telaDeMorte != null)
        {
            telaDeMorte.IniciarTransicao(playerMovement);
        }
        else
        {
            if (playerMovement != null) playerMovement.Respawnar();
        }
    }

    private IEnumerator DamageFlash()
    {
        _sr.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        _sr.color = _originalColor;
    }
}