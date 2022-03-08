using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    [Header("Prefabs")]
    public GameObject[] enemyPrefabs;
    public GameObject[] bossPrefabs;
    public GameObject[] fetchedEnemyPrefabs;

    public GameObject finalBossPrefab;

    private int[] musicSequence = {
        1, 1, 1, 2, 3
    };

    private int[] musicSequenceInEndlessMode = { 1, 2, 2 };

    private int[] enemySequence = {
        0, 0, 0, 1, 2
    };//0: random combimation, 1:fetched from blockchain, 2:boss

    private int[] enemySequenceInEndlessMode = { 0, 1, 1 };

    private int[,] enemyCombinations = {
        {0,0,-1,0,0,0},//0
        {0,0,0,-1,0,0},//1
        {1,-1,0,0,0,0},//2
        {1,1,-1,0,0,0},//3
        {0,1,-1,0,0,0},//4
        {0,0,1,-1,0,0},//5
        {2,-1,0,0,0,0},//6
        {2,2,-1,0,0,0},//7
        {0,2,-1,0,0,0},//8
        {1,2,-1,0,0,0},//9
        {3,-1,0,0,0,0},//10
        {3,3,-1,0,0,0},//11
        {3,2,-1,0,0,0},//12
        {3,0,-1,0,0,0},//13
        {3,2,-1,0,0,0},//14
        {0,1,2,3,-1,0},//15
        {0,0,4,-1,0,0},//16
        {0,0,5,-1,0,0},//17
        {0,1,2,4,-1,0},//18
        {2,2,1,5,-1,0},//19
        {1,1,1,4,-1,0},//20
        {3,4,1,-1,0,0},//21
        {0,0,0,4,5,-1}//22
    };

    private int[] combimationIndexSortedByDifficulty = {
        0, 2, 6, 10, 1, 3, 5, 7, 4, 8, 9, 11, 12, 13, 14, 15 ,16,17,18,19,20,21,22
    };

    private int[] levelBound = {
        4, 12, 16
    };

    public static List<EnemyController> CurrentEnemyControllers { get; private set; }

    public int NextEnemyIndex { get; private set; }
    public int CurrentEnemyIndex
    {
        get
        {
            return (NextEnemyIndex - 1 + enemySequence.GetLength(0)) % enemySequence.GetLength(0);
        }
    }

    //In the future we can change the get method of this property
    //to determine the bgm when there is multiple enemies.
    public int CurrentMusicIndex
    {
        get
        {
            return musicSequence[CurrentEnemyIndex];
        }
    }

    int indexCount; // for tagging events

    void Awake()
    {
        Instance = this;
        NextEnemyIndex = 0;
        CurrentEnemyControllers = new List<EnemyController>();

        indexCount = 0;
    }

    void Start()
    {
        if (GameManager.Instance.isEndlessMode)
        {
            enemySequence = enemySequenceInEndlessMode;
            musicSequence = musicSequenceInEndlessMode;
        }
    }

    public void OnEnemyChange()
    {
        if (musicSequence[CurrentEnemyIndex] != musicSequence[NextEnemyIndex])
            AudioManager.Instance.MusicFadeOut();
    }

    public List<GameObject> SpawnNextEnemy()
    {
        var newEnemies = new List<GameObject>();

        List<GameObject> enemiesToSpawn = new List<GameObject>();
        if (GameManager.Instance.isTutorial)
        {
            enemiesToSpawn.Add(enemyPrefabs[0]);
        }
        else if (enemySequence[NextEnemyIndex] == 2)
        {
            if (GameManager.Instance.level == 0)
                enemiesToSpawn.Add(bossPrefabs[0]);
            else if (GameManager.Instance.level != 7)
                enemiesToSpawn.Add(bossPrefabs[Random.Range(0, bossPrefabs.Length)]);
            else enemiesToSpawn.Add(finalBossPrefab);
        }
        else if (enemySequence[NextEnemyIndex] == 1)
        {
            enemiesToSpawn.Add(fetchedEnemyPrefabs[Random.Range(0, fetchedEnemyPrefabs.Length)]);
        }
        else
        {
            int randomCombinationIndex;

            if (GameManager.Instance.level < 3)
            {
                randomCombinationIndex =
                    combimationIndexSortedByDifficulty[Random.Range(0, levelBound[GameManager.Instance.level])];
            }
            else if (GameManager.Instance.level < 6)
            {
                randomCombinationIndex = Random.Range(levelBound[1], enemyCombinations.GetLength(0));
            }
            else
            {
                randomCombinationIndex = Random.Range(levelBound[2], enemyCombinations.GetLength(0));
            }

            //randomCombinationIndex = 16; //for testing

            for (int i = 0; ; ++i)
            {
                if (enemyCombinations[randomCombinationIndex, i] == -1) break;
                enemiesToSpawn.Add(enemyPrefabs[enemyCombinations[randomCombinationIndex, i]]);
            }
        }

        foreach (GameObject enemyToSpawn in enemiesToSpawn)
        {

            var newEnemy = Instantiate(enemyToSpawn);
            var newEnemyStats = newEnemy.GetComponent<EnemyStats>();
            var positionState = newEnemyStats.GetInitialPositionState();

            if (positionState.Key == 233)//when there is no possible position
            {
                Destroy(newEnemy);
                continue;
            }

            newEnemy.transform.position = Vector3.up * positionState.Value * Stats.verticalTranslation
                                        + Vector3.right * Stats.enemyX[positionState.Key];
            newEnemyStats.verticalPositionState = positionState.Value;
            newEnemyStats.enemyHorizontalPositionState = positionState.Key;
            newEnemies.Add(newEnemy);

            CurrentEnemyControllers.Add(newEnemy.GetComponent<EnemyController>());

            newEnemyStats.indexInGame = indexCount++;
            var eventParams = new Dictionary<string, string>();
            eventParams["level"] = GameManager.Instance.level.ToString();
            eventParams["type"] = DataCollector.Instance.enemyPrefabToIndex[enemyToSpawn].ToString();
            eventParams["position"] = '[' + newEnemyStats.enemyHorizontalPositionState.ToString() + ',' + newEnemyStats.verticalPositionState.ToString() + ']';
            eventParams["index"] = newEnemyStats.indexInGame.ToString();
            DataCollector.Instance.CodeAndSendEvent("enemy_spawned", eventParams);
        }

        if (!GameManager.Instance.isEndlessMode)
            NextEnemyIndex = (NextEnemyIndex + 1) % enemySequence.GetLength(0);
        else NextEnemyIndex = Mathf.Min(NextEnemyIndex + 1, 2);
        return newEnemies;
    }

    public void SummonEnemy(GameObject enemyPrefab, int verticalPositionState, int horizontalPositionState)
    {
        if (EnemyManager.CurrentEnemyControllers.Count == 0
            || GameManager.Instance.GState != GameManager.GameState.Combat) return;

        var newEnemy = Instantiate(enemyPrefab);
        newEnemy.transform.position = Vector3.up * verticalPositionState * Stats.verticalTranslation
                                    + Vector3.right * Stats.enemyX[horizontalPositionState];
        var newEnemyStats = newEnemy.GetComponent<EnemyStats>();
        newEnemyStats.isSummoned = true;
        newEnemyStats.verticalPositionState = verticalPositionState;
        newEnemyStats.enemyHorizontalPositionState = horizontalPositionState;

        enemyControllersCache.Add(newEnemy.GetComponent<EnemyController>());

        newEnemyStats.indexInGame = indexCount++;
        var eventParams = new Dictionary<string, string>();
        eventParams["level"] = GameManager.Instance.level.ToString();
        eventParams["type"] = DataCollector.Instance.enemyPrefabToIndex[enemyPrefab].ToString();
        eventParams["position"] = '[' + newEnemyStats.enemyHorizontalPositionState.ToString() + ',' + newEnemyStats.verticalPositionState.ToString() + ']';
        eventParams["index"] = newEnemyStats.indexInGame.ToString();
        DataCollector.Instance.CodeAndSendEvent("enemy_spawned", eventParams);
    }

    //The cache is for avoiding modifying CurrentEnemyControllers
    //while enumerating it during DeployActions in GameManager.
    [HideInInspector]
    public List<EnemyController> enemyControllersCache = new List<EnemyController>();
    public void ClearEnemyControllerCache()
    {
        foreach (EnemyController enemyController in enemyControllersCache)
            CurrentEnemyControllers.Add(enemyController);
        enemyControllersCache.Clear();
    }

    public void PlayCurrentMusic()
    {
        AudioManager.Instance.PlayMusic(CurrentMusicIndex);
    }

    public static List<EnemyController> EnemyControllersToRemove = new List<EnemyController>();

    public void RemoveUselessEnemyControllers()
    {
        foreach (EnemyController enemyController in EnemyControllersToRemove)
            CurrentEnemyControllers.Remove(enemyController);
        EnemyControllersToRemove.Clear();
    }
}
