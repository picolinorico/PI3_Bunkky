using UnityEngine;

public class TrocarModoCamera : MonoBehaviour
{
    public CameraSeguir gerenciador;
    public BoxCollider2D quadrinhoAnterior;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // A sua ideia na prática: checamos o estado real da câmera!
            if (gerenciador.modoSeguir == false)
            {
                // Se NÃO está seguindo, significa que veio do corredor e vai entrar na rampa
                gerenciador.AtivarModoSeguir();
            }
            else
            {
                // Se JÁ ESTÁ seguindo, significa que está voltando da rampa pro corredor
                gerenciador.FocarNoQuadrinho(quadrinhoAnterior);
            }
        }
    }
}