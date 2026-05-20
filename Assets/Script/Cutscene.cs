using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections; // Necessário para Corotinas

// Adiciona automaticamente o componente necessário para transparência suave
[RequireComponent(typeof(CanvasGroup))]
public class CutsceneSimples : MonoBehaviour
{
    [Header("Configurações da HQ")]
    public Image imagemNaTela;
    public Sprite[] paginas;
    [SerializeField] private float tempoTransicao = 0.5f; // Duração do fade

    [Header("Cena Seguinte")]
    public string nomeDaFase = "fase_1";

    [Header("UI de Segurar Skip")]
    [SerializeField] private GameObject painelSkip; // O objeto pai da UI de skip
    [SerializeField] private Image fillImageSkip; // A imagem que vai preencher (tipo radial)
    [SerializeField] private float tempoNecessarioSkip = 1.5f; // Quanto tempo segurar

    private int paginaAtual = 0;
    private CanvasGroup canvasGroup; // Para controlar a opacidade da imagem
    private bool isChangingPage = false; // Trava para não bugar ao spammar botões
    private float tempoSegurandoSkip = 0f;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (painelSkip != null) painelSkip.SetActive(false); // Começa escondido
    }

    void Start()
    {
        // Começa mostrando a primeira imagem com opacidade total
        canvasGroup.alpha = 1f;
        AtualizarSpriteNaTela();
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        // LÓGICA DE SEGURAR PARA PULAR (ESPAÇO)
        if (Keyboard.current.spaceKey.isPressed)
        {
            tempoSegurandoSkip += Time.deltaTime; // Soma o tempo

            // Ativa o painel se começou a segurar
            if (painelSkip != null && !painelSkip.activeSelf) painelSkip.SetActive(true);

            // Atualiza o preenchimento visual (vai de 0 a 1)
            if (fillImageSkip != null)
            {
                fillImageSkip.fillAmount = tempoSegurandoSkip / tempoNecessarioSkip;
            }

            // Se segurou o suficiente, PULA
            if (tempoSegurandoSkip >= tempoNecessarioSkip)
            {
                PularParaOJogo();
            }
        }

        // Quando soltar o Espaço, reseta tudo e esconde a UI
        if (Keyboard.current.spaceKey.wasReleasedThisFrame)
        {
            tempoSegurandoSkip = 0f;
            if (fillImageSkip != null) fillImageSkip.fillAmount = 0f;
            if (painelSkip != null) painelSkip.SetActive(false);
        }


        // LÓGICA DE AVANÇAR/VOLTAR (Só funciona se não estiver no meio de uma transição)
        if (!isChangingPage)
        {
            // Avançar com 'E'
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                PassarPagina();
            }

            // Voltar com 'Q'
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                VoltarPagina();
            }
        }
    }

    public void PassarPagina()
    {
        if (paginaAtual + 1 < paginas.Length)
        {
            StartCoroutine(MudarPaginaRoutine(paginaAtual + 1));
        }
        else
        {
            // Se passar da última página, o jogo começa
            PularParaOJogo();
        }
    }

    public void VoltarPagina()
    {
        if (paginaAtual - 1 >= 0)
        {
            StartCoroutine(MudarPaginaRoutine(paginaAtual - 1));
        }
    }

    // A mágica da transição acontece aqui
    private IEnumerator MudarPaginaRoutine(int indexDestino)
    {
        isChangingPage = true;

        // Pega o componente que controla o tamanho da imagem na UI
        RectTransform rectDaImagem = imagemNaTela.rectTransform;
        Vector3 escalaOriginal = rectDaImagem.localScale;

        float metadeDoTempo = tempoTransicao / 2f;
        float tempo = 0f;

        // 1. FECHANDO A PÁGINA (Esmaga o X de 1 para 0)
        while (tempo < metadeDoTempo)
        {
            tempo += Time.deltaTime;
            // O Lerp calcula a diminuição suave
            float novoX = Mathf.Lerp(escalaOriginal.x, 0f, tempo / metadeDoTempo);
            rectDaImagem.localScale = new Vector3(novoX, escalaOriginal.y, escalaOriginal.z);
            yield return null;
        }

        // Garante que o X fique exatamente em 0 (página de lado, invisível)
        rectDaImagem.localScale = new Vector3(0f, escalaOriginal.y, escalaOriginal.z);

        // 2. TROCA A ARTE DA HQ (Ninguém vê porque a página está "de lado")
        paginaAtual = indexDestino;
        AtualizarSpriteNaTela();

        // 3. ABRINDO A PÁGINA NOVA (Aumenta o X de 0 para 1)
        tempo = 0f;
        while (tempo < metadeDoTempo)
        {
            tempo += Time.deltaTime;
            float novoX = Mathf.Lerp(0f, escalaOriginal.x, tempo / metadeDoTempo);
            rectDaImagem.localScale = new Vector3(novoX, escalaOriginal.y, escalaOriginal.z);
            yield return null;
        }

        // Garante que volte ao tamanho perfeito original
        rectDaImagem.localScale = escalaOriginal;

        isChangingPage = false;
    }

    // Apenas muda o sprite, sem lidar com tempo
    private void AtualizarSpriteNaTela()
    {
        if (paginas.Length > 0 && imagemNaTela != null)
        {
            imagemNaTela.sprite = paginas[paginaAtual];
        }
    }

    public void PularParaOJogo()
    {
        // Garante que não carregar a cena múltiplas vezes
        if (isChangingPage) return;
        isChangingPage = true;

        Debug.Log("Carregando o jogo...");
        SceneManager.LoadScene(nomeDaFase);
    }
}