using UnityEngine;
using UnityEngine.UI; // Essencial para podermos manipular imagens da UI!

public class VidaUI : MonoBehaviour
{
    [Header("Configurações da UI")]
    public Image[] imagensDosCoracoes; // Guarda a lista dos 3 corações da tela
    public Sprite coracaoCheio;        // A imagem vermelha
    public Sprite coracaoVazio;        // A imagem preta

    // Essa função será chamada pelo Player toda vez que a vida mudar
    public void AtualizarCoracoes(int vidaAtual, int vidaMaxima)
    {
        for (int i = 0; i < imagensDosCoracoes.Length; i++)
        {
            // Se o número do coração for menor que a vida atual, ele fica vermelho
            if (i < vidaAtual)
            {
                imagensDosCoracoes[i].sprite = coracaoCheio;
            }
            // Se não, ele fica preto (vazio)
            else
            {
                imagensDosCoracoes[i].sprite = coracaoVazio;
            }

            // Isso garante que se você tiver 3 de vida máxima, só 3 corações aparecem
            if (i < vidaMaxima)
            {
                imagensDosCoracoes[i].enabled = true;
            }
            else
            {
                imagensDosCoracoes[i].enabled = false;
            }
        }
    }
}