using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterPlaying : MonoBehaviour
{
    public Animator animator;
    bool triggered = false;

    void Update()
    {
        if(!triggered && animator.isActiveAndEnabled){
            triggered = true;
            Destroy(gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
        }
    }
}
