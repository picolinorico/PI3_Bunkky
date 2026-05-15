using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Animator animator;
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
    [SerializeField] private int maxJumps = 2;
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
    [SerializeField] private Vector2 attackSize = new Vector2(2f, 1f);
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackColdown = 0.5f;
    [SerializeField] private LayerMask enemyLayer;
    private bool onAttack = false;

    [Header("Habilidades Desbloqueáveis")]
    [SerializeField] private bool canWallCling = false; // Começa desativado
    [SerializeField] private int deadEnemies = 0;
    [SerializeField] private int deathsForUpgrades = 3;
    [SerializeField] private Vector2 sizeUpgrade = new Vector2(8f, 2f); // Alcance maior
    //[SerializeField] private int danoUpgrade = 2;
    private bool isUpgraded = false;

    [Header("Knockback")]
    public bool isKnockback;
    [SerializeField] private Vector2 forcaKnockback = new Vector2(30f, 10f);
    [SerializeField] private float tempoKnockback = 0.3f;

    [Header("Checkpoint")]
    private Vector2 pontoDeCheckpoint;

    [Header("Conexão com a UI")]
    public UpgradeUI interfaceDeUpgrades;

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
            isGrounded = false;
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    // --- RESTO DOS MÉTODOS MANTIDOS ---

    private void ProcessGravity()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = gravityBase * fallSpeedMultiplier;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
        }
        else rb.gravityScale = gravityBase;
    }

    private void ProcessWallSlide()
    {
        // A novidade é o "&& canWallCling" no final. 
        // Se for falso, ela ignora a parede e cai direto.
        if (!isGrounded && WallCheck() && horizontalMovement != 0 && canWallCling)
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
        else if (wallJumpTimer > 0f) wallJumpTimer -= Time.deltaTime;
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
            if (wallJumpTimer > 0f) { RealizarWallJump(); return; }

            if (coyoteTimeCounter > 0f) ExecutarPulo(false);
            else if (jumpsLeft > 0) ExecutarPulo(true);
        }
        else if (context.canceled && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
    }

    private void ExecutarPulo(bool gastarReserva)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        animator.SetBool("Pulando", true);
        if (gastarReserva) jumpsLeft--;
        coyoteTimeCounter = 0f;
        isGrounded = false;
    }

    private void RealizarWallJump()
    {
        isWallJumping = true;
        rb.linearVelocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
        animator.SetBool("Pulando", true);
        wallJumpTimer = 0;
        if (transform.localScale.x != wallJumpDirection) FlipManualmente();
        Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f);
    }

    private void FlipManualmente()
    {
        isFacingRight = !isFacingRight;
        Vector3 ls = transform.localScale;
        ls.x *= -1f;
        transform.localScale = ls;
    }

    private void CancelWallJump() => isWallJumping = false;

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && !onAttack)
        {
            StartCoroutine(RotinaAtaquePorAnimacao());
        }
    }

    private IEnumerator RotinaAtaquePorAnimacao()
    {
        onAttack = true;
        //animator.SetBool("Forte", isUpgraded);
        animator.SetBool("Atacando", true);

        // Espera passar 1 frame para o Animator transicionar para o estado correto
        yield return new WaitForEndOfFrame();

        AnimatorStateInfo estadoAtual = animator.GetCurrentAnimatorStateInfo(0);
        float duracaoDaAnimacao = estadoAtual.length;
        float tempoPassado = 0f;
        System.Collections.Generic.List<Collider2D> inimigosJaAtingidos = new System.Collections.Generic.List<Collider2D>();

        while (tempoPassado < duracaoDaAnimacao)
        {
            Atacar(inimigosJaAtingidos);

            tempoPassado += Time.deltaTime;
            yield return null;
        }

        onAttack = false;
        animator.SetBool("Atacando", false);
    }

    private void Atacar(System.Collections.Generic.List<Collider2D> jaAtingidos)
    {
        Collider2D[] inimigosAtingidos = Physics2D.OverlapBoxAll(attackPoint.position, attackSize, 0f, enemyLayer);

        foreach (Collider2D inimigo in inimigosAtingidos)
        {
            // SE o inimigo já tomou dano NESTE ataque, pula ele e vai pro próximo
            if (jaAtingidos.Contains(inimigo)) continue;

            if (inimigo.TryGetComponent(out IDamageable objetoComVida))
            {
                objetoComVida.TakeDamage(attackDamage);

                // Adiciona o bicho na lista negra para ele não tomar dano de novo até você atacar outra vez
                jaAtingidos.Add(inimigo);
                Debug.Log($"Deu {attackDamage} de dano em: " + inimigo.name);
            }
        }
    }

    // --- LÓGICA DO UPGRADE ---
    public void RegistrarMorte()
    {
        Debug.Log("AAAAAAAA");
        if (isUpgraded) return;

        deadEnemies++;
        if (deadEnemies >= deathsForUpgrades)
        {
            isUpgraded = true;
            attackSize = sizeUpgrade;
            //attackDamage = danoUpgrade;
            Debug.Log("ATAQUE MELHORADO PERMANENTE!");
            // Aqui você pode instanciar uma partícula de brilho na coelha se quiser
            if (interfaceDeUpgrades != null) interfaceDeUpgrades.LigarIconeAtaque();
        }
    }

    public void DesbloquearWallCling()
    {
        if (!canWallCling)
        {
            canWallCling = true;
            Debug.Log("UPGRADE LIBERADO: Wallcling e Pulo na Parede ativados!");

            // AVISA A UI PARA ACENDER O ÍCONE:
            if (interfaceDeUpgrades != null) interfaceDeUpgrades.LigarIconeWallcling();
        }
    }

    public void AplicarKnockback() => StartCoroutine(RotinaKnockback());

    private IEnumerator RotinaKnockback()
    {
        isKnockback = true;
        rb.linearVelocity = Vector2.zero;
        float dir = transform.localScale.x > 0 ? -1f : 1f;
        rb.AddForce(new Vector2(dir * forcaKnockback.x, forcaKnockback.y), ForceMode2D.Impulse);
        yield return new WaitForSeconds(tempoKnockback);
        isKnockback = false;
    }

    public void Respawnar()
    {
        transform.position = pontoDeCheckpoint;
        rb.linearVelocity = Vector2.zero;
        isKnockback = false;
        // Se quiser resetar o upgrade ao morrer, descomente abaixo:
        // jaUpgradou = false; inimigosMortos = 0; 
    }

    public void AtualizarCheckpoint(Vector2 novaPosicao) => pontoDeCheckpoint = novaPosicao;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (groundCheck) Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        if (wallCheck) Gizmos.DrawWireCube(wallCheck.position, wallCheckSize);
        if (attackPoint) Gizmos.DrawWireCube(attackPoint.position, attackSize);
    }
}