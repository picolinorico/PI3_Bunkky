using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float speed = 8f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private float bloqueioMovimentoTimer; // Trava o movimento horizontal logo após o wall jump

    [Header("Pulo")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private int extraJumpsValue = 1;
    [SerializeField] private int jumpCounter;
    [SerializeField] private float fallMultiplier = 4f;
    [SerializeField] private float lowJumpMultiplier = 3f;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isHoldingJump;

    [Header("Parede: Deslizar e Pular")]
    [SerializeField] private Transform wallCheck; // Onde fica o sensor da parede
    [SerializeField] private LayerMask wallLayer; // O que é considerado parede
    [SerializeField] private float wallSlidingSpeed = 2f; // Velocidade que ele escorrega
    [SerializeField] private Vector2 wallJumpPower = new Vector2(10f, 12f); // Força do pulo na parede (X, Y)
    [SerializeField] private float tempoBloqueioMovimento = 0.2f; // Tempo que o jogador perde o controle do X após pular da parede
    [Tooltip("Quanto tempo a personagem fica grudada na parede sem escorregar ao bater nela")]
    [SerializeField] private float tempoPresoNaParede = 0.15f;
    private bool isTouchingWall;
    private bool isWallSliding;
    private float agarrarTimer; // O cronômetro interno para segurar na parede

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
    [SerializeField] private bool isKnockback;


    [Header("Detecção de Chão")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

   
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

        // 1. Sensores
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, checkRadius, wallLayer);

        // 2. Lógica do Wall Slide
        // Se estiver tocando na parede, não estiver no chão, e o jogador estiver empurrando para o lado da parede
        if (isTouchingWall && !isGrounded && moveInput.x != 0)
        {
            // Se ela acabou de bater na parede neste frame exato
            if (!isWallSliding)
            {
                agarrarTimer = tempoPresoNaParede; // Enche o cronômetro
            }
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
        // Diminui o timer de agarrar enquanto estiver grudada
        if (isWallSliding && agarrarTimer > 0)
        {
            agarrarTimer -= Time.deltaTime;
        }

        // 3. Controle de Movimento Horizontal
        if (bloqueioMovimentoTimer > 0)
        {
            // Se acabou de fazer um wall jump, diminui o timer e não deixa o jogador parar o personagem no ar instantaneamente
            bloqueioMovimentoTimer -= Time.deltaTime;
        }
        else
        {
            // Movimento normal
            rb.linearVelocity = new Vector2(moveInput.x * speed, rb.linearVelocity.y);

            // Inverte o sprite
            if (moveInput.x != 0)
            {
                Vector3 currentScale = transform.localScale;
                currentScale.x = Mathf.Abs(currentScale.x) * Mathf.Sign(moveInput.x);
                transform.localScale = currentScale;
            }
        }

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
        if (isKnockback) return;

        if (isWallSliding)
        {
            if (agarrarTimer > 0)
            {
                // FASE 1: ACABOU DE GRUDAR. Zera a velocidade Y para não subir nem descer.
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            }
            else
            {
                // FASE 2: COMEÇA A ESCORREGAR. O limite máximo agora é 0f (ela nunca sobe enquanto escorrega).
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlidingSpeed, 0f));
            }
        }
        else
        {
            ApplyBetterJumpPhysics();
        }
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

    // --- SISTEMA DE PULO (ATUALIZADO PARA WALL JUMP) ---
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isWallSliding)
            {
                // WALL JUMP!
                isWallSliding = false;
                bloqueioMovimentoTimer = tempoBloqueioMovimento; // Impede o player de voltar pra parede no mesmo milissegundo

                // Descobre para qual lado pular (o oposto de onde o personagem está olhando)
                float direcaoPulo = -Mathf.Sign(transform.localScale.x);

                rb.linearVelocity = Vector2.zero; // Zera a velocidade atual para o pulo ser limpo
                rb.AddForce(new Vector2(wallJumpPower.x * direcaoPulo, wallJumpPower.y), ForceMode2D.Impulse);

                // Vira o personagem para o lado do pulo
                Vector3 currentScale = transform.localScale;
                currentScale.x = Mathf.Abs(currentScale.x) * direcaoPulo;
                transform.localScale = currentScale;
            }
            else if (isGrounded || jumpCounter > 0)
            {
                // PULO NORMAL
                if (!isGrounded) jumpCounter--;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                isHoldingJump = true;
            }
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