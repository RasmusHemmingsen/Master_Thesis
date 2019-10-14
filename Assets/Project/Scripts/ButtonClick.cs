using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonClick : MonoBehaviour
{
    public Transform m_UpperPoint;
    public Transform m_LowerPoint;

    public UnityEvent m_ButtonPushed;

    // Update is called once per frame
    void Update()
    {
        if (transform.position == m_LowerPoint.position)
            m_ButtonPushed.Invoke();
        

    }
}
