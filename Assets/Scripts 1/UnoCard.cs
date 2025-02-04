using System;
using UnityEngine;
using UnityEngine.UI;

public class UnoCard : MonoBehaviour
{
    public enum CardType
    {
        Red,
        Green,
        Blue,
        Yellow
    }

    public enum SpecialCard
    {
        None,
        Skip = 10,
        Reverse = 11,
        Draw2 = 12,
        Wild = 13,
        Draw4Wild = 14
    }

    public event Action<UnoCard, int, Owner> OnSelected;
    public int id;
    private Image img;
    public Sprite BackImg;
    private Sprite FrontImg;
    public MoveObject moveComponent;
    public Owner owner;
    public int globalCardIdx;
    private CardType Color;
    private int Number;
    public int TurnChangeAmount = 1;
    public SpecialCard Type = SpecialCard.None;
    public int AccumulatedCards;
    public Owner LastClicked;

    private void Awake()
    {
        img = GetComponent<Image>();
        moveComponent = GetComponent<MoveObject>();
        moveComponent.targetTransform = transform;
    }

    public void ShowBackImg(bool back)
    {
        if (back)
            img.sprite = BackImg;
        else
            img.sprite = FrontImg;
    }

    public void InitCard(int id, Sprite sprite, int _globalCardIdx)
    {
        this.id = id;

        if (sprite != null && img != null)
        {
            img.sprite = sprite;
            FrontImg = sprite;
        }

        ShowBackImg(true);
        globalCardIdx = _globalCardIdx;
        SetNumberAndColor();


        TurnChangeAmount = Type == SpecialCard.Skip ? 2 : Type == SpecialCard.Reverse ? -1 : 1;
        AccumulatedCards = Type == SpecialCard.Draw2 ? 2 : Type == SpecialCard.Draw4Wild ? 4 : 0;
    }

    public void OnClick(int Player = -1)
    {
        if (Player == -1)
            LastClicked = Owner.Player1; //TODO:multiplayer
        else
            LastClicked = (Owner) Player;
        OnSelected?.Invoke(this, globalCardIdx, owner);
    }

    public void SetWildColor(CardType wildColor)
    {
        Color = wildColor;
    }

    public void Move(Vector3 EndPosition, Action callback)
    {
        moveComponent.Move(EndPosition, callback);
    }

    private void SetNumberAndColor()
    {
        if (id < 56)
        {
            var row = id / 14 % 4;
            Color = row == 0 ? CardType.Red : row == 1 ? CardType.Yellow : row == 2 ? CardType.Green : CardType.Blue;
            Number = id % 14;

            if (Number > 9)
                Type = (SpecialCard) Number;
        }
        else
        {
            var id2 = id - 56;
            var row = id2 / 13 % 4;
            Color = row == 0 ? CardType.Red : row == 1 ? CardType.Yellow : row == 2 ? CardType.Green : CardType.Blue;

            Number = id2 % 13 + 1;
            if (Number > 9)
                Type = Number == 13 ? SpecialCard.Draw4Wild : (SpecialCard) Number;
        }

    }

    public bool AcceptsCard(UnoCard card)
    {
        if (card.Type == SpecialCard.Wild || card.Type == SpecialCard.Draw4Wild)
            return true;

        return card.Number == Number || card.Color == Color;
    }

    public CardType GetColor()
    {
        return Color;
    }
}