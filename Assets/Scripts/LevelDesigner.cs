using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;
public class LevelDesigner : MonoBehaviour
{
    [Header("Manage Each Time")]
    public bool startLevel = false;
    public int levelNumber = 1;
    public bool hasNextLevel = false;
    public int paddingSides;
    public int paddingBottom;
    public int roadLength=15;
    public Slider scoreSlider;
    public TMP_Text levelText;
    [Header("Settings")]
    public GameObject player;
    public GameObject gridPartPrefab;
    public GameObject cubePrefab;
    public GameObject fillPrefab;
    public GameObject linePrefab;
    public GameObject doorPrefab;
    public Animator sceneEndAnimator;

    [HideInInspector]
    public float offset = 1; // DEPRECATED - should have default value 1

    [HideInInspector]
    public Transform roadHolder;
    [HideInInspector]
    public Transform[] doors = new Transform[2];
    private List<Collider> colliders = new List<Collider>();
    private Vector2 startPositionXZ;
    private float Ycord;
    private Vector2 endPositionXZ;
    private Transform summon;
    private Transform gridHolder;
    [HideInInspector]
    public List<Transform> levelObjects = new List<Transform>();
    [HideInInspector]
    public List<Transform> gridObjects = new List<Transform>();
    [Header("Level Design")]
    public int X;
    public int Y;
    [System.Serializable]
    public class Column
    {
        public bool[] rows;
    }
    public Column[] columns;

    public void CreateEmptyArray()
    {
        columns = new Column[X];

        for (int i = 0; i < X; i++)
        {
            columns[i] = new Column();
        }

        foreach (Column collumn in columns)
        {
            collumn.rows = new bool[Y];
            for (int i = 0; i < collumn.rows.Length; i++)
            {
                collumn.rows[i] = false;
            }
        }
    }

    public void GenerateLevel()
    {
        transform.name = string.Format("LevelDesigner -Level {0}", levelNumber);

        ClearGrid();

        gridHolder = new GameObject(string.Format("Grid Holder Level {0}",levelNumber)).transform;
        gridHolder.position = Vector3.zero;
        if (startLevel)
        {
            player.GetComponent<PlayerController>().gm = gridHolder.gameObject.AddComponent<GridManager>();
        }
        else
        {
            gridHolder.gameObject.AddComponent<GridManager>();
        }
        GridManager gm = gridHolder.GetComponent<GridManager>();
        gm.linePrefab = linePrefab;
        gm.sceneEndAnimator = sceneEndAnimator;
        gm.hasNextLevel = hasNextLevel;
        gm.CreateEmptyGrid(columns.Length, columns[0].rows.Length);
        if (scoreSlider && levelText)
        {
            gm.scoreSlider = scoreSlider;
            levelText.text = levelNumber.ToString();
        }

        
        ClearLevel();
        Debug.Log("Generating Level");
        Ycord = transform.position.y;
        startPositionXZ = new Vector2(transform.position.x - ((columns.Length/2f)*offset), 0);

        float Xcord = startPositionXZ.x;
        float Zcord = startPositionXZ.y;
        for (int i =0; i< columns.Length; i++)
        {
            Zcord = startPositionXZ.y;
            int counter = 0;
            foreach (bool IsWall in columns[i].rows)
            {
                Zcord++;
                if (IsWall)
                {
                    summon = Instantiate(cubePrefab).transform;
                    summon.transform.position = new Vector3(Xcord, Ycord, Zcord);
                    summon.parent = transform;
                    levelObjects.Add(summon);
                }
                else
                {
                    summon = Instantiate(gridPartPrefab).transform;
                    summon.transform.position = new Vector3(Xcord, Ycord - (offset / 2f), Zcord);
                    summon.parent = gridHolder;
                    summon.name = string.Concat(i.ToString(), "x", counter.ToString());
                    gridObjects.Add(summon);
                }
                counter++;

                endPositionXZ = new Vector2(Xcord, Zcord);
            }
            Xcord++;


        }



        AddPadding(hasNextLevel);
        ClearRoad();
        ClearDoor();
        //CreateColliders();
        MergeMeshes();
        if (hasNextLevel)
        {
            CreateDoor();
            gm.frontDoor = doors[0];
            gm.backDoor = doors[1];
        }
        CreateRoad(hasNextLevel);
        gridHolder.gameObject.GetComponent<GridManager>().SetUpGrid();
        gridHolder.transform.position = new Vector3(transform.position.x,transform.position.y,transform.position.z);
        gridHolder.transform.parent = transform;
    }

