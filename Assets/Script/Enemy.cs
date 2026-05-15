using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Configurações de Vida")]
    [SerializeField] private int maxHealth = 3;
    private int _currentHealth;

    [Header("Configurações de Ataque")]
    [SerializeField] private int contactDamage = 1;

    [Header("Movimentação (Patrulha)")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Transform[] waypoints;
    private int _currentWaypointIndex = 0;

    [Header("Knockback")]
    [SerializeField] private Vector2 forcaKnockback = new Vector2(7f, 5f);
    [SerializeField] private float tempoKnockback = 0.2f;
    private bool _isKnockedBack; // Trava a patrulha enquanto apanha

    [Header("Feedback Visual")]
    [SerializeField] private GameObject deathEffect;
    private SpriteRenderer _sr;
    private Color _originalColor;

    private Rigidbody2D _rb;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
        _originalColor = _sr.color;
        _currentHealth = maxHealth;
    }

    private void Update()
    {
        // Só continua andando se NÃO estiver sofrendo knockback
        if (!_isKnockedBack)
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        // Se não houver waypoints configurados, o inimigo fica parado
        if (waypoints == null || waypoints.Length < 2) return;

        Transform target = waypoints[_currentWaypointIndex];
        transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        // Verifica se chegou no ponto de destino
        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            _currentWaypointIndex = (_currentWaypointIndex + 1) % waypoints.Length;
            Flip();
        }
    }

    private void Flip()
    {
        // Mantendo o padrão de rotação 180 no eixo Y que você usa no Player
        transform.Rotate(0, 180, 0);
    }

    // DETECÇÃO DE DANO AO PLAYER (CONTATO)
    // No script Enemy.cs
    // Troque OnTriggerStay2D por este:
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Tenta pegar no objeto que bateu OU nos pais/filhos
            IDamageable playerVida = collision.gameObject.GetComponentInParent<IDamageable>();

            if (playerVida != null)
            {
                playerVida.TakeDamage(1);
                Debug.Log("Mandei o comando de dano!");
            }
            else
            {
                Debug.Log("Achei o Player, mas ele não tem o script IDamageable!");
            }
        }
    }

    // IMPLEMENTAÇÃO DA INTERFACE IDAMAGEABLE (O Inimigo recebendo dano)
    // IMPLEMENTAÇÃO DA INTERFACE IDAMAGEABLE (O Inimigo recebendo dano)
    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;

        // Feedback de flash vermelho
        StopCoroutine(nameof(DamageFlash));
        StartCoroutine(nameof(DamageFlash));

        if (_currentHealth <= 0)
        {
            // 1. Procuramos o PlayerMovement REAL que está rodando na fase
            PlayerMovement playerReal = Object.FindFirstObjectByType<PlayerMovement>();

            if (playerReal != null)
            {
                // 2. Avisamos o Player REAL que ele garantiu mais um abate
                playerReal.RegistrarMorte();
            }

            // 3. Chama o seu método de morte (para soltar partículas e dar o Destroy)
            Die();
        }
        else
        {
            // Se tomou dano e sobreviveu, aplica o Knockback
            StartCoroutine(RotinaKnockback());
        }
    }

    private IEnumerator RotinaKnockback()
    {
        _isKnockedBack = true;

        if (_rb != null)
        {
            // Zera a velocidade atual para o pulo não bugar
            _rb.linearVelocity = Vector2.zero;

            // Procura o Player na cena para saber de qual lado o soco veio
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                // Se o inimigo está à DIREITA do player, empurra pra Direita (1). Senão, Esquerda (-1).
                float direcao = transform.position.x > player.transform.position.x ? 1f : -1f;

                // Aplica o empurrão!
                _rb.AddForce(new Vector2(direcao * forcaKnockback.x, forcaKnockback.y), ForceMode2D.Impulse);
            }
        }

        // Espera o tempo configurado
        yield return new WaitForSeconds(tempoKnockback);

        // Zera o movimento de novo pra ele não continuar escorregando como se fosse gelo
        if (_rb != null) _rb.linearVelocity = Vector2.zero;

        _isKnockedBack = false;
    }

    private IEnumerator DamageFlash()
    {
        _sr.color = new Color(2f, 2f, 2f, 0.9f);
        yield return new WaitForSeconds(0.3f);
        _sr.color = _originalColor;
    }

    private void Die()
    {
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}