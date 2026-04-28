using System.Collections;
using UnityEngine;

public class EsconderSala : MonoBehaviour
{
    [Tooltip("Arraste o PREFAB da Tela Preta aqui")]
    public GameObject prefabTelaPreta;

    [Tooltip("Tempo em segundos para a tela clarear/escurecer")]
    public float tempoDeTransicao = 1f;

    private SpriteRenderer spriteTelaPreta;
    private Coroutine transicaoAtual; // Guarda a animação atual para podermos cancelar se o player entrar e sair rápido

    void Start()
    {
        BoxCollider2D colisor = GetComponent<BoxCollider2D>();
        Vector3 posZZero = new Vector3(colisor.bounds.center.x, colisor.bounds.center.y, 0f);

        GameObject clone = Instantiate(prefabTelaPreta, posZZero, Quaternion.identity);
        clone.transform.localScale = new Vector3(colisor.bounds.size.x, colisor.bounds.size.y, 1f);

        // Pegamos o SpriteRenderer para poder mexer na cor/transparência
        spriteTelaPreta = clone.GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && spriteTelaPreta != null)
        {
            if (transicaoAtual != null) StopCoroutine(transicaoAtual);
            transicaoAtual = StartCoroutine(FazerTransicaoAlpha(0f)); // 0f = 100% Transparente
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && spriteTelaPreta != null)
        {
            if (transicaoAtual != null) StopCoroutine(transicaoAtual);
            transicaoAtual = StartCoroutine(FazerTransicaoAlpha(1f)); // 1f = Totalmente Preto
        }
    }

    private IEnumerator FazerTransicaoAlpha(float alphaAlvo)
    {
        Color corAtual = spriteTelaPreta.color;

        // Enquanto a transparência atual não for igual à desejada...
        while (!Mathf.Approximately(corAtual.a, alphaAlvo))
        {
            // Move o alpha suavemente na velocidade configurada
            corAtual.a = Mathf.MoveTowards(corAtual.a, alphaAlvo, (1f / tempoDeTransicao) * Time.deltaTime);
            spriteTelaPreta.color = corAtual;

            yield return null; // Espera o próximo frame
        }
    }
}