using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDisplay : MonoBehaviour
{
    Card currentCard;
    SpriteRenderer spriteRender;

    void Start()
    {
        spriteRender = GetComponent<SpriteRenderer>();
        UpdateCard();
    }


    public void SetCard(Card card)
    {
        StopAllCoroutines();

        currentCard = card;

        StartCoroutine(BackCardAnimation());
    }


    private IEnumerator BackCardAnimation()
    {
        spriteRender.sprite = null;
        yield return new WaitForSeconds(0.2f);
        spriteRender.sprite = currentCard._artwork;
        UpdateCard();
    }

    private void UpdateCard()
    {
        if (currentCard != null)
            spriteRender.sprite = currentCard._artwork;
        else
            spriteRender.sprite = null;
    }

    public bool IsACard()
    {
        return currentCard != null;
    }

    public void ClearCards()
    {
        spriteRender.sprite = null;
    }
}
