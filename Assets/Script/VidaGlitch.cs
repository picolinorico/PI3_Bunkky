using UnityEngine;

// IMPORTANTE: Adicione o : IDamageable aqui para o script "assinar o contrato"
public class VidaGlitch : MonoBehaviour, IDamageable
{
    [Header("Configurações")]
    public int vidaMaxima = 3;
    private int vidaAtual;

    void Start()
    {
        vidaAtual = vidaMaxima;
    }

    // Este é o método que a Interface exige. 
    // Quando o Player chamar TakeDamage, ele vai executar o seu ReceberDano.
    public void TakeDamage(int amount)
    {
        ReceberDano(amount);
    }

    public void ReceberDano(int dano)
    {
        vidaAtual -= dano;
        Debug.Log("Glitch tomou dano! Vida restante: " + vidaAtual);

        if (vidaAtual <= 0)
        {
            Quebrar();
        }
    }

    private void Quebrar()
    {
        Debug.Log("Glitch destruído!");
        Destroy(gameObject);
    }
}