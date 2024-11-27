using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnoAI : MonoBehaviour
{
    private List<UnoCard> cards;
    public Owner Owner;
    public UnoGameManager gameManager;


    private void Play()
    {
        if (cards.Count > 0)
        {
            cards[0].OnClick((int) Owner);
            cards.RemoveAt(0);
        }
    }

    private void PrepareToPlay(UnoCardStack PlayerCardStack, UnoCardStack DrawStack, int TryNumber)
    {
        cards = new List<UnoCard>(PlayerCardStack.GetAllCards().Count + DrawStack.GetAllCards().Count);
        var AvailableCards = new List<UnoCard>();
        for (var i = 0; i < PlayerCardStack.GetAllCards().Count; i++)
            if (i >= TryNumber)
                AvailableCards.Add(PlayerCardStack.GetAllCards()[i]);

        cards.AddRange(AvailableCards);
        var DrawCards = DrawStack.GetAllCards();
        DrawCards.Reverse();
        cards.AddRange(DrawCards);
    }

    public void StartPlay(UnoCardStack PlayerCardStack = null, UnoCardStack DrawStack = null, int TryNumber = 0)
    {
        PrepareToPlay(PlayerCardStack, DrawStack, TryNumber);
        Play();
        var x = Random.Range(0, 10);
        if (x < 1)
            StartCoroutine(CheckForUno());
    }

    public UnoCard.CardType SelectColorForWild(UnoCardStack PlayerCardStack)
    {
        var colorCount = new List<int>(); //TODO:get in utility
        for (var i = 0; i < 4; i++) colorCount.Add(0);

        foreach (var card in PlayerCardStack.GetAllCards()) colorCount[(int) card.GetColor()]++;

        var max = 0;
        UnoCard.CardType color = 0;
        for (var i = 0; i < 4; i++)
            if (colorCount[i] > max)
            {
                color = (UnoCard.CardType) i;
                max = colorCount[i];
            }

        return color;
    }

    private IEnumerator CheckForUno()
    {
        yield return new WaitForSeconds(2 * UnoGameManager.WaitForOneMoveDuration);

        for (var i = 0; i < gameManager.Players.Count; i++) //TODO:Player count
            if (gameManager.Players[i].IsUno() && !gameManager.Players[i].IsImmune())
                gameManager.Players[i].Uno((int) Owner);
    }
}