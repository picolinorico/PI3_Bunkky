using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Movimento")]
    [SerializeField] private float speed = 30f;
    [SerializeField] private float horizontalMovement;
    [SerializeField] private bool isFacingRight = true;

    [Header("Gravidade")]
    [SerializeField] private int gravityBase = 6;
    [SerializeField] private float maxFallSpeed = 70;
    [SerializeField] private float fallSpeedMultiplier = 3;

    [Header("Pulo e Coyote")]
    [SerializeField] private float jumpForce = 35f;
    [SerializeField] private int maxJumps = 2; // Coloque 2 no Inspector para Pulo Duplo
    [SerializeField] private int jumpsLeft;
    [SerializeField] private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    [Header("Detecção de Chão")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(2.09f, 0.1f);
    [SerializeField] private bool isGrounded;

    [Header("Movimento nas Paredes")]
    [SerializeField] private float wallSlideSpeed = 1;
    private bool isWallSliding;

    [Header("Pulo nas Paredes")]
    [SerializeField] private Vector2 wallJumpPower = new Vector2(12f, 24f);
    [SerializeField] private float wallJumpTime = 0.15f;
    private bool isWallJumping;
    private float wallJumpDirection;
    private float wallJumpTimer;

    [Header("Detecção de Parede")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Vector2 wallCheckSize = new Vector2(0.11f, 4.43f);

    [Header("Ataque")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackColdown = 0.5f;
    [SerializeField] private LayerMask enemyLayer;
    private bool onAttack = false;

    public Animator animator;

    [Header("Knockback")]
    public bool isKnockback;
    [SerializeField] private Vector2 forcaKnockback = new Vector2(30f, 10f);
    [SerializeField] private float tempoKnockback = 0.3f;

    [Header("Checkpoint")]
    private Vector2 pontoDeCheckpoint;

    void Awake() => rb = GetComponent<Rigidbody2D>();

    void Start() => pontoDeCheckpoint = transform.position;

    void Update()
    {
        GroundCheck();
        ProcessGravity();
        ProcessWallSlide();
        ProcessWallJump();

        if (horizontalMovement == 0) animator.SetBool("Andando", false);

        if (!isWallJumping && !isKnockback)
        {
            rb.linearVelocity = new Vector2(horizontalMovement * speed, rb.linearVelocity.y);
            Flip();
        }
    }

    private void GroundCheck()
    {
        if (Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, groundLayer))
        {
            animator.SetBool("Pulando", false);
            isGrounded = true;
            jumpsLeft = maxJumps;
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            animator.SetBool("Pulando", true);
            isGrounded = false;
            coyoteTimeCounter -= Time.deltaTime;
        }
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

    private void ProcessWallSlide()
    {
        if (!isGrounded && WallCheck() && horizontalMovement != 0)
        {
            isWallSliding = true;
            animator.SetBool("Wallcling", true);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed));
        }
        else
        {
            isWallSliding = false;
            animator.SetBool("Wallcling", false);
        }
    }

    private void ProcessWallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpDirection = -transform.localScale.x;
            wallJumpTimer = wallJumpTime;
            CancelInvoke(nameof(CancelWallJump));
        }
        else if (wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime;
        }
    }

    private bool WallCheck() => Physics2D.OverlapBox(wallCheck.position, wallCheckSize, 0, wallLayer);

    private void Flip()
    {
        if (isFacingRight && horizontalMovement < 0 || !isFacingRight && horizontalMovement > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
        if (horizontalMovement != 0) animator.SetBool("Andando", true);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // 1. Prioridade total para o Wall Jump
            if (wallJumpTimer > 0f)
            {
                RealizarWallJump();
                return;
            }

            // 2. Se tem Coyote, pula direto sem gastar o estoque de "pulo extra"
            if (coyoteTimeCounter > 0f)
            {
                ExecutarPulo(false); // false = não gasta jumpsLeft
            }
            // 3. Se não tem Coyote, mas tem pulo sobrando (Double Jump)
            else if (jumpsLeft > 0)
            {
                ExecutarPulo(true); // true = gasta jumpsLeft
            }
        }
        else if (context.canceled && rb.linearVelocity.y > 0)
        {
            // Pulo curto
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
    }

    private void ExecutarPulo(bool gastarReserva)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        animator.SetBool("Pulando", true);

        if (gastarReserva)
        {
            jumpsLeft--;
        }

        // Crucial: Independente de como pulou, agora você está no ar.
        coyoteTimeCounter = 0f;
        isGrounded = false;
    }

    private void RealizarWallJump()
    {
        isWallJumping = true;
        rb.linearVelocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
        animator.SetBool("Pulando", true);
        wallJumpTimer = 0;

        if (transform.localScale.x != wallJumpDirection)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }

        Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f);
    }

    private void CancelWallJump() => isWallJumping = false;

    public async void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && !onAttack)
        {
            onAttack = true;
            animator.SetBool("Atacando", true);
            Atacar();
            await Awaitable.WaitForSecondsAsync(attackColdown);
            onAttack = false;
            animator.SetBool("Atacando", false);
        }
    }

    private void Atacar()
    {
        Collider2D[] inimigosAtingidos = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);
        foreach (Collider2D inimigo in inimigosAtingidos)
        {
            if (inimigo.TryGetComponent(out IDamageable objetoComVida))
            {
                objetoComVida.TakeDamage(attackDamage);
            }
        }
    }

    public void AplicarKnockback()
    {
        StartCoroutine(RotinaKnockback());
    }

    private IEnumerator RotinaKnockback()
    {
        isKnockback = true;
        rb.linearVelocity = Vector2.zero;
        float direcaoKnockback = transform.localScale.x > 0 ? -1f : 1f;
        rb.AddForce(new Vector2(direcaoKnockback * forcaKnockback.x, forcaKnockback.y), ForceMode2D.Impulse);
        yield return new WaitForSeconds(tempoKnockback);
        isKnockback = false;
    }

    public void Respawnar()
    {
        transform.position = pontoDeCheckpoint;
        rb.linearVelocity = Vector2.zero;
        isKnockback = false;
    }

    public void AtualizarCheckpoint(Vector2 novaPosicao) => pontoDeCheckpoint = novaPosicao;

    private void OnDrawGizmosSelected()
    {
        if (groundCheck) Gizmos.DrawCube(groundCheck.position, groundCheckSize);
        if (wallCheck) Gizmos.DrawCube(wallCheck.position, wallCheckSize);
        if (attackPoint) Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}