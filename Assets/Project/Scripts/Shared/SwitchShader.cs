using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchShader : MonoBehaviour
{
    #region Marterials
    public Material MatHighlightCube;
    public Material MatStandardCube;
    public Material MatHighlightButtonTop;
    public Material MatHighlightButtonStand;
    public Material MatStandardButtonTop;
    public Material MatStandardButtonStand;
    public Material MatStandardHandle;
    public Material MatHighlightHandle;
    public Material MatHighlightTable;
    public Material MatStandardTable;
    #endregion

    #region Cube
    public void SwitchToStandardCube(Renderer cube)
    {
        cube.material = MatStandardCube;
    }

    public void SwitchToHighlightCube(Renderer cube)
    {
        cube.material = MatHighlightCube;
    }

    #endregion

    #region Button
    public void SwitchToStandardButton(Renderer topRenderer, Renderer standRenderer)
    {
        topRenderer.material = MatStandardButtonTop;
        standRenderer.material = MatStandardButtonStand;
    }

    public void SwitchToHighlightButton(Renderer topRenderer, Renderer standRenderer)
    {
        topRenderer.material = MatHighlightButtonTop;
        standRenderer.material = MatHighlightButtonStand;
    }
    #endregion

    #region Handle
    public void SwitchToHighlightHandle(Renderer handle)
    {
        handle.material = MatHighlightHandle;
    } 
    public void SwitchToStandardHandle(Renderer handle)
    {
        handle.material = MatStandardHandle;
    }
    #endregion

    #region Table
    public void SwitchToHighlightTable(Renderer table)
    {
        table.material = MatHighlightTable;
    }
    public void SwitchToStandardTable(Renderer table)
    {
        table.material = MatStandardTable;
    }
    #endregion

}
