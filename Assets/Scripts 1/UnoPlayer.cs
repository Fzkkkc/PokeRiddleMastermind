using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnoPlayer : MonoBehaviour
{
    public UnoCardStack cardStack;
    public GameObject MyTurnImage;
    public GameObject SelectColorPanel;
    [NonSerialized] public UnoGameManager GameManager;
    [SerializeField] private Owner handOwner;
    private UnoAI AI;
    private const string ManagerTagName = "GameController";
    private int TryNumber;
    private bool UnoImmune;
    private Button UnoButon;
    public List<Sprite> PlayerColorImg;
    public List<string> PlayerColorHex;
    public Image CardPlaceImage;
    public Image PlayerImg;

    public void Init()
    {
        cardStack.OnCardSelected += OnCardSelected;
        if (GetComponent<UnoAI>() != null)
        {
            AI = GetComponent<UnoAI>();
            AI.gameManager = GameManager;
        }

        UnoButon = GetComponentInChildren<Button>();
    }

    public void InteractableUnoButton(bool interactable)
    {
        UnoButon.interactable = interactable;
    }

    public bool AllCardsPlayed()
    {
        return cardStack.IsEmpty();
    }

    public void SetColor(UnoCard.CardType color)
    {
        Color temp;
        if (ColorUtility.TryParseHtmlString(PlayerColorHex[(int) color] + "67", out temp)) CardPlaceImage.color = temp;
        PlayerImg.sprite = PlayerColorImg[(int) color];
    }

    public void RemoveFromHand(UnoCard card)
    {
        cardStack.Pop(card);
    }

    public void SetOwner(Owner _owner)
    {
        handOwner = _owner;
        cardStack.owner = _owner; 
    }

    public void DrawCard(UnoCard card, bool isForUno, Action callback)
    {
        GameManager.DrawPile.RemoveFromDraw(card);
        if (!isForUno)
            Immune(false);

        cardStack.PushAndMove(card, false, () =>
        {
            if ((int) handOwner == GameManager.MainPlayer) //TODO: in online have to change
                card.ShowBackImg(false);
            callback();
        });
    }

    public void ChangeTurnToMe(bool isMyTurn)
    {
        MyTurnImage.SetActive(isMyTurn);
        TryNumber = 0;
        if (AI != null)
            if (isMyTurn)
                StartCoroutine(AIPlay());
    }

    private IEnumerator SelectWildCardColor(UnoCard cardScript)
    {
        if (AI == null)
        {
            SelectColorPanel.SetActive(true);
        }
        else
        {
            yield return new WaitForSeconds(UnoGameManager.WaitForOneMoveDuration);

            GameManager.DiscardPile.SetWildLastCardColor(
                AI.SelectColorForWild(cardStack)
            );
            GameManager.ContinueGame();
        }
    }

    public void PlayAgain()
    {
        TryNumber++;
        if (AI != null)
            StartCoroutine(AIPlay());
    }

    public void ColorSelected(int color)
    {
        GameManager.DiscardPile.SetWildLastCardColor((UnoCard.CardType) color);

        SelectColorPanel.SetActive(false);
        GameManager.ContinueGame();
    }

    private IEnumerator AIPlay()
    {
        AI.Owner = handOwner;
        yield return new WaitForSeconds(0.2f); 
        AI.StartPlay(cardStack, GameManager.DrawPile.DrawStack, TryNumber);
    }

    public void OnCardSelected(UnoCard card, int globalCardIdx, Owner owner)
    {
        if (GameManager.GetTurn() == (int) handOwner && GameManager.GetTurn() == (int) card.LastClicked)
        {
            if (GameManager.DiscardPile.CanPlayOnUpCard() && GameManager.DiscardPile.CanPlayThisCard(card))
            {
                RemoveFromHand(card); //TODO: move in discard in pile code
                Immune(false);
                GameManager.DiscardPile.DiscardedCard(card, () =>
                {
                    if (HasWon())
                    {
                        GameManager.ShowWinner((int) handOwner);
                        return;
                    }

                    if (GameManager.DiscardPile.ColorSelectIsNeeded())
                        StartCoroutine(SelectWildCardColor(card));
                    else
                        GameManager.ContinueGame(card);
                });
            }
            else
            {
                PlayAgain();
            }
        }
    }

    public bool HasWon()
    {
        return cardStack.IsEmpty();
    }

    public void Uno(int callerID)
    {
        if (callerID != (int) handOwner)
        {
            if (IsUno() && !IsImmune())
            {
                Immune(true);
                GameManager.NotifiControl.ShowNotification("forgot uno!", 1);
                DrawCard(GameManager.DrawPile.GetaCard(), true,
                    () => { DrawCard(GameManager.DrawPile.GetaCard(), true, () => { }); });
            }
        }
        else
        {
            Immune(true);
        }
    }

    public void UnoClicked()
    {
        Uno(GameManager.MainPlayer);
    }

    public bool IsUno()
    {
        return cardStack.HasOneCard();
    }

    public void Immune(bool immune)
    {
        UnoImmune = immune;
    }

    public bool IsImmune()
    {
        return UnoImmune;
    }
}