using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalDesigner : MonoBehaviour
{
    public static int X, Y;
    [System.Serializable]
    public class Column
    {
        public bool[] rows = new bool[Y];
    }
    [HideInInspector]
    public Column[] columns = new Column[X];

    public GameObject crystalPrefab;

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

    public void GenerateCrystals()
    {
        Transform crystalPackage = new GameObject().transform;
        crystalPackage.name = "CrystalPackage";
        float cordY = 1;
        float cordX = 0;
        float cordZ = 0;
        foreach (Column column in columns)
        {
            cordZ = 0;
            foreach (bool IsEnemy in column.rows)
            {
                if (IsEnemy)
                {
                    print(new Vector3(cordX, cordY, cordZ));
                    GameObject summon = Instantiate(crystalPrefab);
                    summon.transform.position = new Vector3(cordX, cordY, cordZ);
                    summon.transform.parent = crystalPackage;

                }
                cordZ += 1;
            }
            cordX += 1;
        }
    }
}
