using UnityEngine;

public class TeleportCamera : MonoBehaviour
{
    [Header("Configurações de Posição")]
    public Transform cameraTransform;
    public Transform posicaoFixaA; // A posição original
    public Transform posicaoFixaB; // A nova posição

    private bool estaNaPosicaoA = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (estaNaPosicaoA)
            {
                // Vai para a posição B
                MoverCamera(posicaoFixaB.position);
                estaNaPosicaoA = false;
            }
            else
            {
                // Volta para a posição A
                MoverCamera(posicaoFixaA.position);
                estaNaPosicaoA = true;
            }
        }
    }

    private void MoverCamera(Vector3 novoDestino)
    {
        // No 2D, a câmera geralmente precisa estar em Z = -10
        cameraTransform.position = new Vector3(novoDestino.x, novoDestino.y, -10f);
 
    }
}