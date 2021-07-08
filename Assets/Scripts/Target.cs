using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField]
    private bool editorOn;
    [SerializeField]
    private GameObject onObject, offObject;

    private bool m_On;
    public bool On
    {
        get { return m_On; }
        set
        {
            if (m_On == value) return;
            m_On = value;
            OnValueChanged();
        }
    }

    private void OnValueChanged()
    {
        onObject.SetActive(m_On);
        offObject.SetActive(!m_On);
    }

    private void Update()
    {
        //if(editorOn != m_On)
        //{
        //    m_On = editorOn;
        //    OnValueChanged();
        //}
        
    }

}
