using UnityEngine;

public class TrocarModoCamera : MonoBehaviour
{
    public CameraSeguir gerenciador;
    public BoxCollider2D quadrinhoAnterior;

    [Header("Configuração de Cooldown")]
    public float tempoDeEspera = 1.0f; // 1 segundo de intervalo para a rampa
    private float proximaTrocaDisponivel = 0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Checa se já passou tempo suficiente desde a última troca
            if (Time.time >= proximaTrocaDisponivel)
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

                // Define que a próxima troca só pode acontecer daqui a 1 segundo
                proximaTrocaDisponivel = Time.time + tempoDeEspera;
            }
        }
    }
}