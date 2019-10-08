using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTextureSetup : MonoBehaviour
{
    public Camera m_CameraA;
    public Camera m_CameraB;

    public Material m_CameraMatA;
    public Material m_CameraMatB;


    void Start()
   {
        if (m_CameraA.targetTexture != null)
            m_CameraA.targetTexture.Release();

        m_CameraA.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        m_CameraMatA.mainTexture = m_CameraA.targetTexture;

        if (m_CameraB.targetTexture != null)
            m_CameraB.targetTexture.Release();

        m_CameraB.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        m_CameraMatB.mainTexture = m_CameraB.targetTexture;
    }
}
