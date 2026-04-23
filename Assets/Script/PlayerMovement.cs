using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Vector2 moveInput;
    [SerializeField] private float bloqueioMovimentoTimer;
    [SerializeField] private bool facingRight = true;

    [Header("Pulo")]
    [SerializeField] private float jumpForce = 18f;
    [SerializeField] private int maxJumps = 1;
    [SerializeField] private int jumpsLeft;
    [SerializeField] private bool isGrounded;

    [Header("Gravidade")]
    [SerializeField] private int gravityBase = 2;
    [SerializeField] private float maxFallSpeed = 45;
    [SerializeField] private float fallSpeedMultiplier = 3;

    [Header("Parede: Deslizar e Pular")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallSlidingSpeed = 2f;
    [SerializeField] private Vector2 wallJumpPower = new Vector2(10f, 12f);
    [SerializeField] private float tempoBloqueioMovimento = 0.2f;
    [SerializeField] private float tempoPresoNaParede = 0.15f;
    private bool isTouchingWall;
    private bool isWallSliding;
    private float agarrarTimer;

    [Header("Vida e Dano")]
    [SerializeField] private int vidaMaxima = 3;
    private int vidaAtual;
    public VidaUI controleDeUI;
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
    [SerializeField] private float checkRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

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

        if (controleDeUI != null) controleDeUI.AtualizarCoracoes(vidaAtual, vidaMaxima);
    }

    void Update()
    {
        if (isKnockback) return;

        CheckSurroundings();
        HandleWallSliding();
        HandleMovement();
        Gravity();
    }

    private void Gravity()
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

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, checkRadius, wallLayer);

        // Garante que o pulo recarregue ao tocar no chão OU ao agarrar na parede
        if (isGrounded || isWallSliding)
        {
            jumpsLeft = maxJumps;
        }
    }

    private void HandleMovement()
    {
        if (bloqueioMovimentoTimer > 0)
        {
            bloqueioMovimentoTimer -= Time.deltaTime;
        }
        else
        {
            // Movimento Horizontal
            rb.linearVelocity = new Vector2(moveInput.x * speed, rb.linearVelocity.y);

            // Lógica de Flip centralizada
            if (moveInput.x > 0 && !facingRight) Flip();
            else if (moveInput.x < 0 && facingRight) Flip();
        }
    }

    private void HandleWallSliding()
    {
        if (isTouchingWall && !isGrounded && moveInput.x != 0)
        {
            if (!isWallSliding) agarrarTimer = tempoPresoNaParede;
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }

        if (isWallSliding && agarrarTimer > 0)
            agarrarTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (isKnockback) return;

        if (isWallSliding)
        {
            if (agarrarTimer > 0)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            else
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlidingSpeed, 0f));
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // --- INPUTS (New Input System) ---

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isWallSliding)
            {
                WallJump();
            }
            else if (isGrounded || jumpsLeft > 0)
            {
                // Pulo normal implementado pela sua dupla
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                jumpsLeft--;
            }
        }
        else if (context.canceled)
        {
            // A lógica limpa da sua dupla para controlar a altura do pulo
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

        if (controleDeUI != null) controleDeUI.AtualizarCoracoes(vidaAtual, vidaMaxima);

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

        if (controleDeUI != null) controleDeUI.AtualizarCoracoes(vidaAtual, vidaMaxima);

        if (areaDoCheckpoint != null && cameraSeguir != null)
        {
            cameraSeguir.FocarNoQuadrinho(areaDoCheckpoint);
        }
    }

    private void WallJump()
    {
        isWallSliding = false;
        bloqueioMovimentoTimer = tempoBloqueioMovimento;
        float direcaoPulo = -Mathf.Sign(transform.localScale.x);
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(wallJumpPower.x * direcaoPulo, wallJumpPower.y), ForceMode2D.Impulse);

        // Garante que o Flip aconteça no pulo da parede de forma limpa
        if ((direcaoPulo > 0 && !facingRight) || (direcaoPulo < 0 && facingRight)) Flip();
    }

    public void AtualizarCheckpoint(Vector2 novaPosicao, BoxCollider2D novaAreaDeCamera)
    {
        pontoDeCheckpoint = novaPosicao;
        areaDoCheckpoint = novaAreaDeCamera;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}