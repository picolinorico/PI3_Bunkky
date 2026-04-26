using UnityEngine;
using UnityEngine.SceneManagement;

public class MudarCena : MonoBehaviour
{
    [SerializeField] private string nomeDaCena;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Carrega a cena pelo nome que você digitar no Inspector
            SceneManager.LoadScene(nomeDaCena);
        }
    }
}
