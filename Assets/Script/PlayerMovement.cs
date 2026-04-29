using System.Collections;
using Unity.VisualScripting;
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

    [Header("Pulo")]
    [SerializeField] private float jumpForce = 35f;
    [SerializeField] private int maxJumps = 1;
    [SerializeField] private int jumpsLeft;

    [Header("Detecção de Chão")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(2.09f, 0.1f);
    [SerializeField] private bool isGrounded;

    [Header("Movimento nas Paredes")]
    [SerializeField] private float wallSlideSpeed = 1;
    [SerializeField] private bool isWallSliding;

    [Header("Pulo nas Paredes")]
    [SerializeField] private Vector2 wallJumpPower = new Vector2(12f, 24f);
    [SerializeField] private float wallJumpTime = 0.15f;
    [SerializeField] private bool isWallJumping;
    [SerializeField] private float wallJumpDirection;
    [SerializeField] private float wallJumpTimer;

    [Header("Detecção de Parede")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Vector2 wallCheckSize = new Vector2(0.11f, 4.43f);

    [Header("Ataque e Feedback")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private int attackDamage = 1;          //Arrumar
    [SerializeField] private float attackColdown = 0.5f;
    private bool onAttack = false;
    [SerializeField] private LayerMask enemyLayer;

    public Animator animator;

    [Header("Knockback")]
    public bool isKnockback; // Já estava aí, deixei só para você se localizar
    [SerializeField] private Vector2 forcaKnockback = new Vector2(15f, 10f); // Aumentei o padrão
    [SerializeField] private float tempoKnockback = 0.3f;

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

    void Start() // Primeira coisa que aocntece
    {
        pontoDeCheckpoint = transform.position;
    }

    void Update()
    {
        //Métodos
        GroundCheck();
        ProcessGravity(); 
        ProcessWallSlide();
        ProcessWallJump();
        if (horizontalMovement == 0)
        {
            animator.SetBool("Andando", false);
        }
        if (!isWallJumping)
        {
            //Movimento do jogador
            rb.linearVelocity = new Vector2(horizontalMovement * speed, rb.linearVelocity.y);
            Flip();
        }
    }

    void FixedUpdate() //Sla
    {

    }

    private void GroundCheck() //Checa de o jogador está tocando no chão
    {
        if (Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, groundLayer))
        {
            animator.SetBool("Pulando", false);
            jumpsLeft = maxJumps;
            isGrounded = true;
        }
        else
        {
            animator.SetBool("Pulando", true);
            isGrounded = false;
        }
    }
    private void ProcessGravity() //Melhora a gravidade bosta da Unity
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

    private bool WallCheck() //Checa de o jogador está tocando as paredes
    {

        return (Physics2D.OverlapBox(wallCheck.position, wallCheckSize, 0, wallLayer));

    }

    private void ProcessWallSlide()
    {
        if (!isGrounded & WallCheck() & horizontalMovement != 0)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed));
        }
        else
        {
            isWallSliding = false;
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

    private void CancelWallJump()
    {
        isWallJumping = false;
    }

    private void Flip() //Método responsável por fazer o sprite da personagm virar conforme a direção
    {
        if (isFacingRight && horizontalMovement < 0 || !isFacingRight && horizontalMovement > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }

    // --- INPUTS  ---

    public void OnMove(InputAction.CallbackContext context) //Recebe o input de movimento
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
        animator.SetBool("Andando", true);
    }

    public void OnJump(InputAction.CallbackContext context) //Pulo
    {

        
        //Pulo normal segurando espaço
        if (context.performed)
        {
            animator.SetBool("Pulando", true);
            if (jumpsLeft > 0)
            {
                animator.SetBool("Pulando", true);
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                jumpsLeft--;
            }
        }

        //Pra caso solte antes de atingir a parábola ou queira um little pulo
        else if (context.canceled && (rb.linearVelocity.y > 0))
        {
            
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }

        //Wall Jump
        if (context.performed && wallJumpTimer > 0f)
        {
            animator.SetBool("Pulando", true);
            isWallJumping = true;
            rb.linearVelocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
            wallJumpTimer = 0;

            //Força o flip da personagem no Wall Jump
            if (transform.localScale.x != wallJumpDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 ls = transform.localScale;
                ls.x *= -1f;
                transform.localScale = ls;
            }

            Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f); 
        }
    }

    private void OnDrawGizmosSelected() //Só estética pros Ground e Wall Cehck's
    {
        //Cor pro objeto GroundCheck
        Gizmos.color = Color.white;
        Gizmos.DrawCube(groundCheck.position, groundCheckSize);

        //Cor pro objeto WallCheck
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(wallCheck.position, wallCheckSize);

        //Sla
        if (attackPoint == null) return;
        Gizmos.color = Color.red;       //Sujeito a alterações
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    //Daqui pra baixo cpa eu mudo tudo

    public async void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && !onAttack)
        {
            onAttack = true;
            animator.SetBool("Atacando", true);
            Debug.Log("POW!");
            Atacar();

            await Awaitable.WaitForSecondsAsync(attackColdown);
            onAttack = false;
            animator.SetBool("Atacando", false);
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

    public void AplicarKnockback()
    {
        StartCoroutine(RotinaKnockback());
    }

    private IEnumerator RotinaKnockback()
    {
        isKnockback = true;

        // Zera a velocidade atual para que o movimento do player não anule o empurrão
        rb.linearVelocity = Vector2.zero;

        // Descobre para que lado o player está olhando e empurra pro outro
        float direcaoKnockback = transform.localScale.x > 0 ? -1f : 1f;

        // Usa a força que você configurar lá na Unity
        rb.AddForce(new Vector2(direcaoKnockback * forcaKnockback.x, forcaKnockback.y), ForceMode2D.Impulse);

        // Fica travado pelo tempo configurado
        yield return new WaitForSeconds(tempoKnockback);

        isKnockback = false;
    }

    // Função chamada pelo PlayerHealth
    public void Respawnar()
    {
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
}