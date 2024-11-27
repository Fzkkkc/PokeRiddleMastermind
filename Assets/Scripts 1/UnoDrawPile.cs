using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnoDrawPile : MonoBehaviour
{
    private const int TOTAL_CARDS = 108;
    private const int PLAYER_INIT_CARDS = 5;
    public UnoCardStack DrawStack; //TODO
    public UnoGameManager GameManager;
    public GameObject cardPrefab;
    private Sprite[] CardSprites;
    private readonly List<UnoCard> AllCards = new List<UnoCard>();

    private void Awake()
    {
        DrawStack = GetComponent<UnoCardStack>();
        CardSprites = Resources.LoadAll<Sprite>("");
        DrawStack.OnCardSelected += OnCardSelected;
    }

    public void RemoveFromDraw(UnoCard card)
    {
        DrawStack.Pop(card);
    }

    private void OnCardSelected(UnoCard cardScript, int arg2, Owner owner)
    {
        if (GameManager.GetTurn() != (int) cardScript.LastClicked) return;

        GameManager.Players[GameManager.GetTurn()].DrawCard(cardScript, false, () =>
        {
            GameManager.DiscardPile.CardDrawn();
            if (GameManager.DiscardPile.CanPlayOnUpCard())
                GameManager.ChangeTurn();
            else
                GameManager.Players[GameManager.GetTurn()].PlayAgain();
        });
    }

    public void ShuffleAndDistribute(int playerCount)
    {
        var allNumbers = new List<int>();
        for (var i = 0; i < TOTAL_CARDS; i++) allNumbers.Add(i);
        allNumbers = Utility.Shuffle(allNumbers);

        for (var i = 0; i < TOTAL_CARDS; i++) AllCards.Add(MakeCard(allNumbers[i], i));

        var j = 0;
        while (AllCards.Count > j)
        {
            DrawStack.PushAndMove(AllCards[j], true, () => { });
            j++;
        }

        var drawCardCount = AllCards.Count - (4 * PLAYER_INIT_CARDS + 1);
        var x = 0;
        while (!GameManager.IsAcceptableToStart(AllCards[x])) x++;
        var firstCard = AllCards[x];
        RemoveFromDraw(AllCards[x]);


        StartCoroutine(DistCardtoPlayers(drawCardCount + 1, () =>
        {
            GameManager.GameStart(firstCard);
        }));
    }

    private IEnumerator DistCardtoPlayers(int initj, Action callback)
    {
        yield return new WaitForSeconds(5 / 2 * UnoGameManager.WaitForOneMoveDuration);

        initj = 0;
        for (var i = 0; i < GameManager.Players.Count; i++)
        for (var j = 0; j < PLAYER_INIT_CARDS; j++)
        {
            var index = initj + i * PLAYER_INIT_CARDS + j;
            var id = DrawStack.GetAllCards().Count - 1;
            var card = DrawStack.GetAllCards()[id];
            RemoveFromDraw(card);
            GameManager.Players[i].DrawCard(card, false, () =>
            {
                if (i == 0)
                    card.ShowBackImg(false);
            });
            yield return new WaitForSeconds(UnoGameManager.WaitForOneMoveDuration * 3 / 4);
        }

        callback();
    }

    private UnoCard MakeCard(int id, int globalCardIdx)
    {
        var card = Instantiate(cardPrefab);
        var cardScript = card.GetComponent<UnoCard>();
        cardScript.InitCard(id, CardSprites[id], globalCardIdx);
        return cardScript;
    }

    public UnoCard GetaCard()
    {
        return DrawStack.GetAllCards()[0];
    }
}