using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    // Variável estática para outros scripts saberem se o jogo tá pausado
    public static bool jogoPausado = false;

    [Header("Arraste o seu PANEL aqui")]
    public GameObject menuPausaUI;

    void Update()
    {
        // Checa se a tecla ESC foi apertada
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (jogoPausado)
            {
                Retomar();
            }
            else
            {
                Pausar();
            }
        }
    }

    public void Retomar()
    {
        menuPausaUI.SetActive(false); // Desliga a caixinha do Painel (esconde)
        Time.timeScale = 1f;          // O tempo do jogo volta a correr normal (1x)
        jogoPausado = false;
    }

    private void Pausar()
    {
        menuPausaUI.SetActive(true);  // Liga a caixinha do Painel (mostra na tela)
        Time.timeScale = 0f;          // Congela o tempo da física da Unity (0x)
        jogoPausado = true;
    }

    // Função para o botão "Menu Principal"
    public void CarregarMenuPrincipal()
    {
        Time.timeScale = 1f; // MUITO IMPORTANTE: Descongelar o tempo antes de sair, senão o menu inicial fica travado!
        jogoPausado = false;

        // Carrega a cena zero (que configuramos no Build Profiles como o seu Menu Principal)
        SceneManager.LoadScene(0);
    }
}
