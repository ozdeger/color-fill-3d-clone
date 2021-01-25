using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Vector2Int point = new Vector2Int(-10,-10);
    // 0 = empty / 1 = filled / 2 = line
    public int state = 0;
}
