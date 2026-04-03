using UnityEngine;

public class TeleportCamera : MonoBehaviour
{
    public CameraSeguir gerenciador; // Arraste a Main Camera aqui
    public BoxCollider2D areaA;
    public BoxCollider2D areaB;

    private bool estaNoA = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (estaNoA)
            {
                gerenciador.FocarNoQuadrinho(areaB);
                estaNoA = false;
            }
            else
            {
                gerenciador.FocarNoQuadrinho(areaA);
                estaNoA = true;
            }
        }
    }
}