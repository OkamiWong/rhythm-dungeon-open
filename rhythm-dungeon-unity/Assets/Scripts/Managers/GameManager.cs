using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Rhythm
{
    public int length;
    public float[] accurateTime;
    public int[] scaledTime;

    public Color color;
    public Rhythm(int[] scaledTimeDifferentiated, Color color)
    {
        this.color = color;

        length = scaledTimeDifferentiated.Length;
        scaledTime = new int[length];
        scaledTime[0] = scaledTimeDifferentiated[0];
        for (int i = 1; i < length; i++)
            scaledTime[i] = scaledTime[i - 1] + scaledTimeDifferentiated[i];
        var secondPerHalfBeat = 60f / GameManager.BPM / 2f;
        accurateTime = new float[length];
        for (int i = 0; i < length; ++i)
            accurateTime[i] = scaledTime[i] * secondPerHalfBeat;
    }
}

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Combat, Move, ChoosingItem, LevelUp, GameOver, Pause
    }

    public static GameManager Instance; //Singleton

    public static int BPM = 140;

    [Header("Prefabs")]
    public GameObject playerPrefab;
    public GameObject backgroundPrefab;
    public GameObject[] backgroundOfSecondStagePrefabs;
    public GameObject[] itemPrefabs;
    public Dictionary<string, int> nameToItemIndex;

    [Header("In the Scene")]
    public GameObject currentBackground;

    [Header("Tutorial")]
    public bool isTutorial = false;
    public bool isEndlessMode = false;
    //PlayerAction.Undefinded means wait for C pressed to continue
    private Queue<PlayerAction> tutorialPlayerAction = new Queue<PlayerAction>(new PlayerAction[] {
        PlayerAction.Undefinded,
        PlayerAction.Undefinded,
        PlayerAction.MoveDown,
        PlayerAction.Idle,
        PlayerAction.MoveUp,
        PlayerAction.Charge,
        PlayerAction.Attack,
        PlayerAction.Undefinded
    });

    private Queue<int> tutorialPanelsId = new Queue<int>(new int[] { 0, 1, 3, 7, 2, 4, 5, 6 });

    [HideInInspector]
    public float secondPerHalfBeat;
    [HideInInspector]
    public int combo = 0;
    [HideInInspector]
    private GameState gameState;
    public GameState GState { get { return gameState; } }

    public int HalfBeatCount { get; private set; }

    [HideInInspector]
    public PlayerAction playerAction;
    [HideInInspector]
    public EnemyAction tutorialEnemyAction;

    public int level;
    public int NumberOfCoins { get { return level + 1; } }

    GradeSetter.Grade playerGrade;

    float deltaTimeForInput, deltaTimeForBeat, lastMusicTime;

    float coolBound, fineBound;

    bool isFirstBar;
    bool readyForCombat;

    GameObject[] itemsToChoose = { null, null, null };

    //The struct to restore one press of keys.
    public struct KeyPress
    {
        public KeyCode key;
        public float time;
        public int keyIndex;
        public KeyPress(KeyCode key, float time, int keyIndex)
        {
            this.key = key;
            this.time = time;
            this.keyIndex = keyIndex;
        }
    }
    List<KeyPress> keyPresses;

    private void Awake()
    {
        //Set sigleton
        Instance = this;
        Instantiate(playerPrefab);
    }

    private void Start()
    {
        //Calculate basic numbers
        secondPerHalfBeat = 60f / (float)BPM / 2f;
        HalfBeatCount = -1;
        deltaTimeForInput = deltaTimeForBeat = lastMusicTime = 0f;
        //Instantiate containters
        keyPresses = new List<KeyPress>();
        //Initialize actions of player and enemy. (They are all set to idle.)
        playerAction = PlayerAction.Idle;
        tutorialEnemyAction = EnemyAction.Idle;

        //Set bounds of grades of inputs.
        coolBound = 0.05f;
        fineBound = 0.08f;
        //Debug.Log("Cool Bound = " + coolBound);
        //Debug.Log("Fine Bound = " + fineBound);

        if (isEndlessMode)
        {
            GameResult.Instance.LoadSavedPlayer();
            GameResult.Instance.beatenPlayers.Clear();
        }

        EnterGameState(GameState.Combat);

        //Initialize enemy sequence and spawn the first enemy. 
        EnemyManager.Instance.SpawnNextEnemy();

        OnHalfBeat();//the very first half beat

        if (isTutorial)
        {
            UIManager.Instance.EnableTutorial();
        }
        else
        {
            UIManager.Instance.DisableTutorial();
            nameToItemIndex = new Dictionary<string, int>();
            for (int i = 0; i < itemPrefabs.Length; ++i)
                nameToItemIndex[itemPrefabs[i].GetComponent<Item>().itemName] = i;
        }

        if (isTutorial)
            DataCollector.Instance.CodeAndSendEvent("tutorial_start", null);
        else
        {
            var eventParams = new Dictionary<string, string>();
            eventParams["is_endless_mode"] = isEndlessMode ? "true" : "false";
            DataCollector.Instance.CodeAndSendEvent("game_start", eventParams);
        }
    }

    //Get KeyDown every frame
    [HideInInspector]
    public GameState gameStateBeforePause;
    private void Update()
    {
        //Pause
        if (Input.GetKeyDown(InputSettings.PauseKey))
            PressedPause();

        if (gameState == GameState.Pause && UIManager.Instance.pauseGo.activeInHierarchy == false)
            ExitPause();

        if (gameState == GameState.Pause)
            return;

        //precess press C to continue in tutorial
        if (isTutorial)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                SkipTutorial();
        }

        //Update UI every frame.
        UIManager.Instance.UpdateUI();

        //Only when the player is combating and the white bar blinks does the manager accept inputs. 
        if (HalfBeatCount < 8 || gameState != GameState.Combat) return;
        deltaTimeForInput += Time.deltaTime;
        //Looking through all valid keys. 
        for (int j = 0; j < InputSettings.MusicKeys.Length; ++j)
            if (Input.GetKeyDown(InputSettings.MusicKeys[j]))
                Pressed(j);
    }

    public void Pressed(int keyIndex) //<10: MusicKey; >=10: Control Key
    {
        switch (keyIndex)
        {
            case 10: PressedPause(); return;
            case 11: SkipTutorial(); return;
            default:
                keyPresses.Add(new KeyPress(InputSettings.MusicKeys[keyIndex], deltaTimeForInput, keyIndex));
                UIManager.Instance.ShowCharacterOrArrow(keyIndex);
                AudioManager.Instance.PlayDrum(keyIndex);
                return;
        }
    }

    public void PressedPause()
    {
        if (gameState != GameState.Pause)
        {
            UIManager.Instance.Pause();
            AudioManager.Instance.Pause();

            gameStateBeforePause = gameState;
            gameState = GameState.Pause;
            Time.timeScale = 0;
        }
        else ExitPause();
    }

    public void SkipTutorial()
    {
        ScenesManager.Instance.LoadLoadingScene("RD_0");
        AudioManager.Instance.StopMusic();

        DataCollector.Instance.CodeAndSendEvent("tutorial_skipped", null);
    }

    void FixedUpdate()
    {
        //Timer for half beat
        //Use this rather complex method to control the time error of coroutine. 
        if (AudioManager.Instance.isMusicPlay)
        {
            var currentMusicTime = AudioManager.Instance.background.time;
            if (currentMusicTime < lastMusicTime)
                lastMusicTime -= AudioManager.Instance.background.clip.length;
            deltaTimeForBeat += currentMusicTime - lastMusicTime;
            lastMusicTime = currentMusicTime;
            if (deltaTimeForBeat > secondPerHalfBeat)
            {
                deltaTimeForBeat -= secondPerHalfBeat;
                OnHalfBeat();
            }
        }
        else
        {
            deltaTimeForBeat += Time.fixedDeltaTime;
            lastMusicTime = 0f;
            if (deltaTimeForBeat > secondPerHalfBeat)
            {
                deltaTimeForBeat -= secondPerHalfBeat;
                OnHalfBeat();
            }
        }
    }

    //When the main game or tutorial is closed
    void OnDestroy()
    {
        if (gameState == GameState.Pause)
            Time.timeScale = 1f;
    }

    void ExitPause()
    {
        UIManager.Instance.Continue();
        AudioManager.Instance.Continue();

        gameState = gameStateBeforePause;
        Time.timeScale = 1f;
    }

    //GameOver method
    public void GameOver()
    {
        EnterGameState(GameState.GameOver);
        if (isEndlessMode)
        {
            var blockchainEnemyStats = (BlockchainEnemyStats)EnemyManager.CurrentEnemyControllers[0].enemyStats;
            if (blockchainEnemyStats.nameHolder.gameObject.activeSelf)
                GameResult.Instance.beatenPlayers.Remove(blockchainEnemyStats.nameHolder.text);
            UIManager.Instance.ShowEndlessModeStatistics();
        }
        UIManager.Instance.GameOver(); //Show GameOver screen

        if (EnemyManager.CurrentEnemyControllers.Count > 0
            && EnemyManager.CurrentEnemyControllers[0].enemyStats is BlockchainEnemyStats)
        {
            if (((BlockchainEnemyStats)EnemyManager.CurrentEnemyControllers[0].enemyStats).fromBlockChainText.gameObject.activeSelf)
                GenesisContractService.Instance.Reward(GenesisContractService.Instance.genesisIndex, NumberOfCoins);
        }
    }


    public bool isRevived = false;
    void ReviveAndContinue()
    {
        isRevived = false;
        PlayerStats.Instance.Recover();

        DataCollector.Instance.CodeAndSendEvent("player_revive", null);
    }

    public void PlayerWin()
    {
        EnterGameState(GameState.GameOver);
        level += 1;
        GameResult.Instance.SaveCurrentPlayer();
        UIManager.Instance.PlayerWin();

        DataCollector.Instance.CodeAndSendEvent("player_win", null);
    }

    //Called when every half time of a beat has passed
    void OnHalfBeat()
    {
        //counter
        HalfBeatCount = (HalfBeatCount + 1) % 16;

        //Different tasks are executed on different half-beats.
        if (HalfBeatCount == 0)
        {
            UIManager.Instance.ResetBeatIndicator();
        }

        if (HalfBeatCount == 1)
        {
            UIManager.Instance.ClearCharacters();
        }

        if (gameState == GameState.Combat)
        {
            if (HalfBeatCount == 0)
            {
                if (!isTutorial &&
                    EnemyManager.CurrentEnemyControllers.Count + EnemyManager.Instance.enemyControllersCache.Count == 0)
                {

                    NotificationCenter.Instance.PostNotification("AllEnemyKilled");

                    if (isEndlessMode)
                    {
                        EnterGameState(GameState.Move);
                        return;
                    }

                    //Check whether the boss is just beaten
                    if (EnemyManager.Instance.NextEnemyIndex == 0)
                    {
                        if (level == 7)
                        {
                            PlayerWin();
                            return;
                        }

                        EnterGameState(GameState.LevelUp);
                        PlayerStats.Instance.skillPoint += 5;
                        return;
                    }

                    //Make sure that by the end of level 1 the player has at least 3 items. 
                    if (
                        level == 0 &&
                        (5 - EnemyManager.Instance.NextEnemyIndex) == 3 - PlayerStats.Instance.GetNumberOfItems()
                        )
                    {
                        EnterGameState(GameState.ChoosingItem);
                        return;
                    }
                    if (Random.Range(0, 4) != 0)
                    {
                        EnterGameState(GameState.ChoosingItem);
                        return;
                    }

                    EnterGameState(GameState.Move);
                    return;
                }

                if (!isFirstBar)
                {
                    UpdateRhythm();
                    CheckInput();
                    ClearInput();
                    if (lastRhythm != 0 && playerGrade != GradeSetter.Grade.Fail)
                        PlayerController.Instance.BonusAttack();
                }
                ClearInput();
                UIManager.Instance.ShowBeatBar();
            }

            else if (HalfBeatCount == 1)
            {
                if (!AudioManager.Instance.isMusicPlay)
                    EnemyManager.Instance.PlayCurrentMusic();

                if (!isFirstBar)
                {
                    //sometimes in tut we don't want to show grade
                    bool flag = true;
                    if (isTutorial && tutorialPlayerAction.Peek() == PlayerAction.Undefinded) flag = false;
                    if (isTutorial && tutorialPlayerAction.Peek() == PlayerAction.Idle) flag = false;
                    if (flag) UIManager.Instance.ShowGrade(playerGrade);

                    NotificationCenter.Instance.PostNotification("TwoBarPassedInCombat");
                    DeployActions();
                    UpdateComboAndCharge();
                }

                GenerateEnemyAction();
            }
            else if (isFirstBar && HalfBeatCount == 15)
            {
                isFirstBar = false;
            }
        }

        else if (gameState == GameState.Move)
        {
            if (HalfBeatCount == 1 && isFirstBar)
            {
                EnemyManager.Instance.OnEnemyChange();

                StartCoroutine(MoveCoroutine());

                isFirstBar = false;
            }

            if (HalfBeatCount == 0 && readyForCombat)
            {
                ClearActions();
                ClearInput();
                EnterGameState(GameState.Combat);
                UIManager.Instance.ShowBeatBar();
                return;
            }
        }
        else if (gameState == GameState.ChoosingItem)
        {
            if (HalfBeatCount == 1 && isFirstBar)
            {
                for (int i = 0; i < 3; ++i)
                    itemsToChoose[i] = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
                UIManager.Instance.EnableItemChoosingPanel(itemsToChoose);

                isFirstBar = false;
            }

            if (HalfBeatCount == 0 && UIManager.Instance.ChoosingItemPhaseEnded)
            {
                if (UIManager.Instance.ChoiceOfPlayer == -1)
                {
                    EnterGameState(GameState.Move);
                    return;
                }

                var itemChosen = itemsToChoose[UIManager.Instance.ChoiceOfPlayer];
                if (itemChosen.GetComponent<Item>() is SkillItem)
                {
                    if (PlayerStats.Instance.SkillItems[0] == null)
                        PlayerStats.Instance.AddSkillItem(itemChosen, 0);
                    else if (PlayerStats.Instance.SkillItems[1] == null)
                        PlayerStats.Instance.AddSkillItem(itemChosen, 1);
                    else PlayerStats.Instance.AddSkillItem(itemChosen, UIManager.Instance.SlotToReplace);
                }
                else if (itemChosen.GetComponent<Item>() is PropertyAffectingItem)
                {
                    PlayerStats.Instance.AddPropertyAffectingItem(itemChosen);
                }
                else if (itemChosen.GetComponent<Item>() is ActionAffectingItem)
                {
                    PlayerStats.Instance.AddActionAffectingItem(itemChosen);
                }
                EnterGameState(GameState.Move);
                return;
            }
        }
        else if (gameState == GameState.LevelUp)
        {
            if (HalfBeatCount == 1 && isFirstBar)
            {
                UIManager.Instance.EnableLevelUp();

                isFirstBar = false;
            }

            if (HalfBeatCount == 0 && PlayerStats.Instance.skillPoint == 0)
            {
                level++;

                EnterGameState(GameState.Move);

                AudioManager.Instance.MusicFadeOut();
                return;
            }
        }
        else if (gameState == GameState.GameOver)
        {
            if (HalfBeatCount == 15 && isRevived)
            {
                EnterGameState(GameState.Combat);
                ReviveAndContinue();
            }
        }

        //send messang: on half beat
        NotificationCenter.Instance.PostNotification("OnHalfBeat");
    }

    void EnterGameState(GameState newGameState)
    {
        gameState = newGameState;
        isFirstBar = true;

        NotificationCenter.Instance.PostNotification("EnterGameState");

        if (newGameState == GameState.Combat)
        {
            lastRhythm = currentRhythm = 0;
            currentRhythmCnt = 1;
        }
    }

    //Directing the background and animation of player
    IEnumerator MoveCoroutine()
    {
        //The flag indicating whether it is time to start the fight against the new enemy
        readyForCombat = false;

        float translation = backgroundPrefab.transform.position.x - currentBackground.transform.position.x;
        float time = 14 * secondPerHalfBeat;
        float velocity = translation / time;

        var newEnemies = EnemyManager.Instance.SpawnNextEnemy();

        bool canEnemyMove = true;
        foreach (GameObject newEnemy in newEnemies)
            newEnemy.transform.position += new Vector3(translation, 0f, 0f);

        //Only when there is one enemy,
        //could there be a enemy that is fetched from the blockchain. 
        EnemyStats newBlockChainEnemyStats = null;
        if (newEnemies.Count == 1)
        {
            newBlockChainEnemyStats = newEnemies[0].GetComponent<EnemyStats>();
            canEnemyMove = newBlockChainEnemyStats.isPropertyInitialized;
        }

        PlayerController.Instance.PlayMoveAnimation();

        for (; ; )
        {
            GameObject newBackground;
            if (level < 4)
                newBackground = Instantiate(backgroundPrefab);
            else
                newBackground = Instantiate(
                backgroundOfSecondStagePrefabs[Random.Range(0, backgroundOfSecondStagePrefabs.Length)]
                    );

            float deltaTime = 0f;
            while (deltaTime < time)
            {
                deltaTime += Time.fixedDeltaTime;
                var translationInVector = new Vector3(-1f * velocity * Time.fixedDeltaTime, 0f, 0f);
                currentBackground.transform.position += translationInVector;
                newBackground.transform.position += translationInVector;
                if (canEnemyMove)
                    foreach (GameObject newEnemy in newEnemies)
                        newEnemy.transform.position += translationInVector;
                yield return new WaitForFixedUpdate();
            }

            //Adjusting the background's position and the new enemy's position
            if (deltaTime > time)
            {
                var translationInVector = new Vector3(velocity * (deltaTime - time), 0f, 0f);
                currentBackground.transform.position += translationInVector;
                newBackground.transform.position += translationInVector;
                if (canEnemyMove)
                {
                    foreach (GameObject newEnemy in newEnemies)
                        newEnemy.transform.position += translationInVector;

                    PlayerController.Instance.StopMoveAnimation();

                    Destroy(currentBackground);
                    currentBackground = newBackground;
                    break;
                }
                else canEnemyMove = newBlockChainEnemyStats.isPropertyInitialized;
            }

            Destroy(currentBackground);
            currentBackground = newBackground;
        }

        readyForCombat = true;
    }

    //Defining all actions
    //0,1,2,3 -> index of musicKeys
    public readonly Dictionary<KeyCode, PlayerAction> KeyToAction = new Dictionary<KeyCode, PlayerAction>() {
        {InputSettings.MusicKeys[0], PlayerAction.MoveUp},
        {InputSettings.MusicKeys[1], PlayerAction.Charge},
        {InputSettings.MusicKeys[2], PlayerAction.MoveDown},
        {InputSettings.MusicKeys[3], PlayerAction.Attack},
        {InputSettings.MusicKeys[4], PlayerAction.UseItem_0},
        {InputSettings.MusicKeys[5], PlayerAction.UseItem_1}
    };
    public static readonly Rhythm[] Rhythms = new Rhythm[]{
        new Rhythm(new int[]{1,2,2,2}, Color.yellow),
        new Rhythm(new int[]{1,2,1,1,2}, Color.green),
        new Rhythm(new int[]{1,3,1,2}, Color.cyan),
        new Rhythm(new int[]{1,1,2,1,2}, Color.red)
    };

    int lastRhythm = 0;
    int currentRhythm = 0;
    int currentRhythmCnt = 0;

    public int CurrentRhythmIndex { get { return currentRhythm; } }

    void UpdateRhythm()
    {
        lastRhythm = currentRhythm;
        if (currentRhythm != 0) currentRhythm = currentRhythmCnt = 0;
        currentRhythmCnt += 1;
        if (currentRhythmCnt == 3)
        {
            switch (level)
            {
                case 0: break;
                case 1: currentRhythm = 1; break;
                case 2: currentRhythm = 2; break;
                case 3: currentRhythm = 3; break;
                default: currentRhythm = Random.Range(1, 4); break;
            }
            currentRhythmCnt = 0;
        }
    }

    void ClearActions()
    {
        playerAction = PlayerAction.Idle;
        tutorialEnemyAction = EnemyAction.Idle;
    }

    void CheckInput()
    {
        //send event start
        var eventParams = new Dictionary<string, string>();

        var correctTime = "[";
        for (int i = 0; i < Rhythms[lastRhythm].length; ++i)
            correctTime += Rhythms[lastRhythm].accurateTime[i].ToString() + ",";
        if (correctTime.EndsWith(","))
            correctTime = correctTime.Remove(correctTime.Length - 1);
        correctTime += "]";

        var pressTime = "[";
        var buttonPressed = "[";
        for (int i = 0; i < keyPresses.Count; ++i)
        {
            pressTime += keyPresses[i].time.ToString() + ",";
            buttonPressed += '"' + InputSettings.NameOfMusicKeys[keyPresses[i].keyIndex] + '"' + ',';
        }
        if (pressTime.EndsWith(","))
            pressTime = pressTime.Remove(pressTime.Length - 1);
        pressTime += "]";
        if (buttonPressed.EndsWith(","))
            buttonPressed = buttonPressed.Remove(buttonPressed.Length - 1);
        buttonPressed += "]";

        var skillItemsState = "["
            + PlayerStats.Instance.GetStateOfSkillItem(0)
            + ","
            + PlayerStats.Instance.GetStateOfSkillItem(1)
            + "]";

        eventParams["correct_time"] = correctTime;
        eventParams["press_time"] = pressTime;
        eventParams["button_pressed"] = buttonPressed;
        eventParams["skill_items_state"] = skillItemsState;

        //send event end

        playerAction = PlayerAction.Idle;

        if (keyPresses.Count < Rhythms[lastRhythm].length)
        {
            playerGrade = GradeSetter.Grade.Fail;

            eventParams["is_success"] = "false";
            DataCollector.Instance.CodeAndSendEvent("input", eventParams);
            return;
        }
        var error = 0f;

        for (int i = 0; i < Rhythms[lastRhythm].length; ++i)
            error += Mathf.Pow(Rhythms[lastRhythm].accurateTime[i] - keyPresses[i].time, 2);

        //Debug.Log(error);

        if (error <= fineBound)
        {
            playerAction = KeyToAction[keyPresses[0].key];
            for (int i = 1; i < Rhythms[lastRhythm].length; ++i)
                if (keyPresses[0].key != keyPresses[i].key)
                    playerAction = PlayerAction.Idle;

            if (error <= coolBound)
                playerGrade = GradeSetter.Grade.Cool;
            else
                playerGrade = GradeSetter.Grade.Fine;
        }
        else
            playerGrade = GradeSetter.Grade.Fail;

        if (playerAction == PlayerAction.Idle)
            playerGrade = GradeSetter.Grade.Fail;

        if (playerGrade != GradeSetter.Grade.Fail)
        {
            if (AudioManager.Instance.actionDumpIndex.ContainsKey(playerAction))
            {
                int index = AudioManager.Instance.actionDumpIndex[playerAction];
                for (int i = 0; i < Rhythms[lastRhythm].length; ++i)
                    StartCoroutine(PlayDrum(index, Rhythms[lastRhythm].accurateTime[i]));
            }
        }


        eventParams["is_success"] = playerGrade != GradeSetter.Grade.Fail ? "true" : "false";
        DataCollector.Instance.CodeAndSendEvent("input", eventParams);
    }

    IEnumerator PlayDrum(int drumIndex, float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioManager.Instance.PlayDrum(drumIndex, 2f);
    }
    void ClearInput()
    {
        keyPresses.Clear();
        deltaTimeForInput = 0f;
    }

    void GenerateEnemyAction()
    {
        //In the tutorial, the enemy's actions are determined by the progress of tutorial.
        if (isTutorial)
        {
            var enemyController = EnemyManager.CurrentEnemyControllers[0];
            var enemyStats = enemyController.enemyStats;

            //pa: player action
            //ea: enemy action
            //pp: player position stats
            //ep: enemy position stats
            PlayerAction pa = tutorialPlayerAction.Peek();
            EnemyAction ea = EnemyAction.Idle;
            int pp = PlayerStats.Instance.verticalPositionState;
            int ep = enemyStats.verticalPositionState;
            if (pa == PlayerAction.Idle || pa == PlayerAction.Undefinded)
            {
                ea = EnemyAction.Idle;
            }
            else if (pa == PlayerAction.MoveUp)
            {
                if (ep == 1) ea = EnemyAction.MoveDown;
                else if (pp != 1)
                {
                    ea = enemyController.enemyAI.GetTrackPlayerMove();
                    if (ea == EnemyAction.Idle) ea = EnemyAction.Attack;
                }
                else ea = EnemyAction.Attack;
            }
            else if (pa == PlayerAction.MoveDown)
            {
                if (ep == -1) ea = EnemyAction.MoveUp;
                else if (pp != -1)
                {
                    ea = enemyController.enemyAI.GetTrackPlayerMove();
                    if (ea == EnemyAction.Idle) ea = EnemyAction.Attack;
                }
                else ea = EnemyAction.Attack;
            }
            else if (pa == PlayerAction.Charge || pa == PlayerAction.Attack)
            {
                ea = enemyController.enemyAI.GetTrackPlayerMove();
                if (ea == EnemyAction.Idle) ea = EnemyAction.Charge;
            }

            tutorialEnemyAction = ea;
        }
    }

    void DeployActions()
    {
        if (!isTutorial)
        {
            PlayerController.Instance.DeployAction(playerAction);

            if (isFirstBar)
            {
                foreach (EnemyController enemyController in EnemyManager.CurrentEnemyControllers)
                    enemyController.DeployAction(EnemyAction.Idle);
            }
            else
            {
                foreach (EnemyController enemyController in EnemyManager.CurrentEnemyControllers)
                    enemyController.DeployAIAction();
            }

            EnemyManager.Instance.ClearEnemyControllerCache();
            EnemyManager.Instance.RemoveUselessEnemyControllers();
        }

        if (isTutorial)
        {
            EnemyManager.CurrentEnemyControllers[0].DeployAction(tutorialEnemyAction);

            //when player done the right operate in tutorial
            //if current tut action is idle, player can do anything but we will ignore it
            if (playerAction == tutorialPlayerAction.Peek() || tutorialPlayerAction.Peek() == PlayerAction.Idle)
            {
                if (tutorialPlayerAction.Peek() != PlayerAction.Idle)
                    PlayerController.Instance.DeployAction(playerAction);

                bool next = true;
                if (playerAction == PlayerAction.MoveUp && PlayerStats.Instance.verticalPositionState != 1) next = false;
                if (playerAction == PlayerAction.MoveDown && PlayerStats.Instance.verticalPositionState != -1) next = false;
                if (tutorialPlayerAction.Peek() == PlayerAction.Idle) next = true;

                if (next) NextTutorialState();
            }
        }
    }

    public void NextTutorialState()
    {
        tutorialPlayerAction.Dequeue();
        tutorialPanelsId.Dequeue();

        if (tutorialPlayerAction.Count == 0)
        {
            ScenesManager.Instance.LoadLoadingScene("RD_0");
            AudioManager.Instance.StopMusic();

            DataCollector.Instance.CodeAndSendEvent("tutorial_end", null);
        }

        if (tutorialPanelsId.Count > 0)
            UIManager.Instance.ShowTutorialPanel(tutorialPanelsId.Peek());
    }

    void UpdateComboAndCharge()
    {
        if (playerAction != PlayerAction.Idle && playerAction != PlayerAction.Undefinded)
            combo += 1;
        else
        {
            combo = 0;
            PlayerStats.Instance.fever = false;
        }

        if (combo == 5)
        {
            PlayerStats.Instance.fever = true;
        }

        UIManager.Instance.UpdateCombo();
    }
}