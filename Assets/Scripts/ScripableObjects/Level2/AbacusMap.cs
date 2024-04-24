using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level2/AbacusMap")]
public class AbacusMap : ScriptableObject
{
    [Serializable]
    public struct pair
    {
        public int num;
        public int[] balls;
    }
    public pair[] pairs;
}
