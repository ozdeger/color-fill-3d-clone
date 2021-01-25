using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Vector2 startPos;
    public float speed;
    public int upMax;
    public Vector2Int dir;
    private Vector3 target;
    private bool goingUp = true;

    private void Awake()
    {
        EnemyHolderStatic.enemies.Add(this);
        startPos = new Vector2(transform.position.x, transform.position.z);
        target = new Vector3(transform.position.x + (dir.x * upMax), transform.position.y, transform.position.z + (dir.y * upMax));
        speed *= Time.fixedDeltaTime;
    }

    private void FixedUpdate()
    {
        
        transform.position = Vector3.MoveTowards(transform.position,target,speed);
        if (Vector3.Distance(transform.position, target) < 0.001)
        {
            if (goingUp)
            {
                goingUp = false;
                target = new Vector3(startPos.x, transform.position.y, startPos.y);
            }
            else
            {
                goingUp = true;
                target = new Vector3(transform.position.x + (dir.x * upMax), transform.position.y, transform.position.z + (dir.y * upMax));
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.transform.TryGetComponent<Grid>(out Grid grid))
        {
            if (grid.state == 1)
            {
                GetComponent<MeshRenderer>().enabled = false;
                GetComponent<Collider>().enabled = false;
                ActivateParticle();
                Invoke("GetDestroyed", 1f);
            }
            else if(grid.state == 2)
            {
                GetComponent<Animator>().SetTrigger("flash");
                grid.transform.parent.GetComponent<GridManager>().EnemyHitLine(grid.point);
            }
        }
    }

    private void ActivateParticle()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }

    private void GetDestroyed()
    {
        Destroy(gameObject);
    }
}
