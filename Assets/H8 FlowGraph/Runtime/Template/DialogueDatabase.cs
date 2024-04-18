using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace H8.FlowGraph.NodeTemplate
{
    public class DialogueDatabase : MonoBehaviour
    {
        public static DialogueDatabase Instance { get; private set; }

        public Player Player;
        Dictionary<string, Character> Characters;

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
            Characters = new();

            string[] guids = AssetDatabase.FindAssets($"t:{ typeof(CharacterData)}");
            foreach(var guid in guids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<CharacterData>(assetPath);
                Characters.Add(guid, Character.GetCharacterFromCharacterData(asset));
            }
        }

        public Character GetCharacter(string guid)
        {
            return Characters[guid];
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

        public static Character GetCharacterFromCharacterData(CharacterData characterData)
        {
            return new Character(characterData.Name, 1000);
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