using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : MonoBehaviour
{
    public float speed;
    public Vector3 moveTarget;
    private Vector3 startPos;
    private bool working = false;
    private void Awake()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        
        if (working)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPos + moveTarget, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, startPos + moveTarget) < 0.001)
            {
                print("Crystal Collected - Connect here to UI");
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerStay(Collider coll)
    {
        if(coll.transform.TryGetComponent<Grid>(out Grid grid))
        {
            if (grid.state == 1)
            {
                Collect();
            }
        }
    }

    public void Collect()
    {
        working = true;
    }
}
