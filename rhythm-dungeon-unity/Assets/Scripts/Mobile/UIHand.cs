using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIHand : MonoBehaviour
{
    public GameObject beatPointer;
    public Sprite[] sprites;
    Image image;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    void Start()
    {
        StartCoroutine(Click());
    }

    IEnumerator Click()
    {
        int index = 0;
        float timeInterval = GameManager.Instance.secondPerHalfBeat;
        for (; ; )
        {
            if (image == null || gameObject == null) yield break;
            if (beatPointer.transform.localPosition.x >= Mathf.Epsilon)
            {
                index = (index + 1) % 2;
                image.sprite = sprites[index];
            }
            yield return new WaitForSeconds(timeInterval);
        }
    }
}
