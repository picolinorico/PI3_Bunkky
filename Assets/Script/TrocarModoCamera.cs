using UnityEngine;

public class TrocarModoCamera : MonoBehaviour
{
    public CameraSeguir gerenciador; // Arraste a Main Camera aqui

    // Arraste aqui o BoxCollider do quadrinho que fica ANTES da rampa
    public BoxCollider2D quadrinhoAntesDaRampa;

    private bool entrouNaRampa = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!entrouNaRampa)
            {
                // Entrou na rampa: avisa a câmera para seguir
                gerenciador.AtivarModoSeguir();
                entrouNaRampa = true;
            }
            else
            {
                // Voltou da rampa: avisa a câmera para focar de volta no painel
                gerenciador.FocarNoQuadrinho(quadrinhoAntesDaRampa);
                entrouNaRampa = false;
            }
        }
    }
}