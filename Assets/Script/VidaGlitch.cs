using UnityEngine;

public class VidaGlitch : MonoBehaviour
{
    [Header("Configurações")]
    public int vidaMaxima = 3; // Quantos hits ele aguenta
    private int vidaAtual;

    void Start()
    {
        vidaAtual = vidaMaxima;
    }

    public void ReceberDano(int dano)
    {
        vidaAtual -= dano;
        Debug.Log("Glitch tomou dano! Vida restante: " + vidaAtual);

        // Se a vida zerar, ele quebra
        if (vidaAtual <= 0)
        {
            Quebrar();
        }
    }

    private void Quebrar()
    {
        Debug.Log("Glitch destruído!");
        // Destrói o objeto do jogo
        Destroy(gameObject);
    }
}