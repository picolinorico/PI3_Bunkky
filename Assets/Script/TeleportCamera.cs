using UnityEngine;

public class TeleportCamera : MonoBehaviour
{
    [Header("Câmera Principal")]
    public Camera cameraPrincipal;

    [Header("Áreas dos Quadrinhos (Use Box Colliders)")]
    public BoxCollider2D areaDoQuadrinhoA; // Substitui a antiga posicaoFixaA
    public BoxCollider2D areaDoQuadrinhoB; // Substitui a antiga posicaoFixaB

    // Variável do TeleportCamera para saber onde estamos
    private bool estaNoQuadrinhoA = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Debug do FocarCamera para te ajudar a testar
        Debug.Log("Algo bateu aqui: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Foi o Player! Alternando o foco...");

            // Lógica de Toggle do TeleportCamera
            if (estaNoQuadrinhoA)
            {
                // Vai para o quadrinho B
                FocarNaArea(areaDoQuadrinhoB);
                estaNoQuadrinhoA = false;
            }
            else
            {
                // Volta para o quadrinho A
                FocarNaArea(areaDoQuadrinhoA);
                estaNoQuadrinhoA = true;
            }
        }
    }

    // A função matemática do FocarCamera, mas agora ela aceita qualquer área que você mandar!
    private void FocarNaArea(BoxCollider2D areaDestino)
    {
        // 1. Pega o centro exato da área que foi pedida
        Vector3 centro = areaDestino.bounds.center;

        // Move a câmera para esse centro (mantendo o Z em -10)
        cameraPrincipal.transform.position = new Vector3(centro.x, centro.y, -10f);

        // 2. Calcula o zoom para caber na tela
        float proporcaoTela = (float)Screen.width / (float)Screen.height;
        float larguraArea = areaDestino.bounds.size.x;
        float alturaArea = areaDestino.bounds.size.y;

        // Verifica se precisa ajustar pela altura ou pela largura para não cortar nada
        if (larguraArea / alturaArea > proporcaoTela)
        {
            // O quadrinho é mais largo que a tela, ajusta pela largura
            cameraPrincipal.orthographicSize = (larguraArea / 2f) / proporcaoTela;
        }
        else
        {
            // O quadrinho é mais alto (ou igual) à tela, ajusta pela altura
            cameraPrincipal.orthographicSize = alturaArea / 2f;
        }
    }
}