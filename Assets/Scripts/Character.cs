using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Character
{
    public string Name { get; }

    public Character(string name)
    {
        Name = name;
    }
}

public class Player : Character
{
    public int Money { get; private set; }

    public int STR { get; private set; }
    public int INT { get; private set; }
    public int AGI { get; private set; }

    public Player(string name):base(name)
    {
        Money = 0;
        
        STR = 1;
        INT = 1;
        AGI = 1;
    }
}

public class NPC: Character
{
    public int Love { get; private set; }

    public NPC(string name) : base(name)
    {
        Love = 0;
    }
}
