using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeDestroyer : MonoBehaviour
{
    private bool working = false;
    private Vector2Int dir;
    private Vector2 stepScaleLoss;
    private Vector2 stepMove;
    private float timeCounter = 0;
    private float endTime;

    public void SwipeDestroy(Vector2Int _dir,float time)
    {
        endTime = time;
        working = true;
        dir = _dir;
        stepScaleLoss = new Vector2(transform.localScale.x / time, transform.localScale.z / time) * Time.fixedDeltaTime;
        stepMove = new Vector2(dir.x / time, dir.y / time) * Time.fixedDeltaTime;

    }

    private void FixedUpdate()
    {
        if (working)
        {
            timeCounter += Time.fixedDeltaTime;
            transform.localScale = new Vector3(transform.localScale.x - (stepScaleLoss.x*Mathf.Abs(dir.x)),transform.localScale.y, transform.localScale.z-(stepScaleLoss.y * Mathf.Abs(dir.y)));
            transform.position = new Vector3(transform.position.x + stepMove.x, transform.position.y, transform.position.z+stepMove.y);
            if (timeCounter >= endTime)
            {
                Destroy(gameObject);
            }
        }
    }
}
