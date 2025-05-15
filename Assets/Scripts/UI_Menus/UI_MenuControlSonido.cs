using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;

public class UI_MenuControlSonido : MonoBehaviour
{
    [SerializeField] private GameObject UI_MusicController;
    [SerializeField] private GameObject UI_SFXController;

    [SerializeField] private List<Sprite> musicaControles;
    [SerializeField] private List<Sprite> musicaPressedControles;
    [SerializeField] private List<Sprite> sfxControles;
    [SerializeField] private List<Sprite> sfxPressedControles;
    void Start()
    {
        //empiezan en el valor maximo = 3;
        UI_MusicController.GetComponent<Image>().sprite = musicaControles[3];
        Button boton = UI_MusicController.GetComponent<Button>();
        SpriteState estado = boton.spriteState;
        estado.pressedSprite = musicaPressedControles[3];
        boton.spriteState = estado;

        UI_SFXController.GetComponent<Image>().sprite = sfxControles[3];

        boton = UI_SFXController.GetComponent<Button>();
        estado = boton.spriteState;
        estado.pressedSprite = sfxPressedControles[3];
        boton.spriteState = estado;

    }

    private int valorMusica = 3; // 3 es igual al valor maximo
    public void _ReducirVolumenMusica_UI()
    {
        if(valorMusica == 0)
        {
            valorMusica = 3;
            AudioManager.instance.SetMusicVolume(valorMusica);
        }
        else
        {
            valorMusica--;
            AudioManager.instance.SetMusicVolume(valorMusica);
        }

        UI_MusicController.GetComponent<Image>().sprite = musicaControles[valorMusica];
        Button boton = UI_MusicController.GetComponent<Button>();
        SpriteState estado = boton.spriteState;
        estado.pressedSprite = musicaPressedControles[valorMusica];
        boton.spriteState = estado;
    }

    private int valorSFX = 3; // 3 es igual al valor maximo
    public void _ReducirVolumenSFX_UI()
    {
        if (valorSFX == 0)
        {
            valorSFX = 3;
            AudioManager.instance.SetSFXVolume(valorSFX);
        }
        else
        {
            valorSFX--;
            AudioManager.instance.SetSFXVolume(valorSFX);
        }

        UI_SFXController.GetComponent<Image>().sprite = sfxControles[valorSFX];
        Button boton = UI_SFXController.GetComponent<Button>();
        SpriteState estado = boton.spriteState;
        estado.pressedSprite = sfxPressedControles[valorSFX];
        boton.spriteState = estado;
    }
}
