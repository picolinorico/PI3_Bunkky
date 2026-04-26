using UnityEngine;
using UnityEngine.SceneManagement; // Essencial para trocar de cenas!

public class MenuPrincipal : MonoBehaviour
{
    [Header("Telas do Menu")]
    public GameObject menuInicialUI;   // O grupo com os botões principais
    public GameObject painelCreditos; // O painel dos créditos

    // Função para o botão de Jogar
    public void JogarJogo()
    {
        // Carrega a próxima cena na fila do Build Settings
        // Você também pode usar o nome da cena, ex: SceneManager.LoadScene("Fase1");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void AbrirCreditos()
    {
        // Liga a cortina dos créditos
        menuInicialUI.SetActive(false);
        painelCreditos.SetActive(true);
    }

    public void FecharCreditos()
    {
        // Desliga a cortina dos créditos
        painelCreditos.SetActive(false);
        menuInicialUI.SetActive(true);
    }

    // Função para o botão de Sair
    public void SairDoJogo()
    {
        Debug.Log("O jogo fechou!"); // Mostra no console (pois o Quit não funciona no Editor)
        Application.Quit(); // Fecha o jogo de verdade quando estiver exportado
    }
}