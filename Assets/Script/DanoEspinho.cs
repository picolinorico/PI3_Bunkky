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
            PlayerMovement scriptDoPlayer = collision.gameObject.GetComponent<PlayerMovement>();

            if (scriptDoPlayer != null)
            {
                // Manda o dano e a posição do espinho
                scriptDoPlayer.ReceberDano(danoCausado, transform.position);
            }
        }
    }
}