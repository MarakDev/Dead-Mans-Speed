using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;


    [Header("I N P U T S")]
    public KeyCode input_A = KeyCode.Z;
    public KeyCode input_B = KeyCode.X;

    [HideInInspector] public List<Card> deck;
    [HideInInspector] public GameState currentState;

    [Header("Multiplicador tiempo ronda")]
    [SerializeField] public float factorMult = 0.1f;
    [SerializeField] public float maxSpeedMultRonda = 5;

    [Header("Rayos")]
    [SerializeField] public GameObject thunderPlayer;
    [SerializeField] public GameObject thunderEnemy;

    [Header("Tomb Pointer")]
    [SerializeField] public List<SpriteRenderer> tombPointer;
    private Sprite tombArrow;
    private Sprite tombNoArrow;

    [Header("Tomb Hands")]
    [SerializeField] public List<GameObject> allTombHands;

    [Header("Arbitro")]
    [SerializeField] public GameObject arbitroDecoration;

    [Header("Life UI")]
    [SerializeField] public UI_LifePoint uI_playerLifePoints;
    [SerializeField] public UI_LifePoint uI_enemyLifePoints;

    [Header("Timer")]
    [SerializeField] public Slider uI_playerTimeLeft;
    [SerializeField] public UI_Hourglass uI_Hourglass;

    [Header("WinCounter")]
    [SerializeField] public UI_WinText uI_WinText;

    [Header("GameOverScreen")]
    [SerializeField] public UI_StartRound startRound;
    [SerializeField] public LightController lightController;
    [SerializeField] public GameObject mainMenuIntro;

    [HideInInspector] public Enemy enemy;
    private Player player;

    private Card playerActualCard;
    private Card enemyActualCard;

    //timer del player
    private float playerTimeLeft;

    //---------------------------------------------

    //contador de rondas 
    [HideInInspector] public int roundCounter;
    //multiplicador de dificultad de ronda
    [HideInInspector] public float currentSpeedMultRound;
    //contador de victorias
     public float winCounter = 0;

    public enum GameState
    {
        PlayerTurn,
        EnemyTurn,
        Pause,
        WaitForStartRound,
        Electrified
    }

    void Awake()
    {
        instance = this;

        thunderPlayer.SetActive(false);
        thunderEnemy.SetActive(false);

        Sprite[] subSprites = Resources.LoadAll<Sprite>("Sprites/Decoration/LapidaMarcadora");
        tombArrow = subSprites[0];
        tombNoArrow = subSprites[1];

        enemy = FindAnyObjectByType<Enemy>();
        player = FindAnyObjectByType<Player>();

        currentState = GameState.Pause;

        //mainMenuIntro.SetActive(true);

        roundCounter = 0;

        BuildDeck();
    }

    void Start()
    {
        //STARTGAME = true; //cambiar esto al manager de menus

        arbitroDecoration.GetComponent<Animator>().Play("Arbitro_GroundedIdle");

    }

    void Update()
    {
        UpdateGameState();

        WakeUpTombHands();
    }

    private int limitRound;
    private void NoPairsLimitRound()
    {
        if(roundCounter >= limitRound)
        {
            deck = new List<Card>();
            Card[] allCards = Resources.LoadAll<Card>("Prefabs");
            Card randomCard = allCards[Random.Range(0, allCards.Length - 1)];

            for (int i = 0; i < 10; i++) //se toman 10 cartas aleatorias de toda la baraja total
            {
                deck.Add(randomCard);
            }

        }
    }

    [HideInInspector] public bool STARTGAME = false;
    public void UpdateGameState()
    {
        if (currentState == GameState.Pause && STARTGAME)
        {
            if (Input.GetKeyDown(input_B))
            {
                AudioManager.instance.PlayOneShot(AudioManager.instance.sound_Menu_Play);
                StartNewGame();
            }
        }
        else if (currentState == GameState.PlayerTurn)
        {
            ChangeArrows();

            ReduceTimer();

            ZeroInTimer();

            NoPairsLimitRound();
        }
        else if (currentState == GameState.EnemyTurn)
        {
            ChangeArrows();

            ZeroInTimer();
        }
        else if (currentState == GameState.WaitForStartRound)
        {
            tombPointer[0].sprite = tombNoArrow;
            tombPointer[1].sprite = tombNoArrow;

            StartNewRound();
        }

    }

    private void StartNewRound()
    {
        if (Input.GetKeyDown(input_B) && playRoundFlag) //inicia la partida con el boton B
        {
            playRoundFlag = false;
            randomTimeManos = Random.Range(15, 18);

            //Reset Round
            arbitroDecoration.GetComponent<Animator>().Play("Arbitro_GetOut");
            AudioManager.instance.PlayOneShot(AudioManager.instance.sound_Campana, 0.5f);

            currentSpeedMultRound = 1 + (winCounter * 0.5f); //multiplicador de velocidad de ronda

            blockTombHandsFlag = true; //variable de las manos tumba
            roundCounter = 0; //contador de rondas se iguala a 0 para reiniciarse
            uI_playerTimeLeft.value = uI_playerTimeLeft.maxValue; //se reinicia el timer
            uI_Hourglass.ContinueHourglass();   //activa la animacion del reloj

            limitRound = Random.Range(35, 45);

            startRound.gameObject.SetActive(true);

            //reset enemigo por si acaso
            enemy.ResetIA();

            //Activa el jugar la ronda
            StartCoroutine(PlayRound());
        }
    }

    public void StartNewGame()
    {

        STARTGAME = false;
        mainMenuIntro.SetActive(false);
        //limpia las cartas de la mesa
        player.ClearCardDisplay();
        enemy.ClearCardDisplay();
        winCounter = 0;
        uI_WinText.RestartWinText();

        uI_enemyLifePoints.ResetLife();
        uI_playerLifePoints.ResetLife();

        player.life = 4;
        enemy.life = 4;

        enemy.DefaultSkinEnemy();


        playRoundFlag = true;

        player.GetComponent<Animator>().Play("Player_Idle");

        AudioManager.instance.PlayMusic(AudioManager.instance.music_MainTheme);
        AudioManager.instance.ResetPitch();

        StartCoroutine(lightController.TurnOnLights());
        
    }

    private bool playRoundFlag = false;
    private IEnumerator PlayRound()
    {
        yield return new WaitForSeconds(0.15f);
        currentState = GameState.PlayerTurn;

        playRoundFlag = true;
    }

    //enemigo o player electrocutado
    public void PlayerElectrocuted()
    {
        currentState = GameState.Electrified;

        enemy.EnemyStopAllCorrutines();

        player.life--;

        //animacion trueno
        thunderPlayer.SetActive(true);
        thunderPlayer.GetComponent<Animator>().Play("Thunder");
        StartCoroutine(lightController.ThunderLight());
        AudioManager.instance.PlayOneShot(AudioManager.instance.sound_Thunder);

        //animacion esqueleto
        player.ElectrocutedAnim();
        AudioManager.instance.PlayOneShot(AudioManager.instance.sound_Character_Electrified);

        //desactivacion del trueno
        if (player.life == 0)
            StartCoroutine(ResetGame());
        else
            StartCoroutine(ResetRound(thunderPlayer, true));
    }

    public void EnemyElectrocuted()
    {
        currentState = GameState.Electrified;

        enemy.life--;

        //animacion trueno
        thunderEnemy.SetActive(true);
        thunderEnemy.GetComponent<Animator>().Play("Thunder");
        StartCoroutine(lightController.ThunderLight());
        AudioManager.instance.PlayOneShot(AudioManager.instance.sound_Thunder);

        //animacion esqueleto
        enemy.ElectrocutedAnim();
        AudioManager.instance.PlayOneShot(AudioManager.instance.sound_Character_Electrified);

        if (enemy.life == 0)
            StartCoroutine(ResetOponent());
        else
            //desactivacion del trueno
            StartCoroutine(ResetRound(thunderEnemy, false));

    }

    private IEnumerator ResetRound(GameObject thunder, bool playerOrNot)
    {
        //TimerVisual
        uI_Hourglass.RestartAnimation();
        uI_Hourglass.StopHourglass();
        uI_playerTimeLeft.value = uI_playerTimeLeft.maxValue;
        AudioManager.instance.ResetPitch();

        yield return new WaitForSeconds(0.2f);

        enemy.exclamaciones.SetActive(false);

        yield return new WaitForSeconds(0.3f);

        thunder.SetActive(false);

        AudioManager.instance.PlayOneShot(AudioManager.instance.sound_Character_Hit, 0.5f);

        if (!playerOrNot) { 
            uI_enemyLifePoints.LoseLife(enemy.life);        //actualiza la vida del enemigo
        }
        else
            uI_playerLifePoints.LoseLife(player.life);    //actualiza la vida del enemigo

        //manitas
        SleepTombHands();

        yield return new WaitForSeconds(1.5f);

        //limpia las cartas de la mesa
        player.ClearCardDisplay();
        enemy.ClearCardDisplay();

        playerActualCard = null;
        enemyActualCard = null;

        //actualiza los tomb pointer
        tombPointer[0].sprite = tombNoArrow;
        tombPointer[1].sprite = tombNoArrow;

        yield return new WaitForSeconds(0.5f);

        arbitroDecoration.GetComponent<Animator>().Play("Arbitro_GetUp");
        AudioManager.instance.PlayOneShot(AudioManager.instance.sound_Cavar);

        yield return new WaitForSeconds(0.75f);

        BuildDeck();
        currentState = GameState.WaitForStartRound;
    }

    private IEnumerator ResetOponent()
    {
        //TimerVisual
        uI_Hourglass.RestartAnimation();
        uI_Hourglass.StopHourglass();
        uI_playerTimeLeft.value = uI_playerTimeLeft.maxValue;
        AudioManager.instance.ResetPitch();

        yield return new WaitForSeconds(1.5f);

        thunderEnemy.SetActive(false);

        AudioManager.instance.PlayOneShot(AudioManager.instance.sound_Character_Hit, 0.5f);
        uI_enemyLifePoints.LoseLife(enemy.life);        //actualiza la vida del enemigo

        enemy.GetComponent<Animator>().Play("Enemy_Defeat");
        AudioManager.instance.PlayOneShot(AudioManager.instance.sound_Character_Death);

        yield return new WaitForSeconds(1f);

        //Añadimos una victoria al contador y sumamos en la ui
        uI_WinText.AddNumber();
        winCounter++;

        //recupera 1 vida
        if(player.life <= 3)
        {
            AudioManager.instance.PlayOneShot(AudioManager.instance.sound_Recover_Life);
            player.life++;
            uI_playerLifePoints.AddLife(player.life - 1);
        }

        //limpia las cartas de la mesa
        player.ClearCardDisplay();
        enemy.ClearCardDisplay();

        playerActualCard = null;
        enemyActualCard = null;

        //actualiza los tomb pointer
        tombPointer[0].sprite = tombNoArrow;
        tombPointer[1].sprite = tombNoArrow;

        yield return new WaitForSeconds(0.25f);

        //manitas
        SleepTombHands();

        yield return new WaitForSeconds(2.5f);

        enemy.ResetEnemy();
        //AudioManager.instance.PlayOneShot(AudioManager.instance.sound_Recover_Life);

        yield return new WaitForSeconds(3.25f);

        uI_enemyLifePoints.ResetLife();
        arbitroDecoration.GetComponent<Animator>().Play("Arbitro_GetUp");
        AudioManager.instance.PlayOneShot(AudioManager.instance.sound_Cavar);


        yield return new WaitForSeconds(1f);

        BuildDeck();
        currentState = GameState.WaitForStartRound;
    }

    private IEnumerator ResetGame()
    {

        currentState = GameState.Pause;

        //TimerVisual
        uI_Hourglass.RestartAnimation();
        uI_Hourglass.StopHourglass();
        uI_playerTimeLeft.value = uI_playerTimeLeft.maxValue;
        yield return new WaitForSeconds(0.25f);

        thunderPlayer.SetActive(false);

        uI_playerLifePoints.LoseLife(player.life);        //actualiza la vida del enemigo

        player.GetComponent<Animator>().Play("Player_Defeat");
        AudioManager.instance.PlayOneShot(AudioManager.instance.sound_Character_Death, 0.5f);
        AudioManager.instance.ChangeMusicPitch(0.8f);

        yield return new WaitForSeconds(1.5f);

        playerActualCard = null;
        enemyActualCard = null;

        //actualiza los tomb pointer
        tombPointer[0].sprite = tombNoArrow;
        tombPointer[1].sprite = tombNoArrow;

        yield return new WaitForSeconds(0.25f);

        //manitas
        SleepTombHands();

        yield return new WaitForSeconds(0.5f);

        //
        StartCoroutine(lightController.TurnOffLights());
        //
    }


    //add mult difficulty a la ronda
    public void AddMultToCurrentRound()
    {
        if (currentSpeedMultRound < maxSpeedMultRonda)
            currentSpeedMultRound += factorMult;
        else
            currentSpeedMultRound = maxSpeedMultRonda;

    }

    //same card
    public bool SameCardOnTable()
    {
        if (playerActualCard != null && enemyActualCard != null)
        {
            if (playerActualCard.name == enemyActualCard.name)
                return true;
            else
                return false;
        }
        return false;
    }

    #region TimerFunciones
    private void ZeroInTimer()
    {
        if(uI_playerTimeLeft.value == 0 && (instance.currentState == GameState.PlayerTurn ||
            instance.currentState == GameState.EnemyTurn))
        {
            PlayerElectrocuted();
        }
    }

    private void ReduceTimer()
    {
        playerTimeLeft += Time.deltaTime * (currentSpeedMultRound * 0.75f);

        if(playerTimeLeft >= 1)
        {
            AudioManager.instance.PlayOneShot(AudioManager.instance.sound_Time_Bar, 0.5f);

            uI_playerTimeLeft.value -= (int)playerTimeLeft;
            playerTimeLeft = 0;
        }
    }

    public void RegeneraTimer()
    {
        uI_playerTimeLeft.value += 2;
        uI_Hourglass.RestartAnimation();
        uI_Hourglass.StopHourglass();
    }

    #endregion

    //Cambia las tumbas para indicar cual es el pesronaje que debe jugar
    private void ChangeArrows()
    {
        if (currentState == GameState.PlayerTurn)
        {
            tombPointer[0].sprite = tombArrow;
            tombPointer[1].sprite = tombNoArrow;
        }
        else if(currentState == GameState.EnemyTurn)
        {
            tombPointer[1].sprite = tombArrow;
            tombPointer[0].sprite = tombNoArrow;
        }
    }

    /// <summary>
    /// Decks
    /// </summary>
    /// <returns></returns>

    //Toma una carta del mazo
    public Card GetCard()
    {
        if(deck.Count == 0)
            BuildDeck();

        Card firstCard = deck[0];

        if(currentState == GameState.PlayerTurn)
            playerActualCard = firstCard;
        else if(currentState == GameState.EnemyTurn)
            enemyActualCard = firstCard;

        deck.RemoveAt(0);

        return firstCard;
    }

    //Funciones para generar / regenerar el mazo
    public void BuildDeck()
    {
        deck = new List<Card>();
        Card[] allCards = Resources.LoadAll<Card>("Prefabs");

        for (int i = 0; i < 10 * (winCounter + 1); i++) //se toman 10 cartas aleatorias de toda la baraja total
        {
            int nCard = Random.Range(2, allCards.Length); //empeiza en 2 para excluir las cartas especiales

            //este metodo es solo para que sea muy poco probable obtener una de las cartas secretas
            if (allCards[nCard].name == "_CartaAya" || allCards[nCard].name == "_CartaTermita")
            {
                nCard = Random.Range(0, allCards.Length);
            }

            for (int j = 0; j < 2; j++) //numero de copias de cada carta, en este caso 3
            {
                deck.Add(allCards[nCard]);
            }
        }

        if(Random.Range(0, 3001) >= 3000)
        {
            int cartaEspesia = Random.Range(0, 2);

            deck.Add(allCards[cartaEspesia]);
            deck.Add(allCards[cartaEspesia]);

        }

        Suffle(deck);

        //foreach (Card c in deck)
        //{
        //    Debug.Log(c._name + "\n ");
        //}

        //Debug.Log(deck.Count);
    }

    private void Suffle(List<Card> lista)
    {
        for (int i = lista.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1); // Random.Range con enteros es inclusivo en el primer parámetro, exclusivo en el segundo
            Card temp = lista[i];
            lista[i] = lista[j];
            lista[j] = temp;
        }
    }

    #region Tombstone Hands

    bool blockTombHandsFlag = true;
    int randomTimeManos = 15;
    private void WakeUpTombHands()
    {
        if (roundCounter > randomTimeManos && blockTombHandsFlag)
        {
            AudioManager.instance.PlayOneShot(AudioManager.instance.sound_Cavar);

            blockTombHandsFlag = false;
            for (int i = 0; i < allTombHands.Count; i++)
            {
                allTombHands[Random.Range(0, allTombHands.Count)].GetComponent<Animator>().Play("Tombstone_WakeUp");
            }
        }
    }

    private void SleepTombHands()
    {
        if (!blockTombHandsFlag)
        {
            AudioManager.instance.PlayOneShot(AudioManager.instance.sound_Cavar, 0.5f);

            for (int i = 0; i < allTombHands.Count; i++)
            {
                Animator animator = allTombHands[i].GetComponent<Animator>();
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); // 0 es el índice de la capa

                if (stateInfo.IsName("Tombstone_WakeUpIdle"))
                {
                    animator.Play("Tombstone_Sleep");
                }
            }
        }
    }

    #endregion
}


