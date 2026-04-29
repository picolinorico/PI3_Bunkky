using UnityEngine;

public class BossHarpia : MonoBehaviour
{
    [Header("Configurações de Voo")]
    [SerializeField] private float velocidade = 4f;
    [SerializeField] private float alturaDesejada = 3f; // Ela tenta ficar acima do player
    [SerializeField] private float distanciaHorizontal = 5f; // Distância que ela mantém lateralmente

    private Transform player;
    private Vector2 pontoDestino;
    //private bool escolhendoNovoPonto = false;

    void Start()
    {
        // Certifique-se que a Coelha tenha a tag "Player"
        player = GameObject.FindGameObjectWithTag("Player").transform;
        pontoDestino = transform.position;
        IniciarCicloDeVoo();
    }

    // Usamos o sistema assíncrono para dar "inteligência" ao tempo de decisão
    private async void IniciarCicloDeVoo()
    {
        while (this != null) // Enquanto a Harpia existir
        {
            if (player != null)
            {
                // Escolhe um lado aleatório (esquerda ou direita do player)
                float lado = Random.value > 0.5f ? 1f : -1f;

                // Define um ponto no ar próximo ao player, mas com uma folga
                pontoDestino = new Vector2(
                    player.position.x + (distanciaHorizontal * lado),
                    player.position.y + alturaDesejada
                );
            }

            // Espera de 2 a 4 segundos antes de mudar de ideia de novo
            await Awaitable.WaitForSecondsAsync(Random.Range(2f, 4f));
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // Move a Harpia suavemente em direção ao ponto destino escolhido
        transform.position = Vector2.MoveTowards(transform.position, pontoDestino, velocidade * Time.fixedDeltaTime);

        // Inverte o sprite para olhar sempre para a Coelha
        float direcaoParaPlayer = player.position.x - transform.position.x;
        if (direcaoParaPlayer != 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * Mathf.Sign(direcaoParaPlayer), transform.localScale.y, 1);
        }
    }
}