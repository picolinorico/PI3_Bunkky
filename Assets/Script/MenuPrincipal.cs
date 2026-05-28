using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // Adicionado para usar o teclado novo!

public class MenuPrincipal : MonoBehaviour
{
    [Header("Telas do Menu")]
    public GameObject menuInicialUI;   // O grupo com os botões principais
    public GameObject painelCreditos; // O painel dos créditos
    public GameObject painelOpcoes; // O painel das opções

    void Update()
    {
        // Verifica se o jogador apertou o ESC
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            // Se o painel de Créditos estiver aberto, fecha ele
            if (painelCreditos != null && painelCreditos.activeSelf)
            {
                FecharCreditos();
            }
            // Se o painel de Opções estiver aberto, fecha ele
            else if (painelOpcoes != null && painelOpcoes.activeSelf)
            {
                FecharOpcoes();
            }
        }
    }

    // Função para o botão de Jogar
    public void JogarJogo()
    {
        // Carrega a próxima cena na fila do Build Settings
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // --- ÁREA DOS CRÉDITOS ---
    public void AbrirCreditos()
    {
        menuInicialUI.SetActive(false);
        painelCreditos.SetActive(true);
        painelOpcoes.SetActive(false);
    }

    public void FecharCreditos()
    {
        painelCreditos.SetActive(false);
        menuInicialUI.SetActive(true);
        painelOpcoes.SetActive(false);
    }

    // --- ÁREA DAS OPÇÕES ---
    public void AbrirOpcoes()
    {
        menuInicialUI.SetActive(false);
        painelCreditos.SetActive(false);
        painelOpcoes.SetActive(true); // Liga as opções
    }

    public void FecharOpcoes()
    {
        painelOpcoes.SetActive(false); // Desliga as opções
        menuInicialUI.SetActive(true);
        painelCreditos.SetActive(false);
    }

    // Função para o botão de Sair
    public void SairDoJogo()
    {
        Debug.Log("O jogo fechou!"); // Mostra no console (pois o Quit não funciona no Editor)
        Application.Quit(); // Fecha o jogo de verdade quando estiver exportado
    }
}