using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TransicaoMorte : MonoBehaviour
{
    [Header("Configurações")]
    public float tempoDeslize = 0.4f;
    public float tempoEspera = 0.8f;

    private RectTransform rectTransform;

    private Vector2 posEsquerda = new Vector2(-2500f, 0f);
    private Vector2 posCentro = Vector2.zero;
    private Vector2 posDireita = new Vector2(2500f, 0f);

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // O PlayerHealth chama essa função
    public void IniciarTransicao(PlayerMovement player)
    {
        // 1. Liga a imagem na Unity (para você poder deixar ela desativada no Editor!)
        gameObject.SetActive(true);

        StartCoroutine(RotinaMorte(player));
    }

    private IEnumerator RotinaMorte(PlayerMovement player)
    {
        // 2. TRAVA A COELHA IMEDIATAMENTE
        if (player != null)
        {
            player.enabled = false; // Desliga os botões do teclado

            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero; // Freia ela na hora

            Animator anim = player.GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetBool("Andando", false);
                anim.SetBool("Pulando", false);
            }
        }

        // 3. ENTRA NA TELA (Vem da esquerda pro centro)
        rectTransform.anchoredPosition = posEsquerda;
        yield return StartCoroutine(Deslizar(posEsquerda, posCentro));

        // 4. TELA COBERTA: Teleporta a coelha pro Checkpoint
        if (player != null)
        {
            player.Respawnar();
        }

        yield return new WaitForSeconds(tempoEspera);

        // 5. SAI DA TELA (Vai do centro pra direita)
        yield return StartCoroutine(Deslizar(posCentro, posDireita));

        // 6. DESTRAVA A COELHA
        if (player != null)
        {
            player.enabled = true; // Volta a ouvir o teclado
        }

        // 7. DESLIGA A IMAGEM 
        gameObject.SetActive(false);
    }

    private IEnumerator Deslizar(Vector2 inicio, Vector2 fim)
    {
        float tempo = 0f;
        while (tempo < tempoDeslize)
        {
            tempo += Time.deltaTime;
            float t = tempo / tempoDeslize;
            rectTransform.anchoredPosition = Vector2.Lerp(inicio, fim, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
        rectTransform.anchoredPosition = fim;
    }
}