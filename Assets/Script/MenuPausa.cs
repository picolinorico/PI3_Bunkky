using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    // Variável estática para outros scripts saberem se o jogo tá pausado
    public static bool jogoPausado = false;

    [Header("Paineis")]
    public GameObject menuPausaUI;
    public GameObject painelConfiguracoes; // Painel de Configurações

    void Update()
    {
        // Atalho rápido: ESC pausa ou despausa
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (jogoPausado) Retomar(); else Pausar();
        }
    }

    public void Retomar()
    {
        menuPausaUI.SetActive(false); // Desliga a caixinha do Painel (esconde)
        Time.timeScale = 1f;          // O tempo do jogo volta a correr normal (1x)
        jogoPausado = false;
    }

    public void AbrirConfig()
    {
        menuPausaUI.SetActive(false);
        painelConfiguracoes.SetActive(true); // Liga as opções
    }

    public void FecharConfig()
    {
        painelConfiguracoes.SetActive(false); // Desliga as opções
        menuPausaUI.SetActive(true);
    }

    private void Pausar()
    {
        menuPausaUI.SetActive(true);  // Liga a caixinha do Painel (mostra na tela)
        Time.timeScale = 0f;          // Congela o tempo da física da Unity (0x)
        jogoPausado = true;
    }

    public void Menu()
    {
        Time.timeScale = 1f; // NUNCA esqueça de resetar o tempo antes de mudar de cena
        SceneManager.LoadScene(0); // Carrega o menu principal
    }
}
