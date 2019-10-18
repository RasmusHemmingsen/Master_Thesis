using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchShader : MonoBehaviour
{
    #region Marterials
    public Material m_MatHighlightCube;
    public Material m_MatStandardCube;
    public Material m_MatHighlightButtonTop;
    public Material m_MatHighlightButtonStand;
    public Material m_MatStandardButtonTop;
    public Material m_MatStandardButtonStand;
    public Material m_MatStandardHandle;
    public Material m_MatHighlightHandle;
    public Material m_MatHighlightTable;
    public Material m_MatStandardTable;
    #endregion

    #region Cube
    public void SwitchToStandardCube(Renderer renderer)
    {
        renderer.material = m_MatStandardCube;
    }

    public void SwitchToHighlightCube(Renderer renderer)
    {
        renderer.material = m_MatHighlightCube;
    }

    #endregion

    #region Button
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
    #endregion

    #region Handle
    public void SwitchToHighlightHandle(Renderer renderer)
    {
        renderer.material = m_MatHighlightHandle;
    } 
    public void SwitchToStandardHandle(Renderer renderer)
    {
        renderer.material = m_MatStandardHandle;
    }
    #endregion

    #region Table
    public void SwitchToHighlightTable(Renderer renderer)
    {
        renderer.material = m_MatHighlightTable;
    }
    public void SwitchToStandardTable(Renderer renderer)
    {
        renderer.material = m_MatStandardTable;
    }
    #endregion

}
