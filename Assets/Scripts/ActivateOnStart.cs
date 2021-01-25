using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateOnStart : MonoBehaviour
{
    public MonoBehaviour script;
    private void Start()
    {
        script.enabled = true;
    }
}
