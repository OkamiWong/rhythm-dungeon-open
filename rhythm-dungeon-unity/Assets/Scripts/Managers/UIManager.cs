using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance; //Singleton

    //Ceched components

    [Header("Player Properties")]
    public Text hpText;
    public Text hpTextInLevelUpPanel;
    public Text strengthText;
    public Text luckText;

    [Header("Combo")]
    public GameObject comboNumber;
    public GameObject comboPanel;
    public GameObject feverPanel;

    [Header("Critical Hit")]
    public GameObject playerCritical;
    public GameObject enemyCritical;

    [Header("Screens GameObjects")]
    public GameObject pauseGo;
    public GameObject gameoverGO;
    public GameObject playerWinPanel;

    [Header("About the Character")]
    public GameObject canvas;
    public GameObject character;
    public GameObject arrowUp, arrowDown, arrowLeft, arrowRight;
    public float maxX, maxY;

    [Header("Level Up")]
    public GameObject levelUpPanel;
    public Text remainingSkillPoint;

    [Header("Grade")]
    public GameObject gradeTextHolder;

    [Header("Hiden Objects in Tutorial")]
    public GameObject[] hidenObjectsInTutorial;

    Queue<GameObject> charactersOrArrows;

    //Singleton method
    void SingletonInit()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Awake()
    {
        SingletonInit();
        charactersOrArrows = new Queue<GameObject>();
    }

    //Update ui method
    public void UpdateUI()
    {
        string text = PlayerStats.Instance.CurrentHealth + "/" + PlayerStats.Instance.MaxHealth;
        hpText.text = text;
        if (GameManager.Instance.GState == GameManager.GameState.LevelUp)
        {
            hpTextInLevelUpPanel.text = text;
            strengthText.text = PlayerStats.Instance.rawStrength.ToString();
            luckText.text = PlayerStats.Instance.rawLuck.ToString();
        }
    }

    //Pause method
    public void Pause()
    {
        pauseGo.SetActive(true); //Reverse pause screen active status 
    }

    public void Continue()
    {
        if (SettingsManager.Instance != null)
            SettingsManager.Instance.Back();

        pauseGo.SetActive(false);
    }

    //UI GameOver method
    public void GameOver()
    {
        gameoverGO.SetActive(true); //gameover screen enable

        InitializeRevivalPanel();
    }

    public void PlayerWin()
    {
        playerWinPanel.SetActive(true);
    }

    public void EnterSettings()
    {
        SceneManager.LoadScene("Settings", LoadSceneMode.Additive);
    }

    //Load main menu method
    public void LoadMainMenu()
    {
        AudioManager.Instance.StopMusic();
        ScenesManager.Instance.LoadLoadingScene("MainMenu"); //Load main menu scene
    }

    [Header("Beat Indicator")]
    public PlayableDirector beatIndicatorDirector;
    float realTimeLapse = 0f;
    float timeScaler;
    void Start()
    {
        timeScaler = 4f / (GameManager.Instance.secondPerHalfBeat * 16f);

        if (GameManager.Instance.isTutorial)
        {
            foreach (GameObject gO in hidenObjectsInTutorial)
                gO.SetActive(false);
        }
        else
        {
            foreach (GameObject gO in hidenObjectsInTutorial)
                gO.SetActive(true);
        }

        if (GameManager.Instance.isEndlessMode)
            processBar.SetActive(false);
        else processBar.SetActive(true);

        MobileStart();
    }
    public void ResetBeatIndicator()
    {
        realTimeLapse = 0f;
    }
    void FixedUpdate()
    {
        realTimeLapse += Time.deltaTime;
        beatIndicatorDirector.time = realTimeLapse * timeScaler;
        beatIndicatorDirector.Evaluate();
    }

    [Header("Bars in Beat Indicator")]
    public GameObject beatBarPrefab;
    public GameObject beatBarParent;

    public int gapXPerHalfBeat = 14;
    public void ShowBeatBar()
    {
        var rhythm = GameManager.Rhythms[GameManager.Instance.CurrentRhythmIndex];
        for (int i = 0; i < rhythm.length; ++i)
            StartCoroutine(
                ShowOneBeatBarCoroutine(
                    rhythm.accurateTime[i],
                    gapXPerHalfBeat * rhythm.scaledTime[i] + 3,
                    rhythm.color
                    )
            );
    }

    IEnumerator ShowOneBeatBarCoroutine(float timeAfterInputStarted, int posX, Color color)
    {
        var beatBar = Instantiate(beatBarPrefab);
        beatBar.GetComponent<Image>().color = color;
        beatBar.transform.SetParent(beatBarParent.transform);
        beatBar.transform.localScale = Vector3.one;
        beatBar.transform.localPosition = new Vector3(posX, 0f, 0f);
        yield return new WaitForSeconds(timeAfterInputStarted + GameManager.Instance.secondPerHalfBeat * 7f);
        var deltaTime = 0f;
        var scalingSpeed = 1f / (GameManager.Instance.secondPerHalfBeat / 2f);
        while (deltaTime < GameManager.Instance.secondPerHalfBeat * 2f)
        {
            if (GameManager.Instance.GState != GameManager.GameState.Pause)
            {
                if (deltaTime > GameManager.Instance.secondPerHalfBeat / 2f)
                {
                    if (deltaTime < GameManager.Instance.secondPerHalfBeat)
                        beatBar.transform.localScale += Vector3.one * Time.fixedDeltaTime * scalingSpeed;
                    else
                        beatBar.transform.localScale -= Vector3.one * Time.fixedDeltaTime * (scalingSpeed / 2f);
                }
                deltaTime += Time.deltaTime;
            }
            yield return null;
        }
        Destroy(beatBar);
    }

    [Header("Fever")]
    public GameObject redFrame;
    public void StartFever()
    {
        redFrame.SetActive(true);
    }

    public void StopFever()
    {
        redFrame.SetActive(false);
    }

    //After removing forecast, this dict became useless.
    //Keep it in case we use it in the future. 
    //private static readonly Dictionary<EnemyAction, string> actionToStr = new Dictionary<EnemyAction, string>() {
    //	{ EnemyAction.MoveUp, "MOVE UP" },
    //	{ EnemyAction.MoveDown, "MOVE DOWN" },
    //	{ EnemyAction.Defend, "DEFEND" },
    //	{ EnemyAction.Charge, "CHARGE" },
    //	{ EnemyAction.Attack, "ATTACK" },
    //	{ EnemyAction.Idle, "IDLE" },
    //};

    public void ShowGrade(GradeSetter.Grade grade)
    {
        gradeTextHolder.GetComponent<GradeSetter>().SetGradeText(grade);
        StartCoroutine(ShowObject(gradeTextHolder, GameManager.Instance.secondPerHalfBeat * 8));
        StartCoroutine(CharacterSpawn(gradeTextHolder));
    }

    //Spawn a character at a random position, orienting randomly,
    //to indicate which key just was pressed.
    public void ShowCharacterOrArrow(int buttonIndex)
    {
        GameObject characterOrArrow = null;
        if (buttonIndex < 4)
        {
            switch (buttonIndex)
            {
                case 0: characterOrArrow = Instantiate(arrowUp); break;
                case 1: characterOrArrow = Instantiate(arrowLeft); break;
                case 2: characterOrArrow = Instantiate(arrowDown); break;
                case 3: characterOrArrow = Instantiate(arrowRight); break;
            }
        }
        else
        {
            characterOrArrow = Instantiate(character);
            characterOrArrow.GetComponent<Text>().text = InputSettings.NameOfMusicKeys[buttonIndex];
        }
        characterOrArrow.transform.position = canvas.transform.position + new Vector3(UnityEngine.Random.Range(-maxX, maxX), UnityEngine.Random.Range(-maxY, maxY), 0f);
        characterOrArrow.transform.SetParent(canvas.transform);
        characterOrArrow.transform.localScale = Vector3.one * 1.5f;
        StartCoroutine(CharacterSpawn(characterOrArrow));

        charactersOrArrows.Enqueue(characterOrArrow);
    }

    public IEnumerator CharacterSpawn(GameObject newCh)
    {
        var time = GameManager.Instance.secondPerHalfBeat;
        var velocity = 0.4f / time;
        var deltaTime = 0f;
        var initialScale = newCh.transform.localScale;

        //Make the character big
        while (deltaTime < time)
        {
            if (newCh == null) yield break;
            deltaTime += Time.fixedDeltaTime;
            var delta = velocity * Time.fixedDeltaTime;
            newCh.transform.localScale += new Vector3(delta, delta, 0f);
            yield return new WaitForFixedUpdate();
        }

        //Make the character back to normal size
        deltaTime = 0;
        while (deltaTime < time)
        {
            if (newCh == null) yield break;
            deltaTime += Time.fixedDeltaTime;
            var delta = velocity * Time.fixedDeltaTime;
            newCh.transform.localScale -= new Vector3(delta, delta, 0f);
            yield return new WaitForFixedUpdate();
        }

        if (newCh == null) yield break;
        newCh.transform.localScale = initialScale;
    }

    public void ClearCharacters()
    {
        while (charactersOrArrows.Count > 0)
        {
            Destroy(charactersOrArrows.Dequeue());
        }
    }

    public void UpdateCombo()
    {
        if (GameManager.Instance.combo == 0)
        {
            comboPanel.SetActive(false);
            feverPanel.SetActive(false);
            redFrame.SetActive(false);
        }
        else if (GameManager.Instance.combo >= 5)
        {
            comboPanel.SetActive(false);
            feverPanel.SetActive(true);
            redFrame.SetActive(true);
        }
        else
        {
            comboPanel.SetActive(true);
            feverPanel.SetActive(false);
            redFrame.SetActive(false);
            comboNumber.GetComponent<Text>().text = GameManager.Instance.combo.ToString();
            StartCoroutine(CharacterSpawn(comboNumber));
        }
    }

    public void ShowCritical(bool isPlayer)
    {
        if (isPlayer)
        {
            StartCoroutine(ShowObject(playerCritical, GameManager.Instance.secondPerHalfBeat * 8));
            StartCoroutine(CharacterSpawn(playerCritical));
        }

        else
        {
            //StartCoroutine(ShowObject(enemyCritical, GameManager.Instance.secondPerHalfBeat * 8));
            //StartCoroutine(CharacterSpawn(enemyCritical));
        }
    }

    IEnumerator ShowObject(GameObject gO, float time)
    {
        gO.SetActive(true);
        yield return new WaitForSeconds(time);
        gO.SetActive(false);
    }

    int lastRawMaxHealth, lastRawStrength, lastRawLuck;

    public void EnableLevelUp()
    {
        hpText.transform.parent.gameObject.SetActive(false);
        levelUpPanel.SetActive(true);
        remainingSkillPoint.text = PlayerStats.Instance.skillPoint.ToString();

        PlayerStats.Instance.Recover();

        lastRawMaxHealth = PlayerStats.Instance.rawMaxHealth;
        lastRawStrength = PlayerStats.Instance.rawStrength;
        lastRawLuck = PlayerStats.Instance.rawLuck;
    }

    public void IncreaseHealth()
    {
        PlayerStats.Instance.rawMaxHealth += 20;
        PlayerStats.Instance.Recover();
        AfterIncreasing();
    }
    public void IncreaseStrength()
    {
        PlayerStats.Instance.rawStrength += 5;
        AfterIncreasing();
    }

    public void IncreaseLuck()
    {
        PlayerStats.Instance.rawLuck += 10;
        AfterIncreasing();
    }

    void AfterIncreasing()
    {
        PlayerStats.Instance.skillPoint -= 1;
        remainingSkillPoint.text = PlayerStats.Instance.skillPoint.ToString();
        if (PlayerStats.Instance.skillPoint == 0)
        {
            hpText.transform.parent.gameObject.SetActive(true);
            levelUpPanel.SetActive(false);
            UpdateProcessBar();
            SendLevelupEvent();
        }
    }

    void SendLevelupEvent()
    {
        var eventParams = new Dictionary<string, string>();
        eventParams["delta_max_hp"] = (PlayerStats.Instance.rawMaxHealth - lastRawMaxHealth).ToString();
        eventParams["delta_strength"] = (PlayerStats.Instance.rawStrength - lastRawStrength).ToString();
        eventParams["delta_luck"] = (PlayerStats.Instance.rawLuck - lastRawLuck).ToString();
        DataCollector.Instance.CodeAndSendEvent("levelup", eventParams);
    }

    [Header("Process Bar")]
    public GameObject processBar;
    public GameObject selector;
    public Image[] levelLogos;
    void UpdateProcessBar()
    {
        StartCoroutine(MoveSelectorCoroutine());
        levelLogos[GameManager.Instance.level].color = Color.black;
    }

    IEnumerator MoveSelectorCoroutine()
    {
        var time = GameManager.Instance.secondPerHalfBeat * 16;
        var translation = 40f;
        var speed = translation / time;
        var deltaTime = 0f;
        var destinatedX = selector.transform.localPosition.x + 40f;
        while (deltaTime < time)
        {
            selector.transform.localPosition += Vector3.right * speed * Time.fixedDeltaTime;
            deltaTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        selector.transform.localPosition = Vector3.right * destinatedX;
    }


    [Header("Choosing Item")]
    public GameObject itemChoosingPanel;
    public GameObject skillItemReplacingPanel;
    public IconDisplayer qItemToReplace, eItemToReplace;
    public IconDisplayer[] choicesOfItems;
    GameObject[] _itemPrefabs;

    public int ChoiceOfPlayer { get; private set; }
    public int SlotToReplace { get; private set; }
    public bool ChoosingItemPhaseEnded { get; private set; }

    float startTimeOfChoosingItemPhase;

    public void EnableItemChoosingPanel(GameObject[] itemPrefabs)
    {
        startTimeOfChoosingItemPhase = Time.time;

        _itemPrefabs = itemPrefabs;

        ChoiceOfPlayer = -1;
        SlotToReplace = -1;
        ChoosingItemPhaseEnded = false;

        itemChoosingPanel.SetActive(true);
        for (int i = 0; i < 3; ++i)
            choicesOfItems[i].Initialize(itemPrefabs[i].GetComponent<Item>());
    }

    public void ChooseItem0() { ChoiceOfPlayer = 0; }

    public void ChooseItem1() { ChoiceOfPlayer = 1; }

    public void ChooseItem2() { ChoiceOfPlayer = 2; }

    public void DisableItemChoosingPanel()
    {
        if (ChoiceOfPlayer != -1 && _itemPrefabs[ChoiceOfPlayer].GetComponent<Item>() is SkillItem)
        {
            if (PlayerStats.Instance.SkillItems[0] != null
                && PlayerStats.Instance.SkillItems[1] != null)
                EnableSkillItemReplacingPanel();
            else
            {
                itemChoosingPanel.SetActive(false);
                ChoosingItemPhaseEnded = true;
                if (PlayerStats.Instance.SkillItems[0] == null)
                    SendGetSkillItemEvent(0);
                else SendGetSkillItemEvent(1);
            }
        }
        else
        {
            itemChoosingPanel.SetActive(false);
            ChoosingItemPhaseEnded = true;
            SendGetNonSkillItemEvent();
        }
    }

    public void CancelReplacement()
    {
        skillItemReplacingPanel.SetActive(false);
        ChoiceOfPlayer = -1;
    }

    void EnableSkillItemReplacingPanel()
    {
        skillItemReplacingPanel.SetActive(true);
        qItemToReplace.Initialize(PlayerStats.Instance.SkillItems[0]);
        eItemToReplace.Initialize(PlayerStats.Instance.SkillItems[1]);
    }

    public void ChooseQItem()
    {
        SlotToReplace = 0;
    }
    public void ChooseEItem()
    {
        SlotToReplace = 1;
    }

    public void DisableSkillItemReplacingPanel()
    {
        itemChoosingPanel.SetActive(false);
        skillItemReplacingPanel.SetActive(false);
        ChoosingItemPhaseEnded = true;
        SendGetSkillItemEvent(SlotToReplace);
    }

    void SendGetSkillItemEvent(int slot)
    {
        var eventParams = new Dictionary<string, string>();
        eventParams["item_name"] = '"' + _itemPrefabs[ChoiceOfPlayer].GetComponent<Item>().itemName + '"';
        eventParams["chosing_time"] = (Time.time - startTimeOfChoosingItemPhase).ToString();
        eventParams["slot"] = slot.ToString();
        DataCollector.Instance.CodeAndSendEvent("get_skill_item", eventParams);
    }

    void SendGetNonSkillItemEvent()
    {
        var eventParams = new Dictionary<string, string>();
        eventParams["item_name"] = '"' + _itemPrefabs[ChoiceOfPlayer].GetComponent<Item>().itemName + '"';
        eventParams["chosing_time"] = (Time.time - startTimeOfChoosingItemPhase).ToString();
        DataCollector.Instance.CodeAndSendEvent("get_non_skill_item", eventParams);
    }

    [Header("Skill Item Holders")]
    public GameObject qItem;
    public GameObject eItem;
    public GameObject itemPrefab;
    public GameObject itemPanel;
    Dictionary<String, IconDisplayer> itemToGameObject = new Dictionary<String, IconDisplayer>();

    public void UpdateSkillItem(SkillItem item, string key)
    {
        var itemHolder = key == "Q" ? qItem : eItem;
        if (itemHolder.activeSelf == false)
            itemHolder.SetActive(true);
        var iconDisplayer = itemHolder.GetComponent<IconDisplayer>();
        iconDisplayer.Initialize(item);
        item.iconImage = iconDisplayer.icon;
        item.cdText = iconDisplayer.currentCDText;
    }

    public void AddItem(Item item)
    {
        GameObject itemHolder;
        IconDisplayer iconDisplayer;
        if (itemToGameObject.TryGetValue(item.name, out iconDisplayer))
        {
            iconDisplayer.IncreaseAmount();
        }
        else
        {
            itemHolder = Instantiate(itemPrefab);
            itemHolder.transform.SetParent(itemPanel.transform);
            itemHolder.transform.localScale = Vector3.one;
            iconDisplayer = itemHolder.GetComponent<IconDisplayer>();
            iconDisplayer.Initialize(item);
            itemToGameObject[item.name] = iconDisplayer;
        }
    }


    [Header("Tutorial")]
    public GameObject[] tutorialPanels;
    public GameObject tutorialMainPanel;
    private GameObject showingTutoralPanel = null;

    public void EnableTutorial()
    {
        tutorialMainPanel.SetActive(true);
        tutorialPanels[0].SetActive(true);
    }

    public void DisableTutorial()
    {
        tutorialMainPanel.SetActive(false);
    }

    public void ShowTutorialPanel(int panelId)
    {
        if (showingTutoralPanel != null) showingTutoralPanel.SetActive(false);
        if (panelId >= 0 && panelId < tutorialPanels.Length)
        {
            showingTutoralPanel = tutorialPanels[panelId];
            showingTutoralPanel.SetActive(true);
        }
    }

    [Header("Upload Character")]
    public Text nameInputField;
    public Text uploadedText;
    public GameObject uploadButton, exitButton;
    public void InsertCharacter()
    {
        uploadButton.SetActive(false);
        uploadedText.gameObject.SetActive(true);
#if !UNITY_EDITOR
			uploadedText.text = "Upload request sent to the browser, you can exit.";
#else
        exitButton.SetActive(false);
        uploadedText.text = "Uploading...";
#endif
        GenesisContractService.Instance.InsertCharacter(PlayerStats.Instance, nameInputField.text, GameManager.Instance.level);
    }

    [Header("Upload Character When Player Wins")]
    public Text nameInputFieldWPW;
    public Text uploadedTextWPW;
    public GameObject uploadButtonWPW, exitButtonWPW;

    public void InsertCharacterWhenPlayerWins()
    {
        uploadButtonWPW.SetActive(false);
        uploadedTextWPW.gameObject.SetActive(true);
#if !UNITY_EDITOR
			uploadedTextWPW.text = "Upload request sent to the browser, you can exit.";
#else
        exitButtonWPW.SetActive(false);
        uploadedTextWPW.text = "Uploading...";
#endif
        GenesisContractService.Instance.InsertCharacter(PlayerStats.Instance, nameInputFieldWPW.text, GameManager.Instance.level);
    }

    public void AfterInserting(string text)
    {
        uploadedText.text = text;
        exitButton.SetActive(true);

        uploadedTextWPW.text = text;
        exitButtonWPW.SetActive(true);
    }

    [Header("Revival")]
    public GameObject reviveButton;
    public GameObject revivalError, revivialCommunicating, continueButton, noButton, revivalPanel, uploadPanel;
    public Text amountOfCoins;
    void InitializeRevivalPanel()
    {
        reviveButton.SetActive(true);
        revivalError.SetActive(false);
        revivialCommunicating.SetActive(false);
        continueButton.SetActive(false);
        noButton.SetActive(true);
        uploadPanel.SetActive(false);
        revivalPanel.SetActive(true);
        amountOfCoins.text = "Revival needs " + GameManager.Instance.NumberOfCoins + " coin(s).";
    }
    public void TryReviving()
    {
        reviveButton.SetActive(false);
        revivialCommunicating.SetActive(true);
        GenesisContractService.Instance.UseRevivalCoins(GameManager.Instance.NumberOfCoins, 0);
    }
    public void ReviveSuccessfully()
    {
        revivialCommunicating.SetActive(false);
        continueButton.SetActive(true);
        noButton.SetActive(false);
    }

    public void ReviveFailed(string error)
    {
        revivialCommunicating.SetActive(false);
        revivalError.SetActive(true);
        revivalError.GetComponent<Text>().text = error;
    }

    public void NoClicked()
    {
        uploadPanel.SetActive(true);
        revivalPanel.SetActive(false);
    }

    public void ContinueClicked()
    {
        gameoverGO.SetActive(false);
        GameManager.Instance.isRevived = true;
    }

    [Header("Endless Mode Statistics")]
    public GameObject endlessModeStatistics;
    public GameObject beatenNamePrefab;
    public GameObject beatenNameParent;
    public Text killerName;

    public void ShowEndlessModeStatistics()
    {
        endlessModeStatistics.SetActive(true);
        foreach (string name in GameResult.Instance.beatenPlayers)
        {
            var nameHolder = Instantiate(beatenNamePrefab);
            nameHolder.GetComponent<Text>().text = name;
            nameHolder.transform.SetParent(beatenNameParent.transform);
        }
        var enemyStats = (BlockchainEnemyStats)EnemyManager.CurrentEnemyControllers[0].enemyStats;
        if (enemyStats.nameHolder.gameObject.activeSelf)
            killerName.text = enemyStats.nameHolder.text;
        else killerName.text = string.Empty;
    }

    public void DisableEndlessModeStatistics()
    {
        endlessModeStatistics.SetActive(false);
    }

    [Header("Mobile")]
    public GameObject[] gameObjectsToActivateInMobile;
    public GameObject[] gameObjectsToDeactivateInMobile;
    void MobileStart()
    {
        foreach (GameObject gO in gameObjectsToActivateInMobile)
        {
            gO.SetActive(JSInterface.Instance.isMobile);
        }

        foreach (GameObject gO in gameObjectsToDeactivateInMobile)
        {
            gO.SetActive(!JSInterface.Instance.isMobile);
        }
    }
}