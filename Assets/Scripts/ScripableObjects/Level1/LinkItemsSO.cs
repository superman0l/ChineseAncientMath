using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Level1/LinkItemsSO")]
public class LinkItemsSO : ScriptableObject
{
    [Serializable]
    public struct ID_Object
    {
        public int key;
        public GameObject value;
    }

    public ID_Object[] iD_Objects;
}
