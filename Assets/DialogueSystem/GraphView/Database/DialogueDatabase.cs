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

    public static class DialogueDatabase
    {
        public static Character GetCharacter(Characters character)
        {
            return character switch
            {
                Characters.John => new Character() { Name = "John", Money = 1000 },
                Characters.Adam => new Character() { Name = "Adam", Money = 0},
                Characters.Leonard => new Character() { Name = "Leonard", Money = 50 },
                Characters.Julia => new Character() { Name = "Julia", Money = 150},
                _ => throw new Exception(),
            };
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