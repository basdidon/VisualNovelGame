using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
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
        Dictionary<Characters,Character> characters;

        private void Start()
        {
            characters = new Dictionary<Characters, Character>
            {
                { Characters.John, new Character() { Name = "John", Money = 1000 } },
                { Characters.Adam, new Character() { Name = "Adam", Money = 0} },
                { Characters.Leonard, new Character() { Name = "Leonard", Money = 50 } },
                { Characters.Julia, new Character() { Name = "Julia", Money = 150} },
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
        public string Name { get; set; }
        [field:SerializeField]
        public int Money { get; set; }
    }
}