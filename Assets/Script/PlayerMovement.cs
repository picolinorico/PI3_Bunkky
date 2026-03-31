using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private int extraJumpsValue = 1;
    [SerializeField] private float fallMultiplier = 4f;
    [SerializeField] private float lowJumpMultiplier = 3f;

    [Header("Ataque e Feedback")]
    [SerializeField] private Transform attackPoint;
    //[SerializeField] private float attackRange = 0.5f;
    [SerializeField] private LayerMask enemyLayer;
    //[SerializeField] private float knockbackForce = 7f; // Aumentei um pouco para sentir o impacto
    //[SerializeField] private float knockbackDuration = 0.15f;

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

    void Awake() => rb = GetComponent<Rigidbody2D>();

    void Update()
    {
        if (isKnockback) return;

        rb.linearVelocity = new Vector2(moveInput.x * speed, rb.linearVelocity.y);
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        if (isGrounded && rb.linearVelocity.y <= 0.1f)
            jumpCounter = extraJumpsValue;

        // Inverte o sprite baseado na direção
        if (moveInput.x > 0) transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        else if (moveInput.x < 0) transform.localScale = new Vector3(-0.5f, 0.5f, 0.5f);
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
        }
    }
}