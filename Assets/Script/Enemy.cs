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

    [Header("Feedback Visual")]
    [SerializeField] private GameObject deathEffect;
    private SpriteRenderer _sr;
    private Color _originalColor;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _originalColor = _sr.color;
        _currentHealth = maxHealth;
    }

    private void Update()
    {
        Patrol();
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
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth player = collision.GetComponent<PlayerHealth>();
            if (player != null)
            {
                // Aplica dano e empurra o player para longe da posição deste inimigo
                player.TakeDamage(0);
            }
        }
    }

    // IMPLEMENTAÇÃO DA INTERFACE IDAMAGEABLE (O Inimigo recebendo dano)
    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;

        // Feedback de flash vermelho
        StopCoroutine(nameof(DamageFlash));
        StartCoroutine(nameof(DamageFlash));

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator DamageFlash()
    {
        _sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
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