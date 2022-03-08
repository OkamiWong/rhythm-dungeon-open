using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerAction
{
    Idle, MoveUp, MoveDown, Charge, Attack, UseItem_0, UseItem_1, Undefinded
}

//The component to handle all the behaviors of player.
public class PlayerController : MonoBehaviour
{
    //Singleton
    public static PlayerController Instance;
    //Components
    PlayerStats playerStats;
    Combat playerCombat;
    Animator playerAnimator;

    [Header("Components")]
    public GameObject playerSprite;
    public SpriteRenderer playerSpriteRenderer;

    public GameObject bonusAttackBulletPrefab;

    [Header("Parameters")]
    public float moveSpeed;

    private void Awake()
    {
        Instance = this;

        playerCombat = gameObject.AddComponent<Combat>();
        playerAnimator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        playerStats = PlayerStats.Instance;
    }

    public void DeployAction(PlayerAction action)
    {
        if (playerStats.FreezeCount > 0) return;

        switch (action)
        {
            case PlayerAction.Idle: break;
            case PlayerAction.MoveUp: Move(true); break;
            case PlayerAction.MoveDown: Move(false); break;
            case PlayerAction.Attack: Attack(); break;
            case PlayerAction.Charge: Charge(); break;
            case PlayerAction.UseItem_0: UseItem(0); break;
            case PlayerAction.UseItem_1: UseItem(1); break;
            default: break;
        }
    }

    public void BonusAttack()
    {
        playerCombat.Shoot(bonusAttackBulletPrefab, playerStats.Strength / 2);
    }

    void Move(bool isUpward)
    {
        if (!playerStats.ChangePositionState(isUpward)) return;
        StartCoroutine(MoveCoroutine(isUpward ? 1 : -1));
    }

    IEnumerator MoveCoroutine(int directionalFactor)
    {
        int totalStep = (int)(GameManager.Instance.secondPerHalfBeat / Time.fixedDeltaTime);
        float stepLength = Stats.verticalTranslation / (float)totalStep;

        for (int i = 0; i < totalStep; ++i)
        {
            transform.position += new Vector3(0f, directionalFactor * stepLength, 0f);
            yield return new WaitForFixedUpdate();
        }
    }

    void Attack()
    {
        var isCritical = (UnityEngine.Random.Range(0, 1000) < playerStats.Luck);
        playerCombat.Attack(isCritical);
        playerStats.ClearCharge();
    }

    void Charge()
    {
        playerStats.Charge();
    }

    void UseItem(int which)
    {
        if (playerStats.SkillItems[which] != null)
            playerStats.SkillItems[which].Apply();
    }

    public void PlayMoveAnimation()
    {
        playerAnimator.SetBool("Move", true);
    }

    public void StopMoveAnimation()
    {
        playerAnimator.SetBool("Move", false);
    }
}
