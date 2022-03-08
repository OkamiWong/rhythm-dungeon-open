using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResult : MonoBehaviour
{
    public static GameResult Instance;
    public struct Player
    {
        public int maxHP, strength, luck;
        public List<int> items;
    }

    public Player? lastWonPlayer;

    public List<string> beatenPlayers;
    void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else Instance = this;
        lastWonPlayer = null;
        beatenPlayers = new List<string>();
    }

    public void SaveCurrentPlayer()
    {
        var player = new Player();
        player.maxHP = PlayerStats.Instance.MaxHealth;
        player.strength = PlayerStats.Instance.rawStrength;
        player.luck = PlayerStats.Instance.rawLuck;

        var items = new List<int>();
        foreach (Item item in PlayerStats.Instance.PropertyAffectingItems)
            items.Add(GameManager.Instance.nameToItemIndex[item.itemName]);
        foreach (Item item in PlayerStats.Instance.ActionAffectingItems)
            items.Add(GameManager.Instance.nameToItemIndex[item.itemName]);
        if (PlayerStats.Instance.SkillItems[0] != null)
            items.Add(GameManager.Instance.nameToItemIndex[PlayerStats.Instance.SkillItems[0].itemName]);
        if (PlayerStats.Instance.SkillItems[1] != null)
            items.Add(GameManager.Instance.nameToItemIndex[PlayerStats.Instance.SkillItems[1].itemName]);

        player.items = items;

        lastWonPlayer = player;
    }

    public void LoadSavedPlayer()
    {
        var player = lastWonPlayer.Value;
        PlayerStats.Instance.rawMaxHealth = player.maxHP;
        PlayerStats.Instance.rawStrength = player.strength;
        PlayerStats.Instance.rawLuck = player.luck;

        foreach (int index in player.items)
        {
            var item = GameManager.Instance.itemPrefabs[index];
            Debug.Log("Add item " + item.GetComponent<Item>().itemName + " to player...");
            if (item.GetComponent<ActionAffectingItem>() != null)
                PlayerStats.Instance.AddActionAffectingItem(item);
            else if (item.GetComponent<PropertyAffectingItem>() != null)
                PlayerStats.Instance.AddPropertyAffectingItem(item);
            else
            {
                if (PlayerStats.Instance.SkillItems[0] == null)
                    PlayerStats.Instance.AddSkillItem(item, 0);
                else PlayerStats.Instance.AddSkillItem(item, 1);
            }
        }
    }
}
