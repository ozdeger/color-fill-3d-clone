using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDesigner : MonoBehaviour
{
    public static int X, Y;
    [System.Serializable]
    public class Column
    {
        public bool[] rows = new bool[Y];
    }
    [HideInInspector]
    public Column[] columns = new Column[X];

    public GameObject enemyPrefab;

    public Vector2Int dir;
    public int maxDistance;

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
                collumn.rows[i] = true;
            }
        }
    }

    public void GenerateEnemy()
    {
        Transform enemyPackage = new GameObject().transform;
        enemyPackage.name = "EnemyPackage";
        float cordY = 0;
        float cordX = 0;
        float cordZ = 0;
        foreach(Column column in columns)
        {
            cordX = 0;
            foreach(bool IsEnemy in column.rows)
            {
                if (IsEnemy)
                {
                    print(new Vector3(cordX, cordY, cordZ));
                    GameObject summon = Instantiate(enemyPrefab);
                    summon.transform.position = new Vector3(cordX, cordY, cordZ);
                    summon.transform.parent = enemyPackage;
                    summon.GetComponent<Enemy>().upMax = maxDistance;
                    summon.GetComponent<Enemy>().dir = dir;
                    
                }
                cordX += 1;
            }
            cordZ += 1;
        }
    }
}
