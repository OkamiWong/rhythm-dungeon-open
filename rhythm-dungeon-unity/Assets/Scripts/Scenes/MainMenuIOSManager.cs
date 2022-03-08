using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuIOSManager : MonoBehaviour
{
    public void LoadNewGame()
    {
        ScenesManager.Instance.LoadLoadingScene("Tutorial");
    }
}
