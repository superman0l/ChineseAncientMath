using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;




public class FieldItem : MonoBehaviour
{
    [SerializeField] SpriteRenderer itemSpriteRenderer;
    [SerializeField] TextMeshPro textMesh;

    public void SetNumber(int num)
    {
        textMesh.gameObject.SetActive(true);
        string numstr = num.ToString();
        string res = "";
        for(int i = 0; i < numstr.Length; i++)
        {
            res += "<sprite=" + numstr[i] + "> ";
        }
        textMesh.text = res;
    }

    public void Change(Sprite sprite)
    {
        itemSpriteRenderer.sprite = sprite;
    }
}
