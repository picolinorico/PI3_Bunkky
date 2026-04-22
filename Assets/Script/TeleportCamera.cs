using UnityEngine;

public class TeleportCamera : MonoBehaviour
{
    public CameraSeguir gerenciador; // Arraste a Main Camera aqui
    public BoxCollider2D areaA;
    public BoxCollider2D areaB;

    [Header("Configuração de Cooldown")]
    public float tempoDeEspera = 0.8f; // Tempo de espera para evitar bugs na porta
    private float proximaTrocaDisponivel = 0f;

    private bool estaNoA = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Verifica se o tempo atual do jogo já passou do tempo de bloqueio
            if (Time.time >= proximaTrocaDisponivel)
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

                // Trava a porta pelo tempo de espera
                proximaTrocaDisponivel = Time.time + tempoDeEspera;
            }
        }
    }
}