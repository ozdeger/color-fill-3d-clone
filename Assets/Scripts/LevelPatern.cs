using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPatern : MonoBehaviour
{
    public static int X, Y;
    [System.Serializable]
    public class Column
    {
        public bool[] rows = new bool[Y];
    }
    [HideInInspector]
    public Column[] columns = new Column[X];

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
}
