using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableHandOnEnable : MonoBehaviour
{
    public GameObject hand, handWhenNotMobile;

    void OnEnable()
    {
        if (JSInterface.Instance.isMobile)
            hand.SetActive(true);
        else handWhenNotMobile.SetActive(true);
    }

    void OnDisable()
    {
        if (JSInterface.Instance.isMobile)
            hand.SetActive(false);
        else handWhenNotMobile.SetActive(true);
    }
}
