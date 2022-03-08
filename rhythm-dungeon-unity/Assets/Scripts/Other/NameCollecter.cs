using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameCollecter : MonoBehaviour
{
    public Text nameHolder;

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Submit()
    {
        var eventParams = new Dictionary<string,string>();
        eventParams["name"] = '"' + nameHolder.text + '"';
        DataCollector.Instance.CodeAndSendEvent("record name", eventParams);
        gameObject.SetActive(false);
    }
}
