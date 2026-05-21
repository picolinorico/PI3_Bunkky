using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ControleDeVolume : MonoBehaviour
{
    [Header("Conexão com o Mixer")]
    public AudioMixer mixerPrincipal;

    [Header("Sliders da UI")]
    public Slider sliderGeral;
    public Slider sliderMusica;
    public Slider sliderEfeitos;

    void Start()
    {
        // 1. CARREGANDO OS DADOS SALVOS
        // O "1f" no final significa: se for a primeira vez abrindo o jogo e não tiver nada salvo, coloque no máximo (1).
        float volGeralSalvo = PlayerPrefs.GetFloat("SalvarVolGeral", 1f);
        float volMusicaSalvo = PlayerPrefs.GetFloat("SalvarVolMusica", 1f);
        float volEfeitosSalvo = PlayerPrefs.GetFloat("SalvarVolEfeitos", 1f);

        // 2. ATUALIZANDO AS BARRINHAS
        // Quando a gente muda o "value" do slider pelo código, ele automaticamente chama a função de MudarVolume lá embaixo!
        if (sliderGeral != null) sliderGeral.value = volGeralSalvo;
        if (sliderMusica != null) sliderMusica.value = volMusicaSalvo;
        if (sliderEfeitos != null) sliderEfeitos.value = volEfeitosSalvo;
    }

    // --- FUNÇÕES CHAMADAS PELOS SLIDERS ---

    public void MudarVolumeGeral(float valor)
    {
        mixerPrincipal.SetFloat("VolumeGeral", Mathf.Log10(valor) * 20);

        // 3. SALVANDO O VALOR NO COMPUTADOR DO JOGADOR
        PlayerPrefs.SetFloat("SalvarVolGeral", valor);
    }

    public void MudarVolumeMusica(float valor)
    {
        mixerPrincipal.SetFloat("VolumeMusica", Mathf.Log10(valor) * 20);

        // 3. SALVANDO O VALOR NO COMPUTADOR DO JOGADOR
        PlayerPrefs.SetFloat("SalvarVolMusica", valor);
    }

    public void MudarVolumeEfeitos(float valor)
    {
        mixerPrincipal.SetFloat("VolumeEfeitos", Mathf.Log10(valor) * 20);

        // 3. SALVANDO O VALOR NO COMPUTADOR DO JOGADOR
        PlayerPrefs.SetFloat("SalvarVolEfeitos", valor);
    }
}