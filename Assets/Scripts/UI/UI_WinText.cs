using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_WinText : MonoBehaviour
{
    [SerializeField] private List<Sprite> number;
    private int currentNumber;
    private Image numberSprite;

    void Start()
    {
        numberSprite = GetComponent<Image>();

        RestartWinText();
    }

    public void AddNumber()
    {
        currentNumber++;

        if(currentNumber > 9)
            numberSprite.sprite = number[0];
        else
            numberSprite.sprite = number[currentNumber];
    }

    public void RestartWinText()
    {
        currentNumber = 0;
        numberSprite.sprite = number[currentNumber];
    }
}
