using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UpgradeUI : MonoBehaviour
{
    [Header("Ícones dos Upgrades")]
    public Image iconeAtaque;
    public Image iconeWallcling;

    [Header("Cores de Status")]
    public Color corBloqueado = new Color(0.2f, 0.2f, 0.2f, 1f);
    public Color corDesbloqueado = Color.white;

    [Header("Aviso na Tela (Texto)")]
    public RectTransform painelDeAviso;
    public TextMeshProUGUI textoDeAviso;
    public float tempoNaTela = 2.5f;
    public float velocidadeEscrita = 0.03f; // Um pouco mais rápido para cyberpunk

    [Header("Configuração de Movimento do Painel")]
    public float posEscondidaY = -300f;
    public float posVisivelY = 150f;
    public float tempoAnimacaoSlide = 0.4f;

    [Header("Configuração de Animação do Ícone")]
    public float tempoAnimacaoIcone = 0.3f;
    // Quanto ele estica (1.4f = 140% do tamanho)
    public Vector3 escalaDePulo = new Vector3(1.4f, 1.4f, 1f);

    [Header("Mensagens")]
    [TextArea] public string mensagemAtaque = "SISTEMA ATUALIZADO: Alcance de Ataque Expandido!";
    [TextArea] public string mensagemWallcling = "DOWNLOAD CONCLUÍDO: Pulo nas Paredes Liberado!";

    private void Start()
    {
        // Setup inicial das cores
        if (iconeAtaque != null) iconeAtaque.color = corBloqueado;
        if (iconeWallcling != null) iconeWallcling.color = corBloqueado;

        // Setup inicial da posição do painel
        if (painelDeAviso != null)
        {
            Vector2 pos = painelDeAviso.anchoredPosition;
            painelDeAviso.anchoredPosition = new Vector2(pos.x, posEscondidaY);
            painelDeAviso.gameObject.SetActive(false);
        }
    }

    // --- MÉTODOS CHAMADOS PELO PLAYER ---

    public void LigarIconeAtaque()
    {
        if (iconeAtaque != null)
        {
            iconeAtaque.color = corDesbloqueado;
            // Inicia a animação procedural de "esticar" no ícone
            StartCoroutine(AnimarPuloIcone(iconeAtaque.rectTransform));
        }
        StartCoroutine(RotinaCompletaAviso(mensagemAtaque));
    }

    public void LigarIconeWallcling()
    {
        if (iconeWallcling != null)
        {
            iconeWallcling.color = corDesbloqueado;
            // Inicia a animação procedural de "esticar" no ícone
            StartCoroutine(AnimarPuloIcone(iconeWallcling.rectTransform));
        }
        StartCoroutine(RotinaCompletaAviso(mensagemWallcling));
    }

    // --- COROTINA DE ANIMAÇÃO DO ÍCONE (Squash & Stretch procedural) ---
    private IEnumerator AnimarPuloIcone(RectTransform rectIcone)
    {
        if (rectIcone == null) yield break;

        // Garante que começamos do tamanho normal
        rectIcone.localScale = Vector3.one;

        float tempo = 0;
        float metadeDoTempo = tempoAnimacaoIcone / 2f;

        // 1. ESTICAR (De 1 para escalaDePulo)
        while (tempo < metadeDoTempo)
        {
            tempo += Time.deltaTime;
            // Usamos SmoothStep para um movimento mais "elástico" que o Lerp comum
            float t = tempo / metadeDoTempo;
            rectIcone.localScale = Vector3.Lerp(Vector3.one, escalaDePulo, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        // 2. VOLTAR AO NORMAL (De escalaDePulo para 1)
        tempo = 0;
        while (tempo < metadeDoTempo)
        {
            tempo += Time.deltaTime;
            float t = tempo / metadeDoTempo;
            rectIcone.localScale = Vector3.Lerp(escalaDePulo, Vector3.one, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        // Garante o tamanho final perfeito
        rectIcone.localScale = Vector3.one;
    }


    // --- COROTINAS DE TEXTO E PAINEL (Mantidas do passo anterior) ---

    private IEnumerator RotinaCompletaAviso(string mensagemCompleta)
    {
        // ... (Código mantido igual ao anterior, limpando texto e ligando painel)
        textoDeAviso.text = "";
        painelDeAviso.gameObject.SetActive(true);

        yield return StartCoroutine(MoverPainel(posEscondidaY, posVisivelY));

        foreach (char letra in mensagemCompleta.ToCharArray())
        {
            textoDeAviso.text += letra;
            yield return new WaitForSeconds(velocidadeEscrita);
        }

        yield return new WaitForSeconds(tempoNaTela);

        yield return StartCoroutine(MoverPainel(posVisivelY, posEscondidaY));

        painelDeAviso.gameObject.SetActive(false);
    }

    private IEnumerator MoverPainel(float deY, float paraY)
    {
        // ... (Código mantido igual ao anterior, movendo suavemente)
        float tempo = 0;
        Vector2 pos = painelDeAviso.anchoredPosition;

        while (tempo < tempoAnimacaoSlide)
        {
            tempo += Time.deltaTime;
            float novoY = Mathf.Lerp(deY, paraY, tempo / tempoAnimacaoSlide);
            painelDeAviso.anchoredPosition = new Vector2(pos.x, novoY);
            yield return null;
        }
        painelDeAviso.anchoredPosition = new Vector2(pos.x, paraY);
    }
}