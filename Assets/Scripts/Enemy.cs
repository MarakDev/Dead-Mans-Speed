using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class Enemy : MonoBehaviour
{
    [Header("Skins")]
    [SerializeField] private List<RuntimeAnimatorController> listOfSkins;

    [Header("IA")]
    [SerializeField] public float playTime;
    [SerializeField] public float responseTime;

    [Header("")]
    [SerializeField] public CardDisplay enemyCardDisplay;
    [SerializeField] public GameObject exclamaciones;

    private Animator animator;

    [HideInInspector] private bool playCard;
    [HideInInspector] private bool responseCard;
    [HideInInspector] public int life;

    void Start()
    {
        animator = GetComponent<Animator>();

        exclamaciones.SetActive(false);
        playCard = true;
        responseCard = true;
        life = 4;
    }

    void Update()
    {
        if (instance.currentState == GameState.EnemyTurn)
        {
            if(playCard && !instance.SameCardOnTable())
                StartCoroutine(EnemyPlayCard());
        }

        if (responseCard && instance.SameCardOnTable() && (instance.currentState == GameState.PlayerTurn ||
            instance.currentState == GameState.EnemyTurn))
        {
            StopAllCoroutines();
            StartCoroutine(EnemyResponseCard());
        }

        if (instance.currentState == GameState.Electrified)
        {
            playCard = true;
            responseCard = true;
            StopAllCoroutines();
        }
    }

    public void ChangeTurn()
    {
        instance.currentState = GameState.PlayerTurn;

        instance.uI_Hourglass.ContinueHourglass();
        instance.AddMultToCurrentRound();
    }

    private IEnumerator EnemyPlayCard()
    {
        playCard = false;

        float winMult = instance.winCounter / 50;

        float minVal = (playTime - (0.4f + winMult)) / instance.currentSpeedMultRound;
        float maxVal = (playTime + (0.4f - winMult)) / instance.currentSpeedMultRound;

        yield return new WaitForSeconds(Random.Range(minVal, maxVal));

        instance.roundCounter++;

        animator.Play("");
        animator.Play("Enemy_PutCard");
        enemyCardDisplay.SetCard(instance.GetCard());
        AudioManager.instance.PlayOneShot(AudioManager.instance.sound_Put_Card);


        ChangeTurn();

        playCard = true;
        responseCard = true;
    }

    private IEnumerator EnemyResponseCard()
    {
        playCard = false;
        responseCard = false;

        float minVal = 1 - (instance.winCounter / 35);
        float maxVal = responseTime / instance.currentSpeedMultRound;
        if(maxVal < minVal)
            maxVal = minVal;

        yield return new WaitForSeconds(Random.Range(minVal, maxVal));

        exclamaciones.SetActive(true);

        instance.PlayerElectrocuted();

        playCard = true;
        responseCard = true;
    }

    public void ElectrocutedAnim()
    {
        animator.Play("");
        animator.Play("Enemy_Electrified");
    }

    [ContextMenu("ResetEnemy")]
    public void ResetEnemy()
    {
        if (GameManager.instance.winCounter > 1)
        {
            animator.runtimeAnimatorController = listOfSkins[Random.Range(0, 3)];
        }

        if (GameManager.instance.winCounter == 9)
        {
            animator.runtimeAnimatorController = listOfSkins[3];
            AudioManager.instance.PlayMusic(AudioManager.instance.music_BossTheme);
        }

        if(GameManager.instance.winCounter > 9)
            AudioManager.instance.PlayMusic(AudioManager.instance.music_MainTheme);

        life = 4;
        animator.Play("Enemy_RiseUp");

        AudioManager.instance.PlayOneShot(AudioManager.instance.sound_Cavar);

    }

    public void DefaultSkinEnemy()
    {
        animator.runtimeAnimatorController = listOfSkins[0];
    }

    public void ResetIA()
    {
        playCard = true;
        responseCard = true;
        exclamaciones.SetActive(false);
    }


    public void ClearCardDisplay()
    {
        enemyCardDisplay.ClearCards();
    }

    public void EnemyStopAllCorrutines()
    {
        this.StopAllCoroutines();
    }
    

}
