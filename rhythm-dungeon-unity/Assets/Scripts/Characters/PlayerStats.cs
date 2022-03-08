using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The component to handle player's stats
public class PlayerStats : Stats
{
    public static PlayerStats Instance; //Singleton

    [HideInInspector]
    public int skillPoint = 0;

    protected override void Awake()
    {
        base.Awake();

        Instance = this;
    }

    protected override void Start()
    {
        base.Start();
        //fill hp at start
        CurrentHealth = MaxHealth;
        UIManager.Instance.UpdateUI();

        var weapon = mainBullet.GetComponent<RangeWeapon>();
        weapon.canPenetrateEnemy = weapon.canPenetrateProtector = false;
        weapon.penetratDamageScaler = weapon.penetratProtectorDamageScaler = 0;

        weapon = PlayerController.Instance.bonusAttackBulletPrefab.GetComponent<RangeWeapon>();
        weapon.canPenetrateEnemy = weapon.canPenetrateProtector = false;
        weapon.penetratDamageScaler = weapon.penetratProtectorDamageScaler = 0;

        lastHP = CurrentHealth;
        lastMaxHP = MaxHealth;
    }

    int lastHP, lastMaxHP;
    //for detecting hp change event
    void Update()
    {
        if (CurrentHealth != lastHP || MaxHealth != lastMaxHP)
            SendHPChangeEvent();
        lastHP = CurrentHealth;
        lastMaxHP = MaxHealth;
    }

    void SendHPChangeEvent()
    {
        var eventParams = new Dictionary<string, string>();
        eventParams["current_hp"] = CurrentHealth.ToString();
        eventParams["current_max_hp"] = MaxHealth.ToString();
        DataCollector.Instance.CodeAndSendEvent("hp_change", eventParams);
    }

    public override PropertyAffectingItem AddPropertyAffectingItem(GameObject itemPrefab)
    {
        PropertyAffectingItem item = base.AddPropertyAffectingItem(itemPrefab);
        UIManager.Instance.AddItem(item);
        return item;
    }

    public override ActionAffectingItem AddActionAffectingItem(GameObject itemPrefab)
    {
        ActionAffectingItem item = base.AddActionAffectingItem(itemPrefab);
        UIManager.Instance.AddItem(item);
        return item;
    }

    public override SkillItem AddSkillItem(GameObject itemPrefab, int which)
    {
        SkillItem item = base.AddSkillItem(itemPrefab, which);
        UIManager.Instance.UpdateSkillItem(item, which == 0 ? "Q" : "E");
        return item;
    }

    //Taking damage method
    public override void TakingDamage(float damage, bool clearChargeFlag = true)
    {
        if (damage < Mathf.Epsilon) return;
        base.TakingDamage(damage, clearChargeFlag);

        UIManager.Instance.UpdateUI(); //Update UI
        AudioManager.Instance.Play(AudioManager.Instance.soundEffect, AudioManager.Instance.playerDamage, false); //play damage sound

        if (CurrentHealth <= 0
            && !GameManager.Instance.isTutorial
            && GameManager.Instance.GState != GameManager.GameState.GameOver)
        {
            Death(); //Lose 
        }
    }

    public override void AddingHealth(float recoverAmount)
    {
        base.AddingHealth(recoverAmount);

        UIManager.Instance.UpdateUI(); //Update UI
        //play recover sound
        //to-do
    }

    public override void Death()
    {
        base.Death();
        GameManager.Instance.GameOver(); //Game over in gamemanager

        var eventParams = new Dictionary<string, string>();
        eventParams["level"] = GameManager.Instance.level.ToString();
        DataCollector.Instance.CodeAndSendEvent("player_die", eventParams);
    }

    public int GetNumberOfItems()
    {
        var ret = 0;
        if (SkillItems[0] != null) ret++;
        if (SkillItems[1] != null) ret++;
        ret += PropertyAffectingItems.Count;
        ret += ActionAffectingItems.Count;
        return ret;
    }

    public int GetStateOfSkillItem(int index)
    {
        if (SkillItems[index] == null)
            return -1; //no item
        else return SkillItems[index].CooldownCount < SkillItems[index].CooldownRounds ? 1 : 0;
        //1:can be used; 0:need cooling down
    }
}
