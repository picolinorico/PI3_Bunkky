using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Câmera deste Checkpoint")]
    // Arraste o BoxCollider2D do quadrinho onde este checkpoint está
    public BoxCollider2D areaDesteQuadrinho;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                // Agora enviamos a posição E a área do quadrinho!
                player.AtualizarCheckpoint(transform.position, areaDesteQuadrinho);
            }
        }
    }
}