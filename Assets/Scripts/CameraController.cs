using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    private Vector3 offset;
    private Vector3 offsetToGm;
    private GridManager gm;
    private Camera cam;
    private bool moveToGm = false;
    private bool following=false;
    private float speed;

    public void Awake()
    {
        cam = GetComponent<Camera>();
    }

    public void SetupCamera(GridManager _gm, Transform _player, float levelChangeSpeed)
    {
        speed = levelChangeSpeed;
        gm = _gm;
        player = _player;
        offsetToGm = transform.position - gm.transform.position;
    }

    public void StartFollow()
    {
        offset = transform.position - player.transform.position;
        following = true;
    }

    private void UpdateGmOffset(float add)
    {
        offsetToGm = new Vector3(offsetToGm.x,offsetToGm.y,offsetToGm.z+add);
    }

    public void StopFollow()
    {
        following = false;
    }

    public void MoveToGm(GridManager _gm)
    {
        UpdateGmOffset((_gm.grid[0].intArray.Length - gm.grid[0].intArray.Length) / 2f);
        gm = _gm;
        moveToGm = true;
    }

    public void FixedUpdate()
    {
        if (following)
        {
            transform.position = player.transform.position + offset;
        }
        else if (moveToGm)
        {
            transform.position = Vector3.MoveTowards(transform.position, gm.transform.position+offsetToGm, speed);

            // Check if the position of the cube and sphere are approximately equal.
            if (Vector3.Distance(transform.position, gm.transform.position+offsetToGm) < 0.001)
            {
                moveToGm = false;
            }
        }
    }

    private IEnumerator ResizeCamRoutine(float oldSize, float newSize, float time)
    {
        float elapsed = 0;
        while (elapsed <= time)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / time);

            cam.orthographicSize = Mathf.Lerp(oldSize, newSize, t);
            yield return null;
        }
    }

    public void ResizeCam(float newSize)
    {
        StartCoroutine(ResizeCamRoutine(cam.orthographicSize, newSize, 2));
    }


    public bool IsItFollowing()
    {
        return following;
    }
}
