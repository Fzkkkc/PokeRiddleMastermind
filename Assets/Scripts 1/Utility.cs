using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static List<int> Shuffle(List<int> cards)
    {
        var shuffledCards = new List<int>();
        while (cards.Count > 0)
        {
            var rand = Random.Range(0, cards.Count);
            shuffledCards.Add(cards[rand]);
            cards.RemoveAt(rand);
        }

        return shuffledCards;
    }
}