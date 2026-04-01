using UnityEngine;

public class FocarCamera : MonoBehaviour
{
    [Header("Câmera Principal")]
    public Camera cameraPrincipal; // Arraste a Main Camera pra cá

    [Header("Configurações do Quadrinho Destino")]
    public Transform centroDoQuadrinho; // O centro do quadrinho para onde o player vai
    public float zoomDoQuadrinho = 5f;  // O tamanho ideal da câmera para esse quadrinho

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Quando o jogador passar pelo corredor/porta...
        if (other.CompareTag("Player"))
        {
            // 1. Centraliza a câmera no novo quadrinho (mantendo o Z em -10)
            cameraPrincipal.transform.position = new Vector3(centroDoQuadrinho.position.x, centroDoQuadrinho.position.y, -10f);

            // 2. Ajusta o "Orthographic Size" para dar o zoom exato do quadrinho
            cameraPrincipal.orthographicSize = zoomDoQuadrinho;
        }
    }
}