using System;
using System.Collections.Generic;
using UnityEngine;

public class UnoCardStack : MonoBehaviour
{
    private readonly List<UnoCard> cards = new List<UnoCard>();
    public bool isDiscard;
    public Owner owner;
    private int Discard_Z;
    public Action<UnoCard, int, Owner> OnCardSelected;
    public AudioSource CardSound;

    public bool IsEmpty()
    {
        return cards.Count == 0; 
    }

    public void Pop(UnoCard card)
    {
        card.OnSelected -= OnCardSelected;

        cards.Remove(card);
    }

    public void PlayCardSound()
    {
        CardSound.Play();
    }

    public void PushAndMove(UnoCard card, bool Silent, Action callback)
    {
        if (!Silent) PlayCardSound();

        card.owner = owner;
        cards.Add(card);

        card.OnSelected += OnCardSelected; //TODO


        if (isDiscard)
        {
            card.transform.rotation = Quaternion.Euler(0, 0, Discard_Z);
            card.ShowBackImg(false);
            Discard_Z += 45;
        }
        else
        {
            card.transform.rotation = transform.rotation;
        }

        card.Move(transform.position, () =>
        {
            card.transform.SetParent(transform);

            callback();
        });
    }

    public List<UnoCard> GetAllCards()
    {
        var ALLcards = new List<UnoCard>();
        for (var i = 0; i < cards.Count; i++) ALLcards.Add(cards[i]);

        return ALLcards;
    }

    public bool HasOneCard()
    {
        return cards.Count == 1;
    }
}