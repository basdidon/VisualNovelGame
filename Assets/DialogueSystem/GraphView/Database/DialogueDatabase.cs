using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;

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
        public Player Player;
        Dictionary<Characters,Character> characters;

        private void Start()
        {
            Player = new("bas", 1000);

            characters = new Dictionary<Characters, Character>
            {
                { Characters.John, new Character("John",1000)},
                { Characters.Adam, new Character("Adam",0) },
                { Characters.Leonard, new Character("Leonard", 50)},
                { Characters.Julia, new Character("Julia",150) },
            };

            string packageString = "Player/Name";
            string packageString_2 = "Player/Money";
            ExecuteSyntax(packageString);
            ExecuteSyntax(packageString_2);
        }

        public Character GetCharacter(Characters character)
        {
            return characters[character];
        }

        void ExecuteSyntax(string syntax)
        {
            // {ModelName}/{GetProppertyName}
            string[] stringArray = syntax.Split('/');
            string modelName = stringArray[0];
            string propertyName = stringArray[1];

            if (modelName == "Player")
            {
                if(propertyName == "Name")
                {
                    Debug.Log(Player.Name);
                }else if(propertyName == "Money")
                {
                    Debug.Log(Player.Money);
                }
            }
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
     
    [Serializable][Model]
    public class Player : Character
    {
        public Player(string name, int money) : base(name, money) { }

        // Player/SpendMoney:100
        [Func]
        public void SpendMoney(int cost)
        {
            if (Money < cost)
                throw new Exception();

            Money -= cost;
        }
    }

    public class PlayerController
    {

    }

    public class ModelAttribute : Attribute
    {

    }

    public class FuncAttribute : Attribute
    {

    }

}