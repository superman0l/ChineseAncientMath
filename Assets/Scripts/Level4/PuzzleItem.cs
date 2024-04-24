using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleItem : MonoBehaviour
{

    private bool isSelected, isFixed;
    private int angleIndex = 0;

    [SerializeField] public int id;
    [SerializeField] public int[] angle;
    [SerializeField] public SpriteRenderer spriteRenderer;
    [SerializeField] public float distance;

    private void Start()
    {
        isSelected = false; 
        spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
    }

    public bool CheckFrontItem(Vector3 dir)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position + dir * distance, dir, 0.2f);
        // 向物体前方发射一条射线，检测是否有物体
        if (hit.collider != null && hit.collider.gameObject.tag == "PuzzleItem")
        {
            // Debug.Log(hit.collider.gameObject.name);
            return true;
        }
        return false;
    }

    private void Update()
    {
        if (isFixed)
        {
            return;
        }
        if (isSelected)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePos.x, mousePos.y, transform.position.z);
            if (Input.GetMouseButtonDown(1))
            {
                angleIndex = (angleIndex+1) % angle.Length;
                transform.rotation = Quaternion.Euler(0, 0, angle[angleIndex]);
            }
        }
    }

    private void OnMouseDown()
    {
#if UNITY_EDITOR
       
#endif
        if (isFixed) return;
        if (!isSelected)
        {
            SoundManager.Instance.PlayClickSound();
            ConveyorBelt.Instance.RemoveItem(transform);
            spriteRenderer.maskInteraction = SpriteMaskInteraction.None;
            isSelected = true;
        }
        else
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Ray2D ray = new Ray2D(mousePosition, Vector2.zero);
            Debug.DrawRay(ray.origin, ray.direction * 1, Color.red);
            RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction);

            foreach (RaycastHit2D hit in hits)
            {
                Debug.Log(hit.transform.gameObject.name);
                if (hit.collider != null && hit.transform.gameObject.name == "Trash")
                {
                    //Destroy(gameObject);
                    SoundManager.Instance.PlayClick2Sound();

                    isFixed = true; isSelected = false;
                    spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                    StartCoroutine(MoveDestroy(hit.transform));
                }
            }

            Vector2 fixedPos;
            if(PuzzleManager.Instance.Check(id, transform.position, (int)Mathf.Round(transform.rotation.eulerAngles.z), out fixedPos, gameObject))
            {
                transform.position = new Vector3(fixedPos.x, fixedPos.y, transform.position.z);
                isSelected = false; isFixed = true;
            }
        }
    }

    IEnumerator MoveDestroy(Transform trash)
    {
        float speed = 0.04f;
        transform.position = new Vector3(trash.position.x, transform.position.y, transform.position.z);
        float top = 4f;
        while(transform.position.y <= top)
        {
            transform.position += Vector3.up * speed;
            yield return 1f;
        }
        Destroy(gameObject);
    }
}
