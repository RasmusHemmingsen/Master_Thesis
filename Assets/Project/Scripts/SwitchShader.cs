using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchShader : MonoBehaviour
{
    public Material m_MatHighlightCube;
    public Material m_MatStandardCube;

    public void SwitchToStandardCube(Renderer renderer)
    {
        renderer.material = m_MatStandardCube;
    }

    public void SwitchToHighlightCube(Renderer renderer)
    {
        renderer.material = m_MatHighlightCube;
    }
}
