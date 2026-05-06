using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Pega o movimento e passa APENAS a posição
            PlayerMovement mov = other.GetComponent<PlayerMovement>();
            if (mov != null)
            {
                mov.AtualizarCheckpoint(transform.position);
            }
        }
    }
}