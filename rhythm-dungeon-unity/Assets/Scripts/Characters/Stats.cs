using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public delegate void OnHittingTarget(Stats target);
public delegate void OnBeingHitted(Stats shooter);

//The base class of components handling both player's and enemies' stats. (in-progress)
public class Stats : MonoBehaviour
{
    [Header("Test")]
    public GameObject[] propertyAffectingItemsToTest;
    public GameObject[] actionAffectingItemsToTest;
    public GameObject[] skillItemsToTest;

    [Header("Parameter")]
    public static float verticalTranslation = 0.25f;
    public static float playerX = -0.5f;
    public static float[] enemyX = new float[] { 0.3f, 0.6f, 0.9f };

    [Header("Properties:")]
    public int rawMaxHealth = 100;
    public int rawStrength = 50;
    public int rawLuck = 100;
    public bool isBoss = false;

    [Header("Components")]
    public Text chargeText;
    public GameObject damageTextPrefab;
    public GameObject deathAnimPrefab;

    public int CurrentHealth { get; protected set; }

    [Header("Items")]
    //if you want to add some new items, use AddPropertyItem or AddSkillItem
    private List<PropertyAffectingItem> propertyAffectingItems = new List<PropertyAffectingItem>();
    public List<PropertyAffectingItem> PropertyAffectingItems { get { return propertyAffectingItems; } }

    private SkillItem[] skillItems = new SkillItem[] { null, null };
    public SkillItem[] SkillItems { get { return skillItems; } }

    private List<ActionAffectingItem> actionAffectingItems = new List<ActionAffectingItem>();
    public List<ActionAffectingItem> ActionAffectingItems { get { return actionAffectingItems; } }

    protected DamageEffect damageEffect;
    protected ParticleSystem recoverEffect;
    [HideInInspector]
    public SpriteRenderer sprite;
    [HideInInspector]
    public Animator animator;
    private float animatorSpeed;
    [HideInInspector]
    public GameObject canvas;

    public float ChargeFactor { get; protected set; }

    [Header("Bullets:")]
    public GameObject mainBullet;
    public List<GameObject> additiveBullets = new List<GameObject>();

    [HideInInspector]
    public List<OnHittingTarget> AfterHitFunctions = new List<OnHittingTarget>();
    [HideInInspector]
    public List<OnBeingHitted> AfterHittedFunctions = new List<OnBeingHitted>();

    [HideInInspector]
    public List<Effect> effects = new List<Effect>();

    [HideInInspector]
    public int verticalPositionState = 0; //up:1 mid:0 down:-1

    [HideInInspector]
    public int enemyHorizontalPositionState = 1; //0.3:0, 0.6:1, 0.9:2

    [HideInInspector]
    public bool fever = false;

    public int FreezeCount { get; private set; }

    [HideInInspector]
    public float lastDamage;

    protected virtual void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        if (sprite == null)
            sprite = GetComponentInChildren<SpriteRenderer>();

        animator = sprite.GetComponent<Animator>();

        canvas = transform.Find(tag + "Canvas").gameObject;

