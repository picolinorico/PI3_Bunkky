using UnityEngine;
using UnityEngine.Splines.ExtrusionShapes;

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Movimento")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed = 5;
    [SerializeField] private bool isFacingRight = true;

    [Header("Detecção de Chão")]
    [SerializeField] private bool isGrounded;
    [SerializeField] private LayerMask groundLayer;

    [Header("Detecção de Parede")]
    [SerializeField] private Transform wallCheck;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float direction = isFacingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);

        // Lança um raio para detectar parede à frente
        RaycastHit2D wallHit = Physics2D.Raycast(wallCheck.position, transform.right, 0.5f, groundLayer);

        // Lança um raio para baixo para ver se o chão acabou (evita cair no buraco)
        RaycastHit2D groundHit = Physics2D.Raycast(wallCheck.position, Vector2.down, 1.0f, groundLayer);

        // Se bater na parede OU o chão acabar, ele vira
        //if (wallHit.collider != null || groundHit.collider == null)
        //{
        //    Flip();
        //}
    }

    public void TakeDamage(int damage)
    {
        Die();
    }

    public void Die()
    {
        Debug.Log("Inimigo Morto!");
        Destroy(gameObject);
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0, 180, 0);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.TryGetComponent(out PlayerHealth player))
            {
                player.TakeDamage(1);
            }
        }
    }
}
