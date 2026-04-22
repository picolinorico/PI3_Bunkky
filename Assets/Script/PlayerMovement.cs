using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private bool isFacingRight = true;

    [Header("Movimento")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float horizontalMovement;
    [SerializeField] private float bloqueioMovimentoTimer;

    [Header("Pulo")]
    [SerializeField] private float jumpForce = 25f;
    [SerializeField] private int maxJumps = 1;
    [SerializeField] private int jumpsLeft;

    [Header("Gravidade")]
    [SerializeField] private int gravityBase = 4;
    [SerializeField] private float maxFallSpeed = 50;
    [SerializeField] private float fallSpeedMultiplier = 3;

    [Header("Vida e Dano")]
    [SerializeField] private int vidaMaxima = 3;
    private int vidaAtual;
    [SerializeField] private float forcaKnockbackX = 7f;
    [SerializeField] private float forcaKnockbackY = 5f;
    [SerializeField] private float knockbackDuration = 0.3f;
    private bool isKnockback;

    [Header("Ataque e Feedback")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackColdown = 1f;
    private bool onAttack = false;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Detecção de Chão")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(4.63f, 0.2f);

    [Header("Detecção de Parede")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Vector2 wallCheckSize = new Vector2(1f, 0.1f);

    [Header("Checkpoint e Câmera")]
    private Vector2 pontoDeCheckpoint;
    private BoxCollider2D areaDoCheckpoint;
    private CameraSeguir cameraSeguir;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Faz o cache da câmera uma única vez no início
        cameraSeguir = FindAnyObjectByType<CameraSeguir>();
    }

    void Start()
    {
        vidaAtual = vidaMaxima;
        pontoDeCheckpoint = transform.position;
    }

    void Update()
    {
        rb.linearVelocity = new Vector2(horizontalMovement * speed, rb.linearVelocity.y);
        GroundCheck();
        //HandleMovement();
        ProcessGravity();
    }

    void FixedUpdate()
    {
        if (isKnockback) return;
    }

    private void ProcessGravity()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = gravityBase * fallSpeedMultiplier;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = gravityBase;
        }
    }

    //private void HandleMovement()
    //{
    //    if (bloqueioMovimentoTimer > 0)
    //    {
    //        bloqueioMovimentoTimer -= Time.deltaTime;
    //    }
    //    else
    //    {
    //        // Movimento Horizontal
    //        

    //        // Lógica de Flip centralizada
    //        if (moveInput.x > 0 && !isfacingRight) Flip();
    //        else if (moveInput.x < 0 && isfacingRight) Flip();
    //    }
    //}

    private void Flip()
    {
        if (isFacingRight && horizontalMovement < 0)
        {

        }
    }

    // --- INPUTS (New Input System) ---

    public void OnMove(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    private void GroundCheck()
    {
        if (Physics2D.OverlapBox(groundCheck.position,groundCheckSize, 0, groundLayer))
        {
            jumpsLeft = maxJumps;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (jumpsLeft > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                jumpsLeft--;
            }
        }
        else if (context.canceled)
        {
            // A MÁGICA ESTÁ AQUI: Só corta a velocidade se ela for positiva (subindo)
            if (rb.linearVelocity.y > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
            }
        }
    }

    public async void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && !onAttack)
        {
            onAttack = true;
            Debug.Log("POW!");
            Atacar();

            await Awaitable.WaitForSecondsAsync(attackColdown);
            onAttack = false;
        }
    }

    // --- LÓGICA DE COMBATE E MORTE ---

    private void Atacar()
    {
        Collider2D[] inimigosAtingidos = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider2D inimigo in inimigosAtingidos)
        {
            // TryGetComponent é mais performático que GetComponent
            if (inimigo.TryGetComponent(out VidaGlitch scriptVida))
            {
                scriptVida.ReceberDano(attackDamage);
            }
        }
    }

    public void ReceberDano(int dano, Vector2 posicaoDoPerigo)
    {
        if (isKnockback) return;

        vidaAtual -= dano;
        Debug.Log("Tomei dano! Vida restante: " + vidaAtual);

        if (vidaAtual <= 0) Morrer();
        else StartCoroutine(AplicarKnockback(posicaoDoPerigo));
    }

    private IEnumerator AplicarKnockback(Vector2 posicaoDoPerigo)
    {
        isKnockback = true;
        rb.linearVelocity = Vector2.zero;
        float direcaoX = transform.position.x < posicaoDoPerigo.x ? -1 : 1;
        rb.AddForce(new Vector2(direcaoX * forcaKnockbackX, forcaKnockbackY), ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockbackDuration);
        isKnockback = false;
    }

    private void Morrer()
    {
        vidaAtual = vidaMaxima;
        transform.position = pontoDeCheckpoint;
        rb.linearVelocity = Vector2.zero;
        isKnockback = false;

        if (areaDoCheckpoint != null && cameraSeguir != null)
        {
            cameraSeguir.FocarNoQuadrinho(areaDoCheckpoint);
        }
    }

    public void AtualizarCheckpoint(Vector2 novaPosicao, BoxCollider2D novaAreaDeCamera)
    {
        pontoDeCheckpoint = novaPosicao;
        areaDoCheckpoint = novaAreaDeCamera;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawCube(groundCheck.position,groundCheckSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(wallCheck.position, wallCheckSize);
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}