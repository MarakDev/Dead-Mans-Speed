using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_MenuPrincipal : MonoBehaviour
{
    [SerializeField] public GameObject transitionPage;
    [SerializeField] public GameObject backScreen;

    [SerializeField] public GameObject playButton;
    [SerializeField] public GameObject rulesButton;
    [SerializeField] public GameObject rulesPopUp;
    [SerializeField] public GameObject backButton;

    [HideInInspector] public Animator animator;

    public void Start()
    {
        animator = GetComponent<Animator>();

        transitionPage.SetActive(true);
        rulesPopUp.SetActive(false);
        transitionPage.GetComponent<Image>().color = new Color(0, 0, 0, 0f);

        StartCoroutine(StartIntroAnimation());

        AudioManager.instance.PlayMusic(AudioManager.instance.music_MenuTheme);
    }


    public void _PlayButton()
    {
        AudioManager.instance.PlayOneShot(AudioManager.instance.sound_Menu_Play);
        StartCoroutine(GameTransition());
    }

    public void _RulesButton()
    {
        AudioManager.instance.PlayOneShot(AudioManager.instance.sound_Menu_Play);
        StartCoroutine(RulesPopUp());
    }

    public void _RulesButtonGetBack()
    {
        AudioManager.instance.PlayOneShot(AudioManager.instance.sound_Menu_Play);
        StartCoroutine(RulesPopUpGetBack());
    }

    public IEnumerator StartIntroAnimation()
    {
        animator.Play("UI_MenuIntro");
        yield return new WaitForSeconds(3.5f);

        EventSystem.current.SetSelectedGameObject(playButton);
        transitionPage.SetActive(false);
    }

    public IEnumerator GameTransition()
    {
        EventSystem.current.SetSelectedGameObject(null);
        transitionPage.SetActive(true);

        yield return new WaitForSeconds(0.75f);
        transitionPage.GetComponent<Image>().color = new Color(0, 0, 0, 0.2f);
        AudioManager.instance.PlayWithPitch(AudioManager.instance.sound_Time_Bar, 0.25f);

        yield return new WaitForSeconds(0.75f);
        transitionPage.GetComponent<Image>().color = new Color(0, 0, 0, 0.4f);
        AudioManager.instance.PlayWithPitch(AudioManager.instance.sound_Time_Bar, 0.25f);

        yield return new WaitForSeconds(0.75f);
        transitionPage.GetComponent<Image>().color = new Color(0, 0, 0, 0.6f);
        AudioManager.instance.PlayWithPitch(AudioManager.instance.sound_Time_Bar, 0.25f);

        yield return new WaitForSeconds(0.75f);
        transitionPage.GetComponent<Image>().color = new Color(0, 0, 0, 0.8f);
        AudioManager.instance.PlayWithPitch(AudioManager.instance.sound_Time_Bar, 0.25f);

        yield return new WaitForSeconds(0.75f);
        transitionPage.GetComponent<Image>().color = new Color(0, 0, 0, 1f);
        AudioManager.instance.PlayWithPitch(AudioManager.instance.sound_Time_Bar, 0.25f);

        this.GetComponent<Image>().enabled = false;
        playButton.SetActive(false);
        rulesButton.SetActive(false);
        rulesPopUp.SetActive(false);
        backScreen.SetActive(false);

        yield return new WaitForSeconds(1f);
        transitionPage.SetActive(false);

        AudioManager.instance.PlayMusic(AudioManager.instance.music_MainTheme);

        GameManager.instance.STARTGAME = true;
        GameManager.instance.StartNewGame();


    }

    public IEnumerator RulesPopUp()
    {
        yield return new WaitForSeconds(0.05f);

        rulesPopUp.SetActive(true);
        EventSystem.current.SetSelectedGameObject(backButton);

    }

    public IEnumerator RulesPopUpGetBack()
    {
        yield return new WaitForSeconds(0.05f);

        rulesPopUp.SetActive(false);
        EventSystem.current.SetSelectedGameObject(playButton);

    }


}
