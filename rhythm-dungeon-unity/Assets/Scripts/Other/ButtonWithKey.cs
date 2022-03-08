using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ButtonWithKey : MonoBehaviour
{
    public KeyCode key;
    Button button;

    void Start()
    {
        button = GetComponent<Button>();
    }

    void Update()
    {
        if (Input.GetKeyDown(key))
            button.onClick.Invoke();
    }
}
