using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//The component controlling the stats of a enemy from blockchain.
public class BlockchainEnemyStats : EnemyStats
{
    public Text nameHolder;
    public Text fromBlockChainText;

    //Initialize the enemy by fetching a character from the blockchain.
    protected override void InitializeProperty()
    {
        //This should be a coroutine for the communication with blockchain is not immediate. 
        StartCoroutine(FetchCharacterAndInitialize());
    }

    IEnumerator FetchCharacterAndInitialize()
    {
        //The codes followed by this return statement will be excutated after the returned coroutine ends.

        Coroutine coroutine;
        try
        {
            coroutine = GenesisContractService.Instance.RequestRandomCharacterCoroutine(GameManager.Instance.level);
        }
        catch (System.Exception)
        {
            nameHolder.gameObject.SetActive(false);
            fromBlockChainText.gameObject.SetActive(false);

            float enhanceFactor = Mathf.Log(2f + 3f * GameManager.Instance.level, 2f);
            rawMaxHealth = (int)(rawMaxHealth * enhanceFactor);
            rawStrength = (int)(rawStrength * ((enhanceFactor - 1f) * 0.75f + 1f));
            rawLuck = rawLuck + GameManager.Instance.level * 10;

            isPropertyInitialized = true;
            yield break;
        }
        yield return coroutine;

        rawMaxHealth = (int)GenesisContractService.Instance.requestedCharacter._hp * 10;
        rawStrength = (int)GenesisContractService.Instance.requestedCharacter._str * 10;
        rawLuck = (int)GenesisContractService.Instance.requestedCharacter._luck * 10;
        nameHolder.text = GenesisContractService.Instance.requestedCharacter._name;

        CurrentHealth = rawMaxHealth;//recover

        var optionalField = GenesisContractService.Instance.requestedCharacter._optionalAttrs;
        if (optionalField.Contains("\"RD\":"))
        {
            var left = optionalField.IndexOf("\"RD\":") + 5;
            int right;
            for (int i = left; ; ++i)
            {
                if (optionalField[i] == ']')
                {
                    right = i;
                    break;
                }
            }
            optionalField = optionalField.Substring(left, right - left);
            var indices = optionalField.Split(',');
            var items = new List<GameObject>();
            foreach (string index in indices)
            {
                int intIndex;
                try
                {
                    intIndex = int.Parse(index);
                }
                catch (System.Exception)
                {
                    continue;
                }

                items.Add(GameManager.Instance.itemPrefabs[intIndex]);
            }
            foreach (GameObject item in items)
            {
                Debug.Log("Add item " + item.GetComponent<Item>().itemName + " to " + nameHolder.text);
                if (item.GetComponent<ActionAffectingItem>() != null)
                    AddActionAffectingItem(item);
                else if (item.GetComponent<PropertyAffectingItem>() != null)
                    AddPropertyAffectingItem(item);
                else
                {
                    if (SkillItems[0] == null)
                        AddSkillItem(item, 0);
                    else AddSkillItem(item, 1);
                }
            }
        }

        if (GameManager.Instance.isEndlessMode)
        {
            GameResult.Instance.beatenPlayers.Add(nameHolder.text);
        }

        isPropertyInitialized = true;
    }
}