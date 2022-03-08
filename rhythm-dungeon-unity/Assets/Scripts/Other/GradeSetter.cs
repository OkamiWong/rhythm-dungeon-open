using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GradeSetter : MonoBehaviour
{
    public enum Grade
    {
        Fail, Fine, Cool
    }

    public Color cool, fine, fail;

    Text text;

    private void Start()
    {
        text = GetComponent<Text>();
        gameObject.SetActive(false);
    }

    public void SetGradeText(Grade grade) // 0 = fail, 1 = fine, 2 = cool
    {
        switch (grade)
        {
            case Grade.Fail:
                text.color = fail;
                text.text = "Fail";
                break;
            case Grade.Fine:
                text.color = fine;
                text.text = "Fine";
                break;
            case Grade.Cool:
                text.color = cool;
                text.text = "Cool";
                break;
        }
    }
}