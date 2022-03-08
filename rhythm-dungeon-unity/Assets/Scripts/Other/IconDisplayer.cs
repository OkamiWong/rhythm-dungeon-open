using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconDisplayer : MonoBehaviour
{
    public Image icon;
    public GameObject amountHolder, description;
    public Text currentCDText;
    int amount;

    public void Initialize(Item item)
    {
        icon.gameObject.SetActive(true);
        icon.sprite = item.icon;
        amount = 1;
        description.GetComponent<Text>().text = item.itemName + "\n" + item.itemDescription;
    }

    public void IncreaseAmount()
    {
        amount += 1;
        amountHolder.SetActive(true);
        amountHolder.GetComponent<Text>().text = "*" + amount;
    }

    GameObject panel;

    public void MouseEnter()
    {
        if (JSInterface.Instance.isMobile) return;
        description.transform.SetParent(transform.parent.parent, true);
        description.transform.SetAsLastSibling();
        description.SetActive(true);
    }
    public void MouseExit()
    {
        if (JSInterface.Instance.isMobile) return;
        description.transform.SetParent(transform, true);
        description.SetActive(false);
    }
}
