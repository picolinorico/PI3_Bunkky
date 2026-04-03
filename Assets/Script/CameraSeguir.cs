using UnityEngine;

public class CameraSeguir : MonoBehaviour
{
    [Header("Configurações Base")]
    public Transform player;

    [Header("Início do Jogo")]
    public BoxCollider2D primeiroQuadrinhoDaFase;

    [Header("Configurações dos Painéis Fixos")]
    // Variável exclusiva para a transição entre quadrinhos
    public float tempoSuavizacaoPaineis = 0.3f;

    [Header("Configuração da Rampa (Seguir)")]
    public float zoomDoModoSeguir = 7f;
    // Nova variável exclusiva para quando a câmera segue o player
    public float tempoSuavizacaoSeguir = 0.1f;

    // Variáveis internas
    public bool modoSeguir = false;
    private Vector3 posicaoAlvo;
    private float tamanhoAlvo;
    private Vector3 velocidadePosicao;
    private float velocidadeZoom;

    // Essa variável vai guardar qual é a suavização certa pro momento
    private float suavizacaoAtual;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Start()
    {
        // Começa o jogo usando a suavização de painel
        suavizacaoAtual = tempoSuavizacaoPaineis;

        if (primeiroQuadrinhoDaFase != null)
        {
            FocarNoQuadrinho(primeiroQuadrinhoDaFase);
            transform.position = posicaoAlvo;
            cam.orthographicSize = tamanhoAlvo;
        }
    }

    private void LateUpdate()
    {
        if (modoSeguir && player != null)
        {
            posicaoAlvo = new Vector3(player.position.x, player.position.y, -10f);
            tamanhoAlvo = zoomDoModoSeguir;
        }

        // Repare que agora usamos a variável 'suavizacaoAtual' aqui no final, em vez de um valor fixo
        transform.position = Vector3.SmoothDamp(transform.position, posicaoAlvo, ref velocidadePosicao, suavizacaoAtual);
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, tamanhoAlvo, ref velocidadeZoom, suavizacaoAtual);
    }

    public void FocarNoQuadrinho(BoxCollider2D areaDestino)
    {
        modoSeguir = false;

        // Assim que focar num quadrinho, troca a velocidade para a de painel
        suavizacaoAtual = tempoSuavizacaoPaineis;

        Vector3 centro = areaDestino.bounds.center;
        posicaoAlvo = new Vector3(centro.x, centro.y, -10f);

        float proporcaoTela = (float)Screen.width / (float)Screen.height;
        float larguraArea = areaDestino.bounds.size.x;
        float alturaArea = areaDestino.bounds.size.y;

        if (larguraArea / alturaArea > proporcaoTela)
            tamanhoAlvo = (larguraArea / 2f) / proporcaoTela;
        else
            tamanhoAlvo = alturaArea / 2f;
    }

    public void AtivarModoSeguir()
    {
        modoSeguir = true;

        // Assim que entrar na rampa, troca a velocidade para a de seguir
        suavizacaoAtual = tempoSuavizacaoSeguir;
    }
}