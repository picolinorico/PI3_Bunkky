using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Configurações de Vida")]
    [SerializeField] private int maxHealth = 3;
    private int _currentHealth;

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

    [Header("Feedback de Morte")]
    [SerializeField] private float duracaoPiscarMorte = 1f; // Quanto tempo ele fica piscando
    [SerializeField] private float intervaloPiscar = 0.1f;  // Velocidade do pisca-pisca

    private Rigidbody2D _rb;
    private bool _isDead = false; // Evita que o inimigo morra duas vezes enquanto pisca

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
        _originalColor = _sr.color;
        _currentHealth = maxHealth;
    }

    private void Update()
    {
        // Só continua andando se NÃO estiver sofrendo knockback E não estiver morto
        if (!_isKnockedBack && !_isDead)
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        if (waypoints == null || waypoints.Length < 2) return;

        Transform target = waypoints[_currentWaypointIndex];
        transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            _currentWaypointIndex = (_currentWaypointIndex + 1) % waypoints.Length;
            Flip();
        }
    }

    private void Flip()
    {
        transform.Rotate(0, 180, 0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Se já estiver morto/piscando, não dá mais dano no player por contato
        if (_isDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            IDamageable playerVida = collision.gameObject.GetComponentInParent<IDamageable>();

            if (playerVida != null)
            {
                playerVida.TakeDamage(1);
                Debug.Log("Mandei o comando de dano!");
            }
        }
    }

    public void TakeDamage(int damage)
    {
        // Se já iniciou o processo de morte, ignora qualquer dano extra
        if (_isDead) return;

        _currentHealth -= damage;

        StopCoroutine(nameof(DamageFlash));
        StartCoroutine(nameof(DamageFlash));

        if (_currentHealth <= 0)
        {
            PlayerMovement playerReal = Object.FindFirstObjectByType<PlayerMovement>();

            if (playerReal != null)
            {
                if (gameObject.CompareTag("caramelo"))
                {
                    playerReal.RegistrarMorte();
                }
                else if (gameObject.CompareTag("urso"))
                {
                    playerReal.DesbloquearWallCling();
                }
            }

            Die();
        }
        else
        {
            StartCoroutine(RotinaKnockback());
        }
    }

    private void Die()
    {
        _isDead = true;

        // Estátua! Desativa o componente e congela o sprite onde ele estiver
        if (TryGetComponent(out Animator anim)) anim.enabled = false;

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // ======================================================================
        // CORREÇÃO DOS COLISORES FILHOS (PÉ, SENSORES, ETC.)
        // Pega todos os colisores do pai e dos filhos e desliga um por um
        Collider2D[] todosOsColisores = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in todosOsColisores)
        {
            col.enabled = false;
        }
        // ======================================================================

        if (_rb != null) _rb.bodyType = RigidbodyType2D.Kinematic;

        StartCoroutine(RotinaPiscarMorte());
        
    }


    private IEnumerator RotinaPiscarMorte()
    {
        float tempoPassado = 0f;
        bool visivel = true;

        while (tempoPassado < duracaoPiscarMorte)
        {
            visivel = !visivel;

            if (visivel)
            {
                _sr.color = _originalColor; // Cor normal
            }
            else
            {
                // Deixa o Alpha em 0 (Totalmente transparente)
                _sr.color = new Color(_originalColor.r, _originalColor.g, _originalColor.b, 0f);
            }

            yield return new WaitForSeconds(intervaloPiscar);
            tempoPassado += intervaloPiscar;
        }

        // 4. SÓ AGORA destrói o objeto de verdade da cena!
        Destroy(gameObject);
    }

    private IEnumerator RotinaKnockback()
    {
        _isKnockedBack = true;

        if (_rb != null)
        {
            _rb.linearVelocity = Vector2.zero;

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                float direcao = transform.position.x > player.transform.position.x ? 1f : -1f;
                _rb.AddForce(new Vector2(direcao * forcaKnockback.x, forcaKnockback.y), ForceMode2D.Impulse);
            }
        }

        yield return new WaitForSeconds(tempoKnockback);

        if (_rb != null) _rb.linearVelocity = Vector2.zero;

        _isKnockedBack = false;
    }

    private IEnumerator DamageFlash()
    {
        _sr.color = new Color(2f, 2f, 2f, 0.9f);
        yield return new WaitForSeconds(0.3f);
        _sr.color = _originalColor;
    }
}