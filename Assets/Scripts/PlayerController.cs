using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float levelChangeSpeed;
    public float distanceRequirement;
    public GameObject linePrefab;
    public CameraController cameraController;
    public GameObject blowUpEffect;
    private Vector2 dir;
    private Vector2 oldDir;
    private Vector2 clickPosition;
    private Vector2 savedPosition;
    private Grid currentGrid;
    private Grid oldGrid;
    private float Ypos;
    private float startXpos;
    private bool canTurn = true;
    private bool startGridFilled = false;
    private bool movingToNextLevel = false;
    private Vector3 levelChangeTargetPosition;
    private bool creatingLine = false;
    [HideInInspector]
    public GridManager gm;

    

    private void Awake()
    {
        Ypos = transform.position.y;
        startXpos = transform.position.x;
        cameraController.SetupCamera(gm, transform,levelChangeSpeed);
    }

    private void Update()
    {
        if (!movingToNextLevel)
        {
            if (Input.GetMouseButtonDown(0))
            {
                clickPosition = Input.mousePosition;
            }
            if (Input.GetMouseButton(0))
            {
                if (((Vector2)Input.mousePosition - clickPosition).magnitude > distanceRequirement)
                {
                    Vector2 swipeDir = ((Vector2)Input.mousePosition - clickPosition).normalized;
                    clickPosition = Input.mousePosition;
                    if (Mathf.Abs(swipeDir.x) + Mathf.Abs(swipeDir.y) >= 1)
                    {
                        if (Mathf.Abs(swipeDir.x) > Mathf.Abs(swipeDir.y))
                        {
                            swipeDir.y = 0;
                        }
                        else
                        {
                            swipeDir.x = 0;
                        }
                    }
                    // Dir modifications
                    dir = new Vector2Int(Mathf.RoundToInt(swipeDir.x), Mathf.RoundToInt(swipeDir.y));
                    if (!canTurn)
                    {
                        dir = Vector2.zero;
                    }

                    if (dir == Vector2.zero)
                    {
                        dir = oldDir;
                    }



                    if (dir != oldDir)
                    {
                        transform.position = new Vector3(currentGrid.transform.position.x, Ypos, currentGrid.transform.position.z);
                    }

                    bool canImoveTo = gm.CanIMoveTo((int)(currentGrid.point.x + dir.x), (int)(currentGrid.point.y + dir.y));
                    if (!canImoveTo)
                    {
                        HitWall();
                    }

                    
                    if (oldGrid && creatingLine)
                    {
                        if (oldGrid.point.x == currentGrid.point.x + dir.x && oldGrid.point.y == currentGrid.point.y + dir.y)
                        {
                            dir = oldDir;
                        }
                    }

                    oldDir = dir;


                }

            }
        }
        
    }

    private void FixedUpdate()
    {
        if (!movingToNextLevel)
        {
            transform.position = new Vector3(transform.position.x + (dir.x * speed), transform.position.y, transform.position.z + (dir.y * speed));

            if (gm.IsLevelCleared())
            {
                NextLevel();
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, levelChangeTargetPosition, levelChangeSpeed);

            if (Vector3.Distance(transform.position, levelChangeTargetPosition) < 0.001)
            {
                levelChangeTargetPosition = new Vector3(startXpos, transform.position.y, transform.position.z + 100);
            }
        }
    }

    public void HitWall()
    {
        if (!movingToNextLevel)
        {
            dir = Vector2.zero;
            transform.position = new Vector3(currentGrid.transform.position.x, Ypos, currentGrid.transform.position.z);
            gm.PlayerStopped();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!movingToNextLevel)
        {
            creatingLine = gm.IsCreatingLine();
            if(other.transform.TryGetComponent<Enemy>(out Enemy enemy))
            {
                if (currentGrid.state != 1)
                {
                    gm.HitEnemy();
                    blowUpEffect.SetActive(true);
                    GetComponent<MeshRenderer>().enabled = false;
                    Destroy(this);
                }
            }
            if (other.transform.TryGetComponent<Grid>(out Grid grid))
            {
                oldGrid = currentGrid;
                currentGrid = grid;
                canTurn = true;
                gm.ObserveMovement(grid);
            }
            if (other.transform.TryGetComponent<Crystal>(out Crystal crystal))
            {
                crystal.Collect();
            }
            if (!startGridFilled)
            {
                startGridFilled = true;
                gm.PlayerStopped();
            }
        }
        else
        {
            if(other.transform.TryGetComponent<RoadGrid>(out RoadGrid grid))
            {
                if (!cameraController.IsItFollowing())
                {
                    cameraController.StartFollow();
                    cameraController.ResizeCam(16.5f); // cam size increasae on next level
                }
                InstantiateRoadLine(grid);
            }
            if(other.transform.parent.TryGetComponent<GridManager>(out GridManager newGm))
            {
                if (gm != newGm)
                {
                    gm.CloseBackDoor();
                    gm = newGm;
                    oldGrid = null;
                    currentGrid = other.GetComponent<Grid>();
                    canTurn = true;
                    gm.ObserveMovement(other.GetComponent<Grid>());
                    ReachedNextLevel();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.transform.TryGetComponent<Grid>(out Grid grid))
        {
            canTurn = false;
        }
        CheckAutoStop();
    }

    private void CheckAutoStop()
    {
        bool canImoveTo = gm.CanIMoveTo((int)(currentGrid.point.x + dir.x), (int)(currentGrid.point.y + dir.y));
        if (!canImoveTo)
        {
            HitWall();
        }
    }

    private void NextLevel()
    {
        movingToNextLevel = true;
        levelChangeTargetPosition = new Vector3(startXpos,transform.position.y, transform.position.z);
    }

    private void ReachedNextLevel()
    {
        movingToNextLevel = false;
        cameraController.StopFollow();
        cameraController.MoveToGm(gm);
        HitWall();
    }

    private void InstantiateRoadLine(RoadGrid grid)
    {
        GameObject summon = Instantiate(linePrefab);
        summon.transform.position = grid.transform.position;
        summon.GetComponent<Animator>().SetTrigger("fill");
    }
}
