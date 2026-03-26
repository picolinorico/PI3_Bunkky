using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour //Classe PlayerMovement herdando de MonoBehaviour, que é a classe base para todos os scripts em Unity
{
    [Header("Configurações de Movimento")]
    [SerializeField] private float speed = 5f; // Velocidade de movimento
    [SerializeField] private float jumpForce = 10f; // Força do pulo
    [SerializeField] private float fallMultiplier = 2.5f; // Queda mais rápida
    [SerializeField] private float lowJumpMultiplier = 2f; // Pulo mais baixo

    [Header("Verificação de Chão")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // movimento horizontal usando o valor do novo Input System
        rb.linearVelocity = new Vector2(moveInput.x * speed, rb.linearVelocity.y);
    }

    // chamado automaticamente pelo Player Input quando a ação "Move" é usada
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    // chamado automaticamente pelo Player Input quando a ação "Jump" é usada
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }
}
