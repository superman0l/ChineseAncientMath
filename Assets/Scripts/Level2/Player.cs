using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Vector3[] points;
    private int destPoint = 0;
    [SerializeField] public float speed = 1.0f;
    Coroutine currentMove;
    public bool isMoving { get; set; } = false;

    void Start()
    {
        // StartCoroutine(MoveToNextPoint());
    }

    IEnumerator MoveToNextPoint()
    {
        isMoving = true;SoundManager.Instance.PlayMoveSound();
        while (destPoint < points.Length)
        {
            Vector3 target = points[destPoint];
            while (Vector3.Distance(transform.position, target) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, Time.deltaTime * speed);
                yield return null;
            }

            destPoint++;
        }
        isMoving = false;SoundManager.Instance.StopMoveSound();
        if (AbacusManager.Instance.isComplete)
            AbacusManager.Instance.Finish();
        AbacusManager.Instance.SetNumber();
    }

    public IEnumerator UpdatePoints(Vector3[] newPoints)
    {
        while (AbacusManager.Instance.isMoving())
        {
            yield return null;
        }

        if (currentMove != null)
        {
            StopCoroutine(currentMove);
        }

        points = newPoints;
        destPoint = 0;
        currentMove = StartCoroutine(MoveToNextPoint());
    }
    
}
