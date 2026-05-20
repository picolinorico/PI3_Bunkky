using UnityEngine;
using UnityEngine.SceneManagement; // Essencial para trocar de cenas!

public class MenuPrincipal : MonoBehaviour
{
    [Header("Telas do Menu")]
    public GameObject menuInicialUI;   // O grupo com os botões principais
    public GameObject painelCreditos; // O painel dos créditos
    public GameObject painelOpcoes; // O painel das opções

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