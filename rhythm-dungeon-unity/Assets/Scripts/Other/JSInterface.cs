using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSInterface : MonoBehaviour
{
    public static JSInterface Instance = null;
    public bool injectedWeb3Exists = false;
    void Awake()
    {
        if (JSInterface.Instance == null)
            JSInterface.Instance = this;
        else Destroy(gameObject);
    }

    public void ThereIsInjectedWeb3()
    {
        injectedWeb3Exists = true;
    }

    public void SuccessfullyRevival(string amount)
    {
        var intAmount = int.Parse(amount);
        if (intAmount < GameManager.Instance.NumberOfCoins)
        {
            UIManager.Instance.ReviveFailed("Revival Coin Insufficient");
            return;
        }
        GameManager.Instance.isRevived = true;
        UIManager.Instance.ReviveSuccessfully();
    }

    public bool isMobile;

    public void IsMobile()
    {
        isMobile = true;
    }

    public bool isIOS;

    public void IsIOS()
    {
        isIOS = true;
    }
}
