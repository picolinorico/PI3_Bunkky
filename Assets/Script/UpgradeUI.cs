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
    public RectTransform painelDeAviso; // Arraste o RectTransform do painel aqui
    public TextMeshProUGUI textoDeAviso;
    public float tempoNaTela = 2.5f;
    public float velocidadeEscrita = 0.05f; // Tempo entre cada letra

    [Header("Configuração de Movimento")]
    public float posEscondidaY = -300f; // Posição abaixo da tela
    public float posVisivelY = 150f;    // Posição onde ele para na tela
    public float tempoAnimacaoSlide = 0.5f;

    [Header("Mensagens")]
    [TextArea] public string mensagemAtaque = "SISTEMA ATUALIZADO: Alcance de Ataque Expandido!";
    [TextArea] public string mensagemWallcling = "DOWNLOAD CONCLUÍDO: Pulo nas Paredes Liberado!";

    private void Start()
    {
        if (iconeAtaque != null) iconeAtaque.color = corBloqueado;
        if (iconeWallcling != null) iconeWallcling.color = corBloqueado;

        // Começa escondido embaixo da tela
        if (painelDeAviso != null)
        {
            Vector2 pos = painelDeAviso.anchoredPosition;
            painelDeAviso.anchoredPosition = new Vector2(pos.x, posEscondidaY);
            painelDeAviso.gameObject.SetActive(false);
        }
    }

    public void LigarIconeAtaque()
    {
        if (iconeAtaque != null) iconeAtaque.color = corDesbloqueado;
        StartCoroutine(RotinaCompletaAviso(mensagemAtaque));
    }

    public void LigarIconeWallcling()
    {
        if (iconeWallcling != null) iconeWallcling.color = corDesbloqueado;
        StartCoroutine(RotinaCompletaAviso(mensagemWallcling));
    }

    private IEnumerator RotinaCompletaAviso(string mensagemCompleta)
    {
        // 1. PREPARAÇÃO
        textoDeAviso.text = ""; // Limpa o texto
        painelDeAviso.gameObject.SetActive(true);

        // 2. SLIDE PARA CIMA (Entrada)
        yield return StartCoroutine(MoverPainel(posEscondidaY, posVisivelY));

        // 3. EFEITO MÁQUINA DE ESCREVER
        foreach (char letra in mensagemCompleta.ToCharArray())
        {
            textoDeAviso.text += letra;
            // Toca um som de "clique" aqui se você tiver!
            yield return new WaitForSeconds(velocidadeEscrita);
        }

        // 4. ESPERA UM POUCO PARA O PLAYER LER
        yield return new WaitForSeconds(tempoNaTela);

        // 5. SLIDE PARA BAIXO (Saída)
        yield return StartCoroutine(MoverPainel(posVisivelY, posEscondidaY));

        painelDeAviso.gameObject.SetActive(false);
    }

    // Função auxiliar para mover o painel suavemente
    private IEnumerator MoverPainel(float deY, float paraY)
    {
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