using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class DataCollector : MonoBehaviour
{
#if !UNITY_EDITOR
    [DllImport ("__Internal")]
    private static extern void SendMessageViaWS (string message);
#endif

    public GameObject[] enemyPrefabs;
    public Dictionary<GameObject, int> enemyPrefabToIndex;
    public static DataCollector Instance;

    void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else Instance = this;
    }

    void Start()
    {
        enemyPrefabToIndex = new Dictionary<GameObject, int>();
        for (int i = 0; i < enemyPrefabs.Length; ++i)
            enemyPrefabToIndex[enemyPrefabs[i]] = i;
    }

    public void CodeAndSendEvent(string name, Dictionary<string, string> parameters)
    {
        var message = "{" + "\"event_name\":" + '"' + name + '"' + ",\"time\":" + Time.time + ",\"parameters\":{";
        var serialized_parameters = string.Empty;
        if (parameters != null)
        {
            foreach (KeyValuePair<string, string> parameter in parameters)
            {
                serialized_parameters += '"' + parameter.Key + '"' + ':' + parameter.Value + ',';
            }
            serialized_parameters = serialized_parameters.Remove(serialized_parameters.Length - 1);
        }

#if !UNITY_EDITOR
        SendMessageViaWS(message + serialized_parameters + "}}");
#endif

        Debug.Log("Send message: " + message + serialized_parameters + "}}");
    }
}
