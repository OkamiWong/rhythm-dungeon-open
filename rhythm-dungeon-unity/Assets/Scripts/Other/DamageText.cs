using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    public int maxXOffset;
    public int startY, endY;
    public float time;
    Text text;

    void Start()
    {
        gameObject.transform.localPosition =
            new Vector3(Random.Range((float)(-maxXOffset), (float)(maxXOffset)), startY, 0f);
        StartCoroutine(AscendCoroutine());
    }

    IEnumerator AscendCoroutine()
    {
        var translation = (float)(endY - startY);
        var speed = translation / time;
        var deltaTime = 0f;
        while (deltaTime < time)
        {
            yield return new WaitForFixedUpdate();
            deltaTime += Time.fixedDeltaTime;
            gameObject.transform.localPosition += Vector3.up * speed * Time.fixedDeltaTime;
        }

        Destroy(gameObject);
    }

    public void Initialize(int damage, GameObject canvas)
    {
        gameObject.transform.SetParent(canvas.transform);
        text = GetComponent<Text>();
        text.text = "-" + damage;
        gameObject.transform.localScale = Vector3.one * (Mathf.Log(damage, 50f));
    }
}
