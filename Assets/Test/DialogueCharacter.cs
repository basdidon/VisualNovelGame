using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    [DlClass]
    public class DialogueCharacter : MonoBehaviour
    {
        [DlValue]
        [field:SerializeField] 
        public string Name { get; set; }

        [DlValue]
        [field: SerializeField]
        public int Coin { get; set; }

        public int Mon { get; set; }
    }
}