    public void AddLevel()
    {
        GameObject summon = Instantiate(gameObject);
        summon.transform.position = new Vector3(transform.position.x,transform.position.y,transform.position.z+roadLength+Y);
        LevelDesigner newLd = summon.GetComponent<LevelDesigner>();
        newLd.levelNumber = levelNumber + 1;
        newLd.paddingBottom = 0;
        newLd.startLevel = false;
        newLd.GenerateLevel();
        newLd.scoreSlider = null;
        newLd.levelText = null;
        hasNextLevel = true;
        GenerateLevel();
    }

    public void AddPadding(bool hasRoad)
    {

        // left padding
        summon = Instantiate(cubePrefab).transform;
        summon.transform.position = new Vector3(startPositionXZ.x - (paddingSides * offset / 2f) - (.5F * offset), Ycord, ((startPositionXZ.y + endPositionXZ.y) / 2f) + (.5F * offset) - (paddingBottom*.5f*offset));
        summon.transform.localScale = new Vector3(summon.transform.localScale.x * paddingSides * offset, summon.transform.localScale.y, summon.transform.localScale.z * ((endPositionXZ.y- startPositionXZ.y) + (paddingBottom  *offset)));
        summon.parent = transform;
        levelObjects.Add(summon);
        // right padding
        summon = Instantiate(cubePrefab).transform;
        summon.transform.position = new Vector3(endPositionXZ.x + (paddingSides * offset / 2f) + (.5F * offset), Ycord, ((startPositionXZ.y + endPositionXZ.y) / 2f) + (.5F * offset) - (paddingBottom * .5f * offset));
        summon.transform.localScale = new Vector3(summon.transform.localScale.x * paddingSides * offset, summon.transform.localScale.y, summon.transform.localScale.z * ((endPositionXZ.y - startPositionXZ.y) + (paddingBottom * offset)));
        summon.parent = transform;
        levelObjects.Add(summon);
        // top padding-left
        summon = Instantiate(cubePrefab).transform;
        summon.transform.position = new Vector3((startPositionXZ.x + endPositionXZ.x) / 2f - ((columns.Length - 1) / 4f * offset) - (.5f * offset)-(paddingSides*.5f*offset), Ycord, endPositionXZ.y + (.5f * offset) + (roadLength * offset / 2f));
        summon.transform.localScale = new Vector3((((columns.Length - 1) / 2f)+paddingSides) * summon.transform.localScale.x * offset, summon.transform.localScale.y, summon.transform.localScale.z * roadLength * offset);
        summon.parent = transform;
        levelObjects.Add(summon);

        // top padding-right
        summon = Instantiate(cubePrefab).transform;
        summon.transform.position = new Vector3((startPositionXZ.x + endPositionXZ.x) / 2f + ((columns.Length - 1) / 4f*offset)+(.5f*offset) + (paddingSides * .5f * offset), Ycord, endPositionXZ.y + (.5f * offset) + (roadLength * offset / 2f));
        summon.transform.localScale = new Vector3((((columns.Length-1)/2f)+paddingSides) * summon.transform.localScale.x * offset, summon.transform.localScale.y, summon.transform.localScale.z * roadLength * offset);
        summon.parent = transform;
        levelObjects.Add(summon);

        // top padding-middle
        if (!hasRoad)
        {
            summon = Instantiate(cubePrefab).transform;
            summon.transform.position = new Vector3((startPositionXZ.x + endPositionXZ.x) / 2f, Ycord, endPositionXZ.y + (.5f * offset) + (roadLength * offset / 2f));
            summon.transform.localScale = new Vector3(summon.transform.localScale.x * offset, summon.transform.localScale.y, summon.transform.localScale.z * roadLength * offset);
            summon.parent = transform;
            levelObjects.Add(summon);
        }
        //bottom padding
        if (paddingBottom > 0)
        {
            summon = Instantiate(cubePrefab).transform;
            summon.transform.position = new Vector3((startPositionXZ.x + endPositionXZ.x) / 2f, Ycord, startPositionXZ.y + (.5f * offset) - (paddingBottom * offset / 2f));
            summon.transform.localScale = new Vector3(columns.Length * summon.transform.localScale.x * offset, summon.transform.localScale.y, summon.transform.localScale.z * paddingBottom * offset);
            summon.parent = transform;
            levelObjects.Add(summon);
        }
    }

