using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used in tutorial to implement the feature of press C to continue
public class KeyPressDeactivater : MonoBehaviour
{
    public KeyCode key;
    void FixedUpdate()
    {
        if (Input.GetKeyDown(key))
            TakeEffect();
    }

    public void TakeEffect()
    {
        gameObject.SetActive(false);
        GameManager.Instance.NextTutorialState();
    }
}
