using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Level1/LinkedItemsSO")]

public class LinkedItemsSO : ScriptableObject
{
    [Serializable]
    public struct LinkedPair
    {
        public int object1;
        public int object2;
    }

    public LinkedPair[] linkedPairs;
}
