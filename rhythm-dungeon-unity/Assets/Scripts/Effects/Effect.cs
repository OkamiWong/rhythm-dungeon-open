using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    static public Dictionary<System.Type, GameObject> Prefabs
        = new Dictionary<System.Type, GameObject>();

    [HideInInspector]
    public int life;
    [HideInInspector]
    public Stats holder;

    protected virtual void Awake()
    {
        if (!Prefabs.ContainsKey(GetType()) || Prefabs[GetType()] == null)
            Prefabs[GetType()] = gameObject;
    }

    public void BaseInit(int _life, Stats _holder)
    {
        life = _life; holder = _holder;
        transform.parent = holder.transform;
        transform.position = holder.transform.position;

        OnTakingEffect();
    }

    public virtual void OnTakingEffect() { }

    //Called before every DeployActions
    public virtual void OnEffecting() { }

    //Called when the state disappears
    public virtual void OnLosingEffect() { }
}