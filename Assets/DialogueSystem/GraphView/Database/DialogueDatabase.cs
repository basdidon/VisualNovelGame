using System.Collections.Generic;
using UnityEngine;
using System;

namespace BasDidon.Dialogue.VisualGraphView
{
    public enum Characters
    {
        John,
        Adam,
        Leonard,
        Julia
    }

    public class DialogueDatabase : MonoBehaviour
    {
        public static DialogueDatabase Instance { get; private set; }

        public Player Player;
        Dictionary<Characters,Character> characters;

        private void Awake()
        {
            if(Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            Player = new("H8", 1000);

            characters = new Dictionary<Characters, Character>
            {
                { Characters.John, new Character("John",1000)},
                { Characters.Adam, new Character("Adam",0) },
                { Characters.Leonard, new Character("Leonard", 50)},
                { Characters.Julia, new Character("Julia",150) },
            };
        }

        public Character GetCharacter(Characters character)
        {
            return characters[character];
        }
    }

    [Serializable]
    public class Character
    {
        [field:SerializeField]
        public string Name { get; protected set; }
        [field:SerializeField]
        public int Money { get; protected set; }

        public Character(string name, int money)
        {
            Name = name;
            Money = money;
        }
    }

    [Serializable]
    public class Player : Character
    {
        public Player(string name, int money) : base(name, money) { }

        public void SpendMoney(int cost)
        {
            if (Money < cost)
                throw new Exception();

            Money -= cost;
        }
        public void GainMoney(int moneyToGain)
        {
            if(moneyToGain >= 0)
            {
                Money += moneyToGain;
            }
        }
    }
}