    public void CreateRoad(bool createRoad)
    {
        if (createRoad) { 
            roadHolder = new GameObject().transform;
            roadHolder.name = string.Format("RoadHolder -Level {0}", levelNumber);
            roadHolder.transform.position = new Vector3(transform.position.x - (.5f * offset), Ycord, endPositionXZ.y + offset);
            roadHolder.transform.parent = transform;
            float cordZ = endPositionXZ.y + offset+transform.position.z;
            for (int i = 0; i < roadLength; i++)
            {
                GameObject summon = Instantiate(gridPartPrefab);
                summon.name = string.Format("road {0}", i);
                DestroyImmediate(summon.GetComponent<Grid>());
                summon.AddComponent<RoadGrid>();
                summon.transform.position = new Vector3(transform.position.x - (.5f * offset), Ycord - (offset / 2f), cordZ);
                summon.transform.parent = roadHolder;
                cordZ += offset;
            }
        }
    }

    public void CreateDoor()
    {
        //first door
        doors[0] = Instantiate(doorPrefab).transform;
        doors[0].transform.position = new Vector3(transform.position.x - (.5f * offset), Ycord, endPositionXZ.y + (.699f * offset)+transform.position.z);
        doors[0].parent = transform;
        // second door
        doors[1] = Instantiate(doorPrefab).transform;
        doors[1].transform.position = new Vector3(transform.position.x - (.5f * offset), Ycord, endPositionXZ.y + (.3f * offset) + transform.position.z+(roadLength*offset));
        doors[1].parent = transform;
    }

    public void ClearDoor()
    {
        if (doors[0])
        {
            DestroyImmediate(doors[0].gameObject);
            doors[0] = null;
        }
        if (doors[1])
        {
            DestroyImmediate(doors[1].gameObject);
            doors[1] = null;
        }
    }

    public void ClearRoad()
    {
        if (roadHolder)
        {
            DestroyImmediate(roadHolder.gameObject);
            roadHolder = null;
        }
    }

    public void MergeMeshes()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;

            i++;
        }

        Mesh newMesh = new Mesh();
        newMesh.CombineMeshes(combine);
        transform.GetComponent<MeshFilter>().sharedMesh = newMesh;

        ClearObjects();
    }

    public void ClearObjects()
    {
        foreach (Transform transform in levelObjects)
        {
            if (transform != null)
            {
                DestroyImmediate(transform.gameObject);
            }
        }
        levelObjects.Clear();
    }

    public void ClearLevel()
    {
        GetComponent<MeshFilter>().mesh = new Mesh();
    }

    public void ClearGrid()
    {
        Transform gridHolder =null;
        if (gridObjects[0])
        {
            gridHolder = gridObjects[0].parent;
        }
        foreach (Transform transform in gridObjects)
        {
            if (transform != null)
            {
                DestroyImmediate(transform.gameObject);
            }
        }
        if (gridHolder)
        {
            DestroyImmediate(gridHolder.gameObject);
        }
        gridObjects.Clear();
    }

    //public void CreateColliders()
    //{ 
    //    foreach(Collider coll in colliders)
    //    {
    //        DestroyImmediate(coll);
    //    }
    //    gridHolder.gameObject.AddComponent<WallTag>();
    //    foreach (Transform cube in levelObjects)
    //    {
    //        BoxCollider cubeCollider = cube.GetComponent<BoxCollider>();
    //        BoxCollider newCollider = gridHolder.gameObject.AddComponent<BoxCollider>();
    //        newCollider.size = cubeCollider.size;
    //        newCollider.center =(cube.transform.position - gridHolder.transform.position);
    //        newCollider.isTrigger = true;
    //        colliders.Add(newCollider);
    //    }
    //}


}
