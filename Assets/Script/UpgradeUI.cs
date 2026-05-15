using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    [Header("Ícones dos Upgrades")]
    public Image iconeAtaque;
    public Image iconeWallcling;

    [Header("Cores de Status")]
    public Color corBloqueado = new Color(0.2f, 0.2f, 0.2f, 1f); // Cinza escuro
    public Color corDesbloqueado = Color.white; // Acende com a cor normal da imagem

    private void Start()
    {
        // Começa o jogo apagando as luzes dos ícones
        if (iconeAtaque != null) iconeAtaque.color = corBloqueado;
        if (iconeWallcling != null) iconeWallcling.color = corBloqueado;
    }

    // Métodos que o Player vai chamar quando matar os bichos
    public void LigarIconeAtaque()
    {
        if (iconeAtaque != null) iconeAtaque.color = corDesbloqueado;
    }

    public void LigarIconeWallcling()
    {
        if (iconeWallcling != null) iconeWallcling.color = corDesbloqueado;
    }
}