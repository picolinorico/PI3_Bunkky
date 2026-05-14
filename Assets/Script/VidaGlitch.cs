using UnityEngine;

// IMPORTANTE: Adicione o : IDamageable aqui para o script "assinar o contrato"
public class VidaGlitch : MonoBehaviour, IDamageable
{
    [Header("Configurações")]
    private int vidaGlitch = 1;
    [Header("Efeitos")]
    [SerializeField] private GameObject particulaPrefab;

    public void TakeDamage(int dano)
    {
        vidaGlitch -= dano;
         Quebrar();
    }

    private void Quebrar()
    {
        Debug.Log("Glitch destruído!");
        if (particulaPrefab != null)
        {
            // Cria as partículas na posição do objeto
            GameObject particulas = Instantiate(particulaPrefab, transform.position, Quaternion.identity);

            // Opcional: Se o seu prefab não tiver o "Stop Action" como Destroy, 
            // você força a destruição dele após 2 segundos para não pesar o jogo
            Destroy(particulas, 2f);
        }
        Destroy(gameObject);
        
    }
}