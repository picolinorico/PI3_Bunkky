using UnityEngine;
using UnityEngine.SceneManagement; // Essencial para trocar de cenas!

public class MenuPrincipal : MonoBehaviour
{
    // Função para o botão de Jogar
    public void JogarJogo()
    {
        // Carrega a próxima cena na fila do Build Settings
        // Você também pode usar o nome da cena, ex: SceneManager.LoadScene("Fase1");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Função para o botão de Sair
    public void SairDoJogo()
    {
        Debug.Log("O jogo fechou!"); // Mostra no console (pois o Quit não funciona no Editor)
        Application.Quit(); // Fecha o jogo de verdade quando estiver exportado
    }
}