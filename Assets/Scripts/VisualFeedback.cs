using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class VisualFeedback : MonoBehaviour
{
    public bool giveFeedback = true;

    [SerializeField]
    private GameObject model;

    [SerializeField]
    private float activeTime = .5f;

    private bool invisible = true;

    private void Start()
    {
        GameObject.Find("TestManager").GetComponent<TestController>().visuals.Add(this);
    }

    void Update()
    {
        if (!giveFeedback)
            return;

        InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.triggerButton, out var l_Button);
        InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.triggerButton, out var r_Button);

        if ((r_Button || l_Button || Input.GetKeyDown(KeyCode.T)) && invisible)
        {
            StartCoroutine(TempActive());
        }
    }

    public void SetVisibility(bool isVisible)
    {
        model.SetActive(isVisible);
    }

    IEnumerator TempActive()
    {
        invisible = false;
        model.SetActive(!invisible);
        yield return new WaitForSeconds(activeTime);
        invisible = true;
        model.SetActive(!invisible);
    }

    //offSet should only be on the x axis but the option is there.
    public void SetOffset(Vector3 offSet)
    {
        model.transform.localPosition = offSet;
    }
}
