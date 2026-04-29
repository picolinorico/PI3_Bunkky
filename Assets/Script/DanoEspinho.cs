using UnityEngine;

public class DanoEspinho : MonoBehaviour
{
    [Header("Configuração do Espinho")]
    public int danoCausado = 1;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Puxa o script mestre do Player
            PlayerHealth scriptDoPlayer = collision.gameObject.GetComponent<PlayerHealth>();

            if (scriptDoPlayer != null)
            {
                // Manda o dano e a posição do espinho
                scriptDoPlayer.TakeDamage(danoCausado);
            }
        }
    }
}