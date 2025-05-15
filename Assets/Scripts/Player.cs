using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class Player : MonoBehaviour
{
    [SerializeField] public CardDisplay playerCardDisplay;
    [SerializeField] public GameObject exclamaciones;

    [HideInInspector] public int life;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        life = 4;

        exclamaciones.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(instance.input_B) && instance.currentState == GameState.PlayerTurn)
        {
            if (instance.SameCardOnTable())
                instance.PlayerElectrocuted();
            else
            {
                AudioManager.instance.PlayOneShot(AudioManager.instance.sound_Put_Card);

                PlayCard();
                instance.currentState = GameState.EnemyTurn;
                instance.AddMultToCurrentRound();
                instance.RegeneraTimer();
            }
        }

        if (playerCardDisplay.IsACard())
        {
            if (Input.GetKeyDown(instance.input_A) && instance.SameCardOnTable() && (instance.currentState == GameState.PlayerTurn ||
                instance.currentState == GameState.EnemyTurn))
            {
                //arreglo del kerus bug
                StartCoroutine(ExclamacionesAnimacion());

                instance.enemy.EnemyStopAllCorrutines();
                instance.EnemyElectrocuted();
            }

            if (Input.GetKeyDown(instance.input_A) && !instance.SameCardOnTable() && (instance.currentState == GameState.PlayerTurn ||
                instance.currentState == GameState.EnemyTurn))
            {
                instance.PlayerElectrocuted();
            }
        }
    }

    public void PlayCard()
    {
        animator.Play("");
        animator.Play("Player_PutCard");
        playerCardDisplay.SetCard(instance.GetCard());
        instance.roundCounter++;

        AudioManager.instance.IntenseMusicOnRound(instance.roundCounter);
    }

    public void ElectrocutedAnim()
    {
        animator.Play("");
        animator.Play("Player_Electrified");
    }

    public void ClearCardDisplay()
    {
        playerCardDisplay.ClearCards();
    }

    public IEnumerator ExclamacionesAnimacion()
    {
        exclamaciones.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        exclamaciones.SetActive(false);
    }
}
