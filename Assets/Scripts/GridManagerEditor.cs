using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridManager))]
public class GridManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        //GridManager gm = (GridManager)target;

        //if (GUILayout.Button("Test Line"))
        //{
        //    gm.TestLine();
        //}
        //if (GUILayout.Button("Clear Level"))
        //{
        //    ld.ClearLevel();
        //}
    }
}