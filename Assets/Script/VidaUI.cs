using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class VidaUI : MonoBehaviour
{
    [Header("Configurações da UI")]
    public Image[] imagensDosCoracoes;
    public Sprite coracaoCheio, coracaoVazio; // Variáveis do mesmo tipo na mesma linha

    // Lembra da vida anterior para saber se tomou dano (começa em -1 para forçar o setup inicial)
    private int vidaAnterior = -1;

    public void AtualizarCoracoes(int vidaAtual, int vidaMaxima)
    {
        // Isso evita que um coração fique preto depois do respawn.
        StopAllCoroutines();

        // Descobre se o jogador acabou de tomar dano comparando a vida
        bool tomouDano = vidaAnterior != -1 && vidaAtual < vidaAnterior;

        // O coração que vai piscar é exatamente o número da vida atual
        int indexPerdido = tomouDano ? vidaAtual : -1;

        // Passa por todos os corações da tela em um único loop rápido
        for (int i = 0; i < imagensDosCoracoes.Length; i++)
        {
            // Mostra apenas a quantidade de corações da vida máxima (desliga os extras)
            imagensDosCoracoes[i].enabled = i < vidaMaxima;

            // Se for o coração atingido agora, ele pisca. Se não for, atualiza normal.
            if (i == indexPerdido)
            {
                StartCoroutine(PiscarCoracao(imagensDosCoracoes[i]));
            }
            else
            {
                // Define direto: Se o índice for menor que a vida, é Cheio. Senão, é Vazio.
                imagensDosCoracoes[i].sprite = (i < vidaAtual) ? coracaoCheio : coracaoVazio;
            }
        }

        // Salva a vida atual para a próxima checagem
        vidaAnterior = vidaAtual;
    }

    // A mágica do tempo para dar o "Game Feel"
    private IEnumerator PiscarCoracao(Image coracao)
    {
        // Vamos fazer o coração piscar 3 vezes
        for (int i = 0; i < 3; i++)
        {
            coracao.sprite = coracaoVazio; // Fica preto
            yield return new WaitForSeconds(0.1f); // Espera um pouquinho

            coracao.sprite = coracaoCheio; // Volta a ficar vermelho
            yield return new WaitForSeconds(0.1f); // Espera um pouquinho
        }

        // No final de tudo, garante que ele apague de vez (fique preto)
        coracao.sprite = coracaoVazio;
    }
}