using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private int extraJumpsValue = 1;
    [SerializeField] private float fallMultiplier = 4f;
    [SerializeField] private float lowJumpMultiplier = 3f;

    [Header("Vida e Dano")]
    [SerializeField] private int vidaMaxima = 3;
    private int vidaAtual;
    [SerializeField] private float forcaKnockbackX = 7f; // Força para trás
    [SerializeField] private float forcaKnockbackY = 5f; // Força para cima
    [SerializeField] private float knockbackDuration = 0.3f;

    [Header("Ataque e Feedback")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private int attackDamage = 1; // Quanto de dano o ataque dá
    [SerializeField] private LayerMask enemyLayer;

    [Header("Detecção de Chão")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isGrounded;
    private bool isHoldingJump;
    private int jumpCounter;
    private bool isKnockback;
    private Vector2 pontoDeCheckpoint;
    private BoxCollider2D areaDoCheckpoint;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // Inicia o jogo com a vida cheia
        vidaAtual = vidaMaxima;
        // O primeiro checkpoint é onde o jogador começa a fase
        pontoDeCheckpoint = transform.position;
    }

    void Update()
    {
        // Se estiver sofrendo knockback, o jogador não pode andar
        if (isKnockback) return;

        rb.linearVelocity = new Vector2(moveInput.x * speed, rb.linearVelocity.y);
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        if (isGrounded && rb.linearVelocity.y <= 0.1f)
            jumpCounter = extraJumpsValue;

        // Inverte o sprite preservando a escala original
        if (moveInput.x != 0)
        {
            Vector3 currentScale = transform.localScale;
            currentScale.x = Mathf.Abs(currentScale.x) * Mathf.Sign(moveInput.x);
            transform.localScale = currentScale;
        }
    }

    void FixedUpdate()
    {
        if (!isKnockback) ApplyBetterJumpPhysics();
    }

    private void ApplyBetterJumpPhysics()
    {
        if (rb.linearVelocity.y < 0)
            rb.gravityScale = fallMultiplier;
        else if (rb.linearVelocity.y > 0 && (!isHoldingJump || rb.linearVelocity.y < 2f))
            rb.gravityScale = lowJumpMultiplier;
        else
            rb.gravityScale = 1f;
    }

    // --- SISTEMA DE VIDA E KNOCKBACK ---

    public void ReceberDano(int dano, Vector2 posicaoDoPerigo)
    {
        // Evita tomar 10 danos de uma vez só se ficar encostado no espinho
        if (isKnockback) return;

        vidaAtual -= dano;
        Debug.Log("Tomei dano! Vida restante: " + vidaAtual);

        if (vidaAtual <= 0)
        {
            Morrer();
        }
        else
        {
            StartCoroutine(AplicarKnockback(posicaoDoPerigo));
        }
    }

    private IEnumerator AplicarKnockback(Vector2 posicaoDoPerigo)
    {
        isKnockback = true;

        // Zera a velocidade atual para o pulo não bugar
        rb.linearVelocity = Vector2.zero;

        // Descobre se o perigo está na direita ou esquerda para pular pro lado oposto
        float direcaoX = transform.position.x < posicaoDoPerigo.x ? -1 : 1;

        // Aplica o empurrão
        rb.AddForce(new Vector2(direcaoX * forcaKnockbackX, forcaKnockbackY), ForceMode2D.Impulse);

        // Espera o tempo do knockback passar
        yield return new WaitForSeconds(knockbackDuration);

        // Devolve o controle ao jogador
        isKnockback = false;
    }

    private void Morrer()
    {
        Debug.Log("O Player morreu! Voltando para o checkpoint...");

        // 1. Restaura a vida
        vidaAtual = vidaMaxima;

        // 2. Teleporta o personagem para o checkpoint
        transform.position = pontoDeCheckpoint;

        // 3. Zera a velocidade para ele não "nascer" caindo ou correndo
        rb.linearVelocity = Vector2.zero;

        // 4. Garante que o controle seja devolvido (caso tenha morrido no knockback)
        isKnockback = false;

        if (areaDoCheckpoint != null)
        {
            // Procura o Gerenciador e manda ele focar na área que salvamos
            FindAnyObjectByType<CameraSeguir>().FocarNoQuadrinho(areaDoCheckpoint);
        }
}

    // --- Função para atualizar o checkpoint quando você chegar em novas áreas ---
    public void AtualizarCheckpoint(Vector2 novaPosicao, BoxCollider2D novaAreaDeCamera)
    {
        pontoDeCheckpoint = novaPosicao;
        areaDoCheckpoint = novaAreaDeCamera; // Salva o quadrinho
        Debug.Log("Checkpoint salvo com câmera!");
    }

    // --- INPUTS ---
    public void OnMove(InputAction.CallbackContext context) => moveInput = context.ReadValue<Vector2>();

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && (isGrounded || jumpCounter > 0))
        {
            if (!isGrounded) jumpCounter--;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isHoldingJump = true;
        }
        if (context.canceled) isHoldingJump = false;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("POW!");
            Atacar();
        }
    }

    private void Atacar()
    {
        // 1. Cria um círculo invisível no "attackPoint" e pega tudo que encostar nele que seja da camada "enemyLayer"
        Collider2D[] inimigosAtingidos = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        // 2. Para cada inimigo ou glitch que o círculo acertou, aplica o dano
        foreach (Collider2D inimigo in inimigosAtingidos)
        {
            VidaGlitch scriptVida = inimigo.GetComponent<VidaGlitch>();
            if (scriptVida != null)
            {
                scriptVida.ReceberDano(attackDamage);
            }
        }
    }
    // Essa função serve SÓ para o editor da Unity. Ela desenha uma bolinha vermelha para você ver onde o ataque está batendo.
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}