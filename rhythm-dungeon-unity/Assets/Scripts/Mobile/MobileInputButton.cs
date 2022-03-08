using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MobileInputButton : MonoBehaviour
{
    public int keyIndex;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnTouch);
    }

    void OnTouch()
    {
        GameManager.Instance.Pressed(keyIndex);
    }
}
