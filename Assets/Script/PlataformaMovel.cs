using UnityEngine;

public class PlataformaMovel : MonoBehaviour
{
    [Header("Configurações de Trajeto")]
    [SerializeField] private Transform pontoA;
    [SerializeField] private Transform pontoB;
    [SerializeField] private float velocidade = 3f;

    [Header("Posição Inicial")]
    [Tooltip("Se marcado, começa no Ponto B. Se desmarcado, começa no Ponto A.")]
    [SerializeField] private bool comecarNoPontoB = false;

    private Vector3 destinoAtual;

    void Start()
    {
        // Define a posição inicial e o próximo destino baseado no Checkbox do Inspector
        if (comecarNoPontoB)
        {
            transform.position = pontoB.position;
            destinoAtual = pontoA.position;
        }
        else
        {
            transform.position = pontoA.position;
            destinoAtual = pontoB.position;
        }
    }

    void Update()
    {
        // Movimento constante em direção ao destino
        transform.position = Vector3.MoveTowards(transform.position, destinoAtual, velocidade * Time.deltaTime);

        // Troca de destino ao chegar
        if (Vector3.Distance(transform.position, destinoAtual) < 0.05f)
        {
            destinoAtual = (destinoAtual == pontoA.position) ? pontoB.position : pontoA.position;
        }
    }

    // Sistema para o player "grudar" na plataforma
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verifica se é o player e se o objeto não está sendo destruído
        if (collision.gameObject.CompareTag("Player") && gameObject.activeInHierarchy)
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Só tira o pai se a Coelha ainda existir
            if (collision.transform != null)
            {
                collision.transform.SetParent(null);
            }
        }
    }
}