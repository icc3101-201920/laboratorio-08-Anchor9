﻿using Laboratorio_7_OOP_201902.Cards;
using Laboratorio_7_OOP_201902.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Laboratorio_7_OOP_201902.Interfaces;

namespace Laboratorio_7_OOP_201902
{
    [Serializable]
    public class Deck: ICharacteristics

    {

        private List<Card> cards;

        public Deck()
        {
        
        }

        public List<Card> Cards { get => cards; set => cards = value; }

        public void AddCard(Card card)
        {
            Cards.Add(card);
        }
        public void DestroyCard(int cardId)
        {
            cards.RemoveAt(cardId);
        }

        

        public void Shuffle()
        {
            Random random = new Random();
            int n = cards.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                Card value = cards[k];
                cards[k] = cards[n];
                cards[n] = value;
            }
        }
        public List<string> GetCharachteristics()
        {
            List<string> list = new List<string>;



            //Linq tipo sql

            IEnumerable<Card> MeleeTotal =
                from card in Cards
                where card.Type == EnumType.melee
                select card;

            IEnumerable<Card> RangeCardTotal =
                from card in Cards
                where card.Type == EnumType.range
                select card;

            IEnumerable<Card> LongRangeTotal =
                from card in Cards
                where card.Type == EnumType.longRange
                select card;

            IEnumerable<Card>  BuffTotal =
                from card in Cards
                where card.Type == EnumType.buff
                select card;

            IEnumerable<Card> WheaterTotal =
                from card in Cards
                where card.Type == EnumType.weather
                select card;

            string TotalM = Convert.ToString(MeleeTotal.Count());
            list.Add(TotalM);

            string TotalR = Convert.ToString(RangeCardTotal.Count());
            list.Add(TotalR);

            string TotalLR = Convert.ToString(LongRangeTotal.Count());
            list.Add(TotalLR);

            string TotalB = Convert.ToString(BuffTotal.Count());
            list.Add(TotalM);

            string TotalW = Convert.ToString(WheaterTotal.Count());
            list.Add(TotalM);

            //Solo agregue a la lista contar
            return list;
        }

    }
}
