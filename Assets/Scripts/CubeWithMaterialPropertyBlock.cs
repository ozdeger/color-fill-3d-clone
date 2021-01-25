using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeWithMaterialPropertyBlock : MonoBehaviour
{
    public Color Color;

    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;

    void Awake()
    {
        _propBlock = new MaterialPropertyBlock();
        _renderer = GetComponent<Renderer>();
    }

    void Update()
    {
        // Get the current value of the material properties in the renderer.
        _renderer.GetPropertyBlock(_propBlock);
        // Assign our new value.
        _propBlock.SetColor("_Color", Color);
        // Apply the edited values to the renderer.
        _renderer.SetPropertyBlock(_propBlock);
    }

    public void UpdateColor()
    {
        _propBlock.SetColor("_Color", Color);
    }
}
