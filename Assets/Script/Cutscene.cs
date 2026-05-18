using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // Necessário para ler o teclado novo

public class CutsceneSimples : MonoBehaviour
{
    [Header("Configurações da HQ")]
    public Image imagemNaTela;
    public Sprite[] paginas;

    [Header("Cena Seguinte")]
    public string nomeDaFase = "fase_1";

    private int paginaAtual = 0;

    void Start()
    {
        // Começa atualizando a tela para a primeira imagem
        AtualizarImagem();
    }

    void Update()
    {
        // Pega o teclado atual
        if (Keyboard.current != null)
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

            // Pular cutscene com 'Espaço'
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                PularParaOJogo();
            }
        }
    }

    public void PassarPagina()
    {
        paginaAtual++;

        if (paginaAtual < paginas.Length)
        {
            AtualizarImagem();
        }
        else
        {
            // Se passar da última página, o jogo começa
            PularParaOJogo();
        }
    }

    public void VoltarPagina()
    {
        paginaAtual--;

        // Trava no zero para o jogador não bugar o jogo tentando voltar antes do início
        if (paginaAtual < 0)
        {
            paginaAtual = 0;
        }

        AtualizarImagem();
    }

    // Criamos essa função para não repetir código toda vez que mudar de página
    private void AtualizarImagem()
    {
        if (paginas.Length > 0 && imagemNaTela != null)
        {
            imagemNaTela.sprite = paginas[paginaAtual];
        }
    }

    public void PularParaOJogo()
    {
        SceneManager.LoadScene(nomeDaFase);
    }
}