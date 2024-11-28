using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum Owner
{
    Player1,
    Player2,
    Player3,
    Player4,
    Discard,
    Draw
}

public class UnoGameManager : MonoBehaviour
{
    private const int TOTAL_CARDS = 108;

    public List<UnoPlayer> Players;
    public UnoDiscardPile DiscardPile;
    public UnoDrawPile DrawPile;

    private int Turn = -1;
    private int PlayerCount;
    private int ChangeTurnOrder = 1;
    public GameObject FinishPanel;
    public TMP_Text text;
    public TMP_Text textButton;
    public static float WaitForOneMoveDuration = 0.5f;
    public int MainPlayer;
    public NotifiControl NotifiControl;
    private bool Paused;

    public bool IsWin = false;
    
    private void Awake()
    {
        PlayerCount = 4;
        Turn = 0;
        var Colors = new List<UnoCard.CardType>();
        foreach (UnoCard.CardType color in Enum.GetValues(typeof(UnoCard.CardType))) Colors.Add(color);
        for (var i = 0; i < PlayerCount; i++)
        {
            Players[i].SetOwner((Owner) i);
            Players[i].GameManager = this;
            Players[i].Init();
            if (i == 0)
            {
                Players[i].SetColor(UnoColorSelect.ColorSelected);
                Colors.Remove(UnoColorSelect.ColorSelected);
            }
            else
            {
                Players[i].SetColor(Colors[0]);
                Colors.Remove(Colors[0]);
            }

            Players[i].InteractableUnoButton(false);
        }

        DrawPile.GameManager = this;
        DiscardPile.GameManager = this;
    }

    public void ContinueGame(UnoCard card = null)
    {
        if (card == null)
            ChangeTurn(DiscardPile.GetLastCard());
        else
            ChangeTurn(card);
    }

    public void Start()
    {
        DrawPile.ShuffleAndDistribute(PlayerCount);
    }

    public void Pause(bool _pause)
    {
        Paused = _pause;
        if (!_pause) UpdatePlayersTurn();
    }

    public int GetTurn()
    {
        return Turn;
    }

    public void ChangeTurn(UnoCard card = null)
    {
        if (card != null)
        {
            Turn = (Turn + card.TurnChangeAmount * ChangeTurnOrder) % PlayerCount;
            if (card.TurnChangeAmount < 0) ChangeTurnOrder = ChangeTurnOrder * -1;
        }
        else
        {
            Turn = (Turn + ChangeTurnOrder) % PlayerCount;
        }

        if (Turn < 0)
            Turn += PlayerCount;
        if (!Paused)
            UpdatePlayersTurn();
    }

    private void UpdatePlayersTurn()
    {
        for (var i = 0; i < PlayerCount; ++i) Players[i].ChangeTurnToMe(Turn == i);
    }

    public void GameStart(UnoCard firstCard)
    {
        DiscardPile.DiscardedCard(firstCard, () => { });

        if (firstCard.Type == UnoCard.SpecialCard.Reverse)
        {
            Turn = 1;
            ChangeTurn(firstCard);
        }
        else if (firstCard.Type == UnoCard.SpecialCard.Skip)
        {
            Turn = -1;
            ChangeTurn(firstCard);
        }

        else
        {
            UpdatePlayersTurn();
        }

        for (var i = 0; i < PlayerCount; ++i) Players[i].InteractableUnoButton(true);
    }

    public bool IsAcceptableToStart(UnoCard card)
    {
        return card.Type != UnoCard.SpecialCard.Draw4Wild;
    }

    public void ShowWinner(int turn)
    {
        if (PlayerPrefs.GetInt("IsFromNovell", 0) == 1)
        {
            textButton.text = turn == 0 ? "Continue" : "Retry";
        }
        else
        {
            textButton.text = turn == 0 ? "Menu" : "Retry";
        }

        IsWin = turn == 0;
        
        text.text = turn == 0 ? "YOU WON!" : "YOU LOSE";
        
        FinishPanel.SetActive(true);
    }
}