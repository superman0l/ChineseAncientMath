using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LinkItem : MonoBehaviour
{
    bool ActivatedState = false;
    LinkItem self;
    int iD;
    // Start is called before the first frame update
    void Start()
    {
        self = GetComponent<LinkItem>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetID(int ID)
    {
        iD = ID;
    }

    public int GetID() { return iD; }

    private void OnMouseDown()
    {
        SoundManager.Instance.PlayClickSound();
        if (ActivatedState)
        {
            ItemsManager.Instance.ClearSelected();
            Close();
            return;
        }
        Debug.Log("click");
        ItemsManager.Instance.LinkItemSelected(self);
        ActivatedState = true;
        transform.DOScale(1f, 0.3f); // 物品变大
    }

    public void Close()
    {
        ActivatedState = false;
        transform.DOScale(0.8f, 0.3f); // 物品恢复原大小
    }

    public void DestroySelf()
    {
        StartCoroutine(DestroyAnim());
    }
    IEnumerator DestroyAnim()
    {
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Color color = spriteRenderer.color;
        color.a = 0f;
        Tween tween = spriteRenderer.DOColor(color, 0.3f);
        yield return tween.WaitForCompletion();
        Destroy(gameObject);
    }
}
