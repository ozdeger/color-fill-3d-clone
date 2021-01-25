using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class GridManager : MonoBehaviour
{
    [System.Serializable]
    public class Array
    {
        public int[] intArray;
    }

    [System.Serializable]
    public class Area
    {
        public List<Vector2Int> pointArray = new List<Vector2Int>();
    }

    [System.Serializable]
    public class Transforms
    {
        public Transform[] transforms;
    }
    [HideInInspector]
    public Animator sceneEndAnimator;
    [HideInInspector]
    public bool hasNextLevel;
    [HideInInspector]
    public Transforms[] gridObjects;
    [HideInInspector]
    public Array[] grid;
    private Array[] resolveGrid;
    [HideInInspector]
    public GameObject linePrefab;
    [HideInInspector]
    public Slider scoreSlider;
    [HideInInspector]
    public Transform backDoor;
    [HideInInspector]
    public Transform frontDoor;
    private int gridCount;
    private int filledGridCount = 0;
    private List<Transform> lineObjects = new List<Transform>();
    private List<Vector2Int> onGoingLine = new List<Vector2Int>();
    private List<Area> areaList = new List<Area>();
    [HideInInspector]
    public Vector2Int[] testLine; // DEPRECATED

    private bool freezePlayer;
    private bool creatingLine = false;
    private float lineDeathAnimationTime = 2f;

    public void Awake()
    {
        gridCount = transform.childCount;
        if (scoreSlider)
        {
            scoreSlider.maxValue = (int)(gridCount * 88f / 100f);
            scoreSlider.value = 0;
        }
    }

    private void OpenFrontDoor()
    {
        frontDoor.GetComponent<Animator>().SetTrigger("open");
    }

    public void CloseBackDoor()
    {
        backDoor.GetComponent<Animator>().SetTrigger("close");
    }

    public void UpdateUI()
    {
        if (scoreSlider)
        {
            scoreSlider.value = filledGridCount;
        }
    }

    // 0 = empty ,2 = filled , 1 = wall , 3 = line

    public void CreateEmptyGrid(int x, int y)
    {
        grid = new Array[x];

        for(int i = 0; i< x; i++)
        {
            grid[i] = new Array();
        }

        foreach(Array collumn in grid)
        {
            collumn.intArray = new int[y];
            for(int i = 0; i < collumn.intArray.Length; i++)
            {
                collumn.intArray[i] = 1;
            }
        }
    }

    public void CreateEmptyGridObjects(int x, int y)
    {
        gridObjects = new Transforms[x];

        for (int i = 0; i < x; i++)
        {
            gridObjects[i] = new Transforms();
        }

        foreach (Transforms collumn in gridObjects)
        {
            collumn.transforms = new Transform[y];
            for (int i = 0; i < collumn.transforms.Length; i++)
            {
                collumn.transforms[i] = null;
            }
        }
    }

    public Array[] CreateResolveGrid()
    {
        int x = grid.Length;
        int y = grid[0].intArray.Length;
        Array[] newGrid = new Array[x];

        for (int i = 0; i < x; i++)
        {
            newGrid[i] = new Array();
        }

        for(int j=0;j<x;j++)
        {
            newGrid[j].intArray = new int[y];
            for (int i = 0; i < y; i++)
            {
                newGrid[j].intArray[i] = grid[j].intArray[i];
            }
        }
        return newGrid;
    }

    public void SetUpGrid()
    {
        CreateEmptyGridObjects(grid.Length, grid[0].intArray.Length);
        foreach (Transform child in transform)
        {
            string[] indexStrings = child.name.Split('x');
            int x = int.Parse(indexStrings[0]);
            int y = int.Parse(indexStrings[1]);
            UpdateGrid(x, y, 0);
            child.gameObject.AddComponent<Grid>().point = new Vector2Int(x, y);
            gridObjects[x].transforms[y] = child;
        }
    }

    public void UpdateGrid(int x, int y,int value)
    {
        grid[x].intArray[y] = value;
    }

    public void ResolveLine()
    {
        areaList.Clear();
        resolveGrid = CreateResolveGrid();

        foreach (Vector2Int point in onGoingLine)
        {
            //right
            if (point.x + 1 < resolveGrid.Length)
            {
                if (GetResolvePoint(point.x + 1, point.y) == 0)
                {
                    Area area = Plague(new Area(),point.x+1, point.y);
                    areaList.Add(area);
                }
            }
            //left
            if (point.x - 1 >= 0)
            {
                if (GetResolvePoint(point.x - 1, point.y) == 0)
                {
                    Area area = Plague(new Area(),point.x-1,point.y);
                    areaList.Add(area);
                }
            }
            //up
            if (point.y + 1 < resolveGrid[0].intArray.Length)
            {
                if (GetResolvePoint(point.x, point.y + 1) == 0)
                {
                    Area area = Plague(new Area(),point.x,point.y+1);
                    areaList.Add(area);
                }
            }
            //down
            if (point.y - 1 >= 0)
            {
                if (GetResolvePoint(point.x, point.y - 1) == 0)
                {
                    Area area = Plague(new Area(),point.x,point.y-1);
                    areaList.Add(area);
                }
            }
        }

        // Area Fills
        bool repeat = true;
        Area biggestArea = null;
        int biggestAreaSize = 0;
        while (repeat)
        {
            repeat = false;
            biggestArea = null;
            biggestAreaSize = 0;

            foreach (Area area in areaList)
            {
                if (area.pointArray.Count > biggestAreaSize)
                {
                    biggestArea = area;
                    biggestAreaSize = area.pointArray.Count;
                }
            }
            resolveGrid = CreateResolveGrid();
            if (biggestArea !=null)
            {
                foreach (Vector2Int point in biggestArea.pointArray)
                {
                    if (resolveGrid[point.x].intArray[point.y] == 1)
                    {
                        resolveGrid[point.x].intArray[point.y] = 0;
                    }
                }
                if (ForceAreaCheck(biggestArea))
                {
                    FillArea(biggestArea);
                    areaList.Remove(biggestArea);
                    repeat = true;
                    continue;
                }
                if (biggestAreaSize> gridCount*12f/100f)
                {
                    areaList.Remove(biggestArea);
                    break;
                }

            }
        }
        foreach(Area area in areaList)
        {
            FillArea(area);
        }
        
        foreach(Transform line in lineObjects)
        {
            line.GetComponent<Animator>().SetTrigger("fill");
        }

        foreach(Vector2Int point in onGoingLine)
        {
            UpdateGrid(point.x,point.y,2);
            gridObjects[point.x].transforms[point.y].GetComponent<Grid>().state = 1;
            filledGridCount++;
        }
        UpdateUI();
    }

    private Area Plague(Area area, int startX, int startY,bool closed = true)
    {
        Vector2Int point;
        if (area.pointArray.Count == 0)
        {
            point = new Vector2Int(startX,startY);
            area.pointArray.Add(point);
            UpdateResolveGrid(point.x, point.y, 1);
        }
        else
        {
             point = area.pointArray[area.pointArray.Count - 1];
        }
        //right
        if (point.x + 1 < resolveGrid.Length)
        {
            if (GetResolvePoint(point.x + 1, point.y) == 0)
            {
                UpdateResolveGrid(point.x+1, point.y, 1);
                area.pointArray.Add(new Vector2Int(point.x + 1, point.y));
                Plague(area,0,0,closed);
            }
        }
        //left
        if (point.x - 1 >= 0)
        {
            if (GetResolvePoint(point.x - 1, point.y) == 0)
            {
                UpdateResolveGrid(point.x-1, point.y, 1);
                area.pointArray.Add(new Vector2Int(point.x - 1, point.y));
                Plague(area,0,0,closed);
            }
        }
        //up
        if (point.y + 1 < resolveGrid[0].intArray.Length)
        {
            if (GetResolvePoint(point.x, point.y + 1) == 0)
            {
                UpdateResolveGrid(point.x, point.y+1, 1);
                area.pointArray.Add(new Vector2Int(point.x, point.y+1));
                Plague(area,0,0,closed);
            }
        }
        //down
        if (point.y - 1 >= 0)
        {
            if (GetResolvePoint(point.x, point.y - 1) == 0)
            {
                UpdateResolveGrid(point.x, point.y-1, 1);
                area.pointArray.Add(new Vector2Int(point.x, point.y-1));
                Plague(area,0,0,closed);
            }
        }
        return area;
    }

    public bool ForceAreaCheck(Area area)
    {
        if(area == null)
        {
            return false;
        }

        foreach(Vector2Int point in area.pointArray)
        {
            //right
            if (point.x + 1 >= resolveGrid.Length)
            {
                return false;
            }

            //left
            if (point.x - 1 < 0)
            {
                return false;
            }

            //up
            if (point.y + 1 >= resolveGrid[0].intArray.Length)
            {
                return false;
            }

            //down
            if (point.y - 1 < 0)
            {
                return false;
            }

        }
        return true;
    }

    public void TestLine()
    {
        onGoingLine =(List<Vector2Int>)testLine.Clone();
        foreach (Vector2Int point in onGoingLine)
        {
            UpdateGrid(point.x, point.y, 3);
        }
        ResolveLine();
    }

    public int GetGridPoint(int x, int y)
    {
        return grid[x].intArray[y];
    }

    public int GetResolvePoint(int x, int y)
    {
        return resolveGrid[x].intArray[y];
    }

    public void UpdateResolveGrid(int x, int y, int value)
    {
        resolveGrid[x].intArray[y] = value;
    }
    // 0 = false | 1 = true | 2 = reject movement
    public bool CanIMoveTo(int x,int y)
    {
        if (freezePlayer)
        {
            return false;
        }
        
        if (x >= 0 && y >= 0)
        {
            if (x < grid.Length && y < grid[0].intArray.Length)
            {
                if (GetGridPoint(x,y)==1)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        return false;
    }



    public void ObserveMovement(Grid _grid)
    {
        if (!freezePlayer)
        {
            Vector2Int point = new Vector2Int(_grid.point.x, _grid.point.y);
            if (!creatingLine)
            {
                if (GetGridPoint(point.x, point.y) == 0)
                {
                    creatingLine = true;
                    onGoingLine.Add(point);
                    UpdateGrid(point.x, point.y, 3);
                    InstantiateLine(_grid);
                }
            }
            else
            {
                if (GetGridPoint(point.x, point.y) == 0)
                {
                    onGoingLine.Add(point);
                    UpdateGrid(point.x, point.y, 3);
                    InstantiateLine(_grid);
                }
                else if (GetGridPoint(point.x, point.y) == 3)
                {
                    freezePlayer = true;
                    LineDied(point);
                }
                else
                {
                    LineEnded();
                    creatingLine = false;
                }
            }
        }
    }

    private void InstantiateLine(Grid grid)
    {
        GameObject summon = Instantiate(linePrefab);
        summon.transform.position = grid.transform.position;
        grid.state = 2;
        lineObjects.Add(summon.transform);
    }

    public void PlayerStopped()
    {
        if (!freezePlayer)
        {
            LineEnded();
        }
    }

    private void LineEnded()
    {
        ResolveLine();
        onGoingLine.Clear();
        lineObjects.Clear();
        creatingLine = false;
    }

    private void FillArea(Area area)
    {
        foreach(Vector2Int point in area.pointArray)
        {
            GameObject summon = Instantiate(linePrefab);
            summon.transform.position = gridObjects[point.x].transforms[point.y].position;
            summon.GetComponent<Animator>().SetTrigger("fill");
            UpdateGrid(point.x, point.y, 2);
            gridObjects[point.x].transforms[point.y].GetComponent<Grid>().state = 1;
            filledGridCount++;
            
        }
    }

    public bool IsLevelCleared()
    {
        if (filledGridCount >= gridCount)
        {
            if (hasNextLevel)
            {
                backDoor.GetComponent<Animator>().SetTrigger("open");
                OpenFrontDoor();
                return true;
            }
            else
            {
                sceneEndAnimator.SetTrigger("close");
                Invoke("NextScene", .2f);
            }
        }
        return false;
    }

    private void NextScene()
    {
        EnemyHolderStatic.enemies.Clear();
        if(SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings)
        { 
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            Debug.Log("Last Level Reached");
        }
    }

    private void Died()
    {
        EnemyHolderStatic.enemies.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LineDied(Vector2Int point)
    {
        StartCoroutine(LineDiedRoutine(point));
    }

    private IEnumerator LineDiedRoutine(Vector2Int point)
    {
        List<Transform> linePreHit = new List<Transform>();
        Transform hitObj;
        Vector2Int dir1 = new Vector2Int(1, 0);
        Vector2Int dir2 = new Vector2Int(1, 0);
        float stepTime = lineDeathAnimationTime / ((float)lineObjects.Count);
        int counter = 0;
        while (true)
        {
            if (onGoingLine[counter] == point)
            {
                hitObj = lineObjects[0];
                lineObjects.RemoveAt(0);
                Destroy(hitObj.gameObject);
                break;
            }
            else
            {
                linePreHit.Insert(0, lineObjects[0]);
                lineObjects.RemoveAt(0);
            }
            counter++;
        }

        while (lineObjects.Count > 1)
        {
            Vector3 dirFloat = (lineObjects[1].position - lineObjects[0].position);
            dir1 = new Vector2Int((int)dirFloat.x, (int)dirFloat.z);
            lineObjects[0].gameObject.AddComponent<SwipeDestroyer>().SwipeDestroy(dir1, stepTime);
            lineObjects.RemoveAt(0);
            if (linePreHit.Count > 1)
            {
                dirFloat = (linePreHit[1].position - linePreHit[0].position);
                dir2 = new Vector2Int((int)dirFloat.x, (int)dirFloat.z);
                linePreHit[0].gameObject.AddComponent<SwipeDestroyer>().SwipeDestroy(dir2, stepTime);
                linePreHit.RemoveAt(0);
            }
            else if (linePreHit.Count > 0)
            {
                linePreHit[0].gameObject.AddComponent<SwipeDestroyer>().SwipeDestroy(dir2, stepTime);
                linePreHit.RemoveAt(0);
            }
            yield return new WaitForSeconds(stepTime);
        }
        if (lineObjects.Count > 0)
        {
            lineObjects[0].gameObject.AddComponent<SwipeDestroyer>().SwipeDestroy(dir1, stepTime);
            lineObjects.RemoveAt(0);
        }

        while (linePreHit.Count > 1)
        {
            Vector3 dirFloat = (linePreHit[1].position - linePreHit[0].position);
            dir2 = new Vector2Int((int)dirFloat.x, (int)dirFloat.z);
            linePreHit[0].gameObject.AddComponent<SwipeDestroyer>().SwipeDestroy(dir2, stepTime);
            linePreHit.RemoveAt(0);
            yield return new WaitForSeconds(stepTime);
        }
        if (linePreHit.Count > 0)
        {
            linePreHit[0].gameObject.AddComponent<SwipeDestroyer>().SwipeDestroy(dir2, stepTime);
            linePreHit.RemoveAt(0);
        }
        Invoke("Died", .5f);
    }

    public bool IsCreatingLine()
    {
        if (creatingLine)
        {
            return true;
        }
        return false;
    }

    public void HitEnemy()
    {
        freezePlayer = true;
        FreezeEnemies();
        Invoke("Died", 1.6f);
    }

    public void EnemyHitLine(Vector2Int point)
    {
        if (!freezePlayer)
        {
            LineDied(onGoingLine[0]);
            FreezeEnemies();
        }
        freezePlayer = true;
    }

    private void FreezeEnemies()
    {
        for (int i = 0; i < EnemyHolderStatic.enemies.Count; i++)
        {
            if (EnemyHolderStatic.enemies[i])
            {
                Destroy(EnemyHolderStatic.enemies[i]);
            }
        }
    }
}
