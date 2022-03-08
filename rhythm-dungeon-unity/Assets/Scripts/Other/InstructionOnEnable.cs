using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionOnEnable : MonoBehaviour
{
    void OnEnable()
    {
        StartCoroutine(UIManager.Instance.CharacterSpawn(gameObject));
    }
}