        damageEffect = new DamageEffect();
        recoverEffect = canvas.transform.Find("RecoverEffect").GetComponent<ParticleSystem>();
    }

    protected virtual void Start()
    {
        ClearCharge();
        AddTestItems();

        NotificationCenter.Instance.AddEventHandler("OnHalfBeat", HandleEffects);
    }

    protected void OnDestroy()
    {
        NotificationCenter.Instance.RemoveEventHandler("OnHalfBeat", HandleEffects);
        //destory items and effects
        foreach (Item item in skillItems)
            if (item && item.gameObject) Destroy(item.gameObject);
        foreach (Item item in actionAffectingItems)
            if (item && item.gameObject) Destroy(item.gameObject);
        foreach (Item item in propertyAffectingItems)
            if (item && item.gameObject) Destroy(item.gameObject);
        foreach (Effect effect in effects)
            if (effect) effect.OnLosingEffect();
    }

    void AddTestItems()
    {
        if (propertyAffectingItemsToTest != null)
        {
            foreach (GameObject item in propertyAffectingItemsToTest)
            {
                AddPropertyAffectingItem(item);
            }
        }
        if (actionAffectingItemsToTest != null)
        {
            foreach (GameObject item in actionAffectingItemsToTest)
            {
                AddActionAffectingItem(item);
            }
        }
        if (skillItemsToTest != null)
        {
            if (skillItemsToTest.Length > 0) AddSkillItem(skillItemsToTest[0], 0);
            if (skillItemsToTest.Length > 1) AddSkillItem(skillItemsToTest[1], 1);
        }
    }

    void HandleEffects(object sender, EventArgs e)
    {
        if (GameManager.Instance.GState != GameManager.GameState.Combat) return;
        if (GameManager.Instance.HalfBeatCount != 1) return;

        var statesToRemove = new List<Effect>();
        foreach (Effect effect in effects)
        {
            effect.life--;
            effect.OnEffecting();
            if (effect.life == 0)
            {
                effect.OnLosingEffect();
                statesToRemove.Add(effect);
            }
        }
        foreach (Effect effect in statesToRemove)
            effects.Remove(effect);
    }

    public virtual PropertyAffectingItem AddPropertyAffectingItem(GameObject itemPrefab)
    {
        GameObject item = Instantiate(itemPrefab);
        var component = item.GetComponent<PropertyAffectingItem>();
        component.userStats = this;
        propertyAffectingItems.Add(component);
        return component;
    }

    public virtual ActionAffectingItem AddActionAffectingItem(GameObject itemPrefab)
    {
        GameObject item = Instantiate(itemPrefab);
        var component = item.GetComponent<ActionAffectingItem>();
        actionAffectingItems.Add(component);
        component.userStats = this;
        component.OnAdded(this);
        return component;
    }

    public virtual SkillItem AddSkillItem(GameObject itemPrefab, int which)
    {
        if (which >= skillItems.Length) return null;

        //destory old item
        if (skillItems[which] != null)
            skillItems[which].SelfDestroy();

        GameObject item = Instantiate(itemPrefab);
        var component = item.GetComponent<SkillItem>();
        component.userStats = this;
        skillItems[which] = component;

        return component;
    }

    public virtual void TakingDamage(float damage, bool clearChargeFlag = true)
    {
        if (!this || !gameObject) return;

        int dam = (int)damage;
        dam = dam < 0 ? 0 : dam;
        CurrentHealth -= dam;
        lastDamage = dam;
        if (clearChargeFlag) ClearCharge(); //clear charge or not depends on flag

        //damage effect
        if (dam > 0)
        {
            StartCoroutine(damageEffect.OnDamage(sprite));
            var damageText = Instantiate(damageTextPrefab);
            damageText.GetComponent<DamageText>().Initialize(dam, canvas);
        }
    }

    public virtual void AddingHealth(float recoverAmount)
    {
        int recover = Mathf.Min((int)recoverAmount, MaxHealth - CurrentHealth);
        CurrentHealth += recover;

        //recover effect
        if (recover > 0) recoverEffect.Play();
    }

    public virtual void Recover()
    {
        AddingHealth(MaxHealth - CurrentHealth);
    }

    public virtual void Death()
    {
        if (!this || !gameObject) return;

        if (tag == "Enemy")
        {
            if (isBoss)
            {
                var death = Instantiate(deathAnimPrefab);
                death.transform.position = transform.position;
            }
            else
            {
                var death = Instantiate(deathAnimPrefab);
                death.transform.position = transform.position;
                var character = death.GetComponentInChildren<SpriteRenderer>();
                character.sprite = sprite.sprite; //change image
                character.transform.localScale = sprite.transform.localScale;
            }
        }
    }

    public int MaxHealth
    {
        get
        {
            int delta = 0;
            foreach (PropertyAffectingItem item in propertyAffectingItems)
                delta += item.CalculateDeltaMaxHealth();
            return delta + rawMaxHealth;
        }
    }

    public int Strength
    {
        get
        {
            int delta = 0;
            foreach (PropertyAffectingItem item in propertyAffectingItems)
                delta += item.CalculateDeltaStrength();
            if (fever)
                delta += (int)(rawStrength * 0.5f);
            return delta + rawStrength;
        }
    }

    public int Luck
    {
        get
        {
            int delta = 0;
            foreach (PropertyAffectingItem item in propertyAffectingItems)
                delta += item.CalculateDeltaLuck();
            return delta + rawLuck;
        }
    }

    public float DeltaChargeFactor
    {
        get
        {
            float dcf = 1.25f;  //initial delta charge factor
            foreach (PropertyAffectingItem item in propertyAffectingItems)
                dcf += item.CalculateDeltaChargeFactor();
            return dcf;
        }
    }

    public int DeltaCooldown
    {
        get
        {
            int dcd = 0;
            foreach (PropertyAffectingItem item in propertyAffectingItems)
                dcd += item.CalculateDeltaCooldown();
            return Math.Min(dcd, 1);
        }
    }

    public virtual bool IsMoveViable(bool isUpward)
    {
        if (verticalPositionState == 1 && isUpward) return false;
        if (verticalPositionState == -1 && !isUpward) return false;
        return true;
    }

    public bool ChangePositionState(bool isUpward)
    {
        if (!IsMoveViable(isUpward)) return false;

        verticalPositionState += isUpward ? 1 : -1;
        return true;
    }

    public void Charge()
    {
        if (ChargeFactor < 1.01f)
        {
            ChargeFactor += DeltaChargeFactor;
            UpdateChargeText();
        }
    }

    public void ClearCharge()
    {
        ChargeFactor = 1f;
        UpdateChargeText();
    }

    public void Freeze()
    {
        FreezeCount++;
        //stop animations when frozen
        animatorSpeed = animator.speed;
        animator.speed = 0;
    }

    public void Unfreeze()
    {
        if (FreezeCount > 0)
            FreezeCount--;
        if (FreezeCount == 0)
            animator.speed = animatorSpeed;

    }

    void UpdateChargeText()
    {
        if (!chargeText) return;
        if (Mathf.Abs(ChargeFactor - 1f) < Mathf.Epsilon)
            chargeText.gameObject.SetActive(false);
        else
        {
            chargeText.gameObject.SetActive(true);
            chargeText.text = "+ " + ((int)(100 * ChargeFactor - 100)) + "%";
        }
    }
}
