using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchShader : MonoBehaviour
{
    public Material m_MatHighlightCube;
    public Material m_MatStandardCube;
    public Material m_MatHighlightButtonTop;
    public Material m_MatHighlightButtonStand;
    public Material m_MatStandardButtonTop;
    public Material m_MatStandardButtonStand;


    public void SwitchToStandardCube(Renderer renderer)
    {
        renderer.material = m_MatStandardCube;
    }

    public void SwitchToHighlightCube(Renderer renderer)
    {
        renderer.material = m_MatHighlightCube;
    }

    public void SwitchToStandardButton(Renderer topRenderer, Renderer standRenderer)
    {
        topRenderer.material = m_MatStandardButtonTop;
        standRenderer.material = m_MatStandardButtonStand;
    }

    public void SwitchToHighlihtButton(Renderer topRenderer, Renderer standRenderer)
    {
        topRenderer.material = m_MatHighlightButtonTop;
        standRenderer.material = m_MatHighlightButtonStand;
    }
}
