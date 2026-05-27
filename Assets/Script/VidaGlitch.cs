using UnityEngine;
using System.Collections; // ESSENCIAL para usar Coroutines (IEnumerator)

public class VidaGlitch : MonoBehaviour, IDamageable
{
    [Header("Configurações")]
    private int vidaGlitch = 1;

    [Header("Efeitos")]
    [SerializeField] private GameObject particulaPrefab;

    [Header("Feedback de Morte (Pisca-Pisca)")]
    [SerializeField] private float duracaoPiscarMorte = 0.2f; // Quanto tempo ele fica piscando
    [SerializeField] private float intervaloPiscar = 0.1f;  // Velocidade do pisca-pisca

    private SpriteRenderer _sr;
    private Color _originalColor;
    private bool _isDead = false; // Evita bugar se tomar dano duplo enquanto pisca

    private void Awake()
    {
        // Pega o SpriteRenderer do Glitch e guarda a cor original dele
        _sr = GetComponent<SpriteRenderer>();
        if (_sr != null)
        {
            _originalColor = _sr.color;
        }
    }

    public void TakeDamage(int dano)
    {
        // Se já estiver no processo de quebrar, ignora mais danos
        if (_isDead) return;

        vidaGlitch -= dano;

        if (vidaGlitch <= 0)
        {
            Quebrar();
        }
    }

    private void Quebrar()
    {
        _isDead = true;
        Debug.Log("Glitch destruído!");

        // 1. Cria as partículas na posição do objeto
        if (particulaPrefab != null)
        {
            GameObject particulas = Instantiate(particulaPrefab, transform.position, Quaternion.identity);
            Destroy(particulas, 2f);
        }

        // 2. DESLIGA OS COLISORES (Pai e Filhos) para a coelha passar por dentro dele enquanto pisca
        Collider2D[] todosOsColisores = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in todosOsColisores)
        {
            col.enabled = false;
        }

        // 3. Se ele tiver um Rigidbody2D, deixa Kinematic para não cair no limbo do mapa
        if (TryGetComponent(out Rigidbody2D rb))
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
        }

        // 4. Se tiver Animator, congela a animação do glitch
        if (TryGetComponent(out Animator anim))
        {
            anim.enabled = false;
        }

        // 5. Inicia a rotina de piscar e adia o Destroy do gameObject
        if (_sr != null)
        {
            StartCoroutine(RotinaPiscarMorte());
        }
        else
        {
            // Margem de segurança: Se o objeto por acaso não tiver SpriteRenderer, destrói logo
            Destroy(gameObject);
        }
    }

    private IEnumerator RotinaPiscarMorte()
    {
        float tempoPassado = 0f;
        bool visivel = true;

        while (tempoPassado < duracaoPiscarMorte)
        {
            visivel = !visivel;

            if (visivel)
            {
                _sr.color = _originalColor; // Volta para a cor normal
            }
            else
            {
                // Deixa o Alpha em 0 (Totalmente transparente)
                _sr.color = new Color(_originalColor.r, _originalColor.g, _originalColor.b, 0f);
            }

            yield return new WaitForSeconds(intervaloPiscar);
            tempoPassado += intervaloPiscar;
        }

        // SÓ AGORA deleta o Glitch da cena de verdade!
        Destroy(gameObject);
    }
}