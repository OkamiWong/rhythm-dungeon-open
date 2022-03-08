using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class SkillItem : Item
{
    private bool effecting;
    public bool Effecting
    {
        get { return effecting; }
        set
        {
            effecting = value;
            if (value == true)
            {
                lastCount = 0;
                CooldownCount = -1;
            }
            UpdateUI();
        }
    }

    [SerializeField]
    private int cooldownRounds;
    public int CooldownRounds
    {
        get
        {
            return Mathf.Max(0, cooldownRounds + userStats.DeltaCooldown);
        }
    }

    private int cooldownCount;
    public int CooldownCount
    {
        get { return cooldownCount; }
        set
        {
            cooldownCount = value;
            UpdateUI();
        }
    }

    public int lastRounds;
    [HideInInspector]
    public int lastCount;

    [HideInInspector]
    public Image iconImage;
    [HideInInspector]
    public Text cdText;

    protected virtual void Start()
    {
        Effecting = false;
        CooldownCount = CooldownRounds;
        NotificationCenter.Instance.AddEventHandler("OnHalfBeat", OnHalfBeat);
    }

    protected virtual void OnHalfBeat(object sender, EventArgs e)
    {
        if (GameManager.Instance.GState != GameManager.GameState.Combat) return;

        if (GameManager.Instance.HalfBeatCount == 1)
        {
            if (Effecting)
            {
                lastCount++;
                if (lastCount > lastRounds)
                    OnInvalid();
            }

            if (CooldownCount < CooldownRounds)
                CooldownCount++;
        }
    }

    protected List<Stats> GetOtherSideStatsList()
    {
        if (userStats.tag == "Player")
        {
            var otherSideStatsList = new List<Stats>();
            foreach (EnemyController enemyController in EnemyManager.CurrentEnemyControllers)
                otherSideStatsList.Add(enemyController.enemyStats);
            return otherSideStatsList;
        }
        else
        {
            return new List<Stats> { PlayerStats.Instance };
        }
    }

    public virtual void Apply()
    {
        if (Effecting) return;
        if (CooldownCount < CooldownRounds) return;
        Effecting = true;
    }

    public virtual void OnInvalid()
    {
        if (!Effecting) return;
        Effecting = false;
    }

    public virtual void SelfDestroy()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        NotificationCenter.Instance.RemoveEventHandler("OnHalfBeat", OnHalfBeat);
        if (Effecting) OnInvalid();
    }

    void UpdateUI()
    {
        if (iconImage == null || cdText == null) return;

        float scaler;
        if (CooldownCount == CooldownRounds)
        {
            scaler = 1f;
            cdText.text = string.Empty;
        }
        else
        {
            scaler = 0.3f;
            cdText.text = (CooldownRounds - cooldownCount).ToString();
        }
        Color c = iconImage.color;
        c.r = c.g = c.b = scaler;
        iconImage.color = c;
    }
}
