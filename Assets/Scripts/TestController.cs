using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class TestController : MonoBehaviour
{
    private readonly System.Random rng = new System.Random();

    //x is number of targets, y is visual feedback 1 for feedback 0 for no feedback, z is offset
    [SerializeField]
    List<Vector3> trainingSchedule;

    [SerializeField]
    private Vector3 controllerOffSet = new Vector3(.1f, 0f, 0f);
    [SerializeField]
    private float resetDistance = .2f;
    [SerializeField]
    private float timeBetweenSets = 2f;

    [SerializeField]
    private TargetController targetController;
    [SerializeField]
    private List<int> targetOrder;
    [SerializeField]
    private Transform rightController, leftController, Head;

    public List<VisualFeedback> visuals;

    private Coroutine currentTask;

    void Start()
    {
        currentTask = StartCoroutine(RunPrismTraining());
    }

    IEnumerator RunPrismTraining()
    {
        foreach (Vector3 set in trainingSchedule)
        {
            FillTargetOrder((int)set.x);
            ChangeOffSet(new Vector3(set.z, 0, 0));
            SetVisualFeedback(set.y == 1);
            currentTask = StartCoroutine(RunThroughTargets());
            yield return new WaitWhile(() => { return currentTask != null; });
            StartCoroutine(targetController.SetDone(timeBetweenSets));
            yield return new WaitForSeconds(timeBetweenSets + .1f);
        }
    }

    private void FillTargetOrder(int numberOffTargets)
    {
        targetOrder = new List<int>();
        for(int i = 0; i< numberOffTargets; i++)
        {
            targetOrder.Add(0);
            targetOrder.Add(1);
            targetOrder.Add(2);
        }
        Shuffle(targetOrder);
    }

    public void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    IEnumerator RunThroughTargets()
    {
        foreach (int target in targetOrder)
        {
            targetController.ChangeActiveTarget(target);
            yield return new WaitUntil(TriggerPressed);
            targetController.DisableActiveTarget();
            yield return new WaitForSeconds(.5f);
            yield return new WaitUntil(ControllerReset);
        }
        currentTask = null;
    }

    private bool TriggerPressed()
    {
        bool triggerPressed;
        InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.triggerButton, out var r_Button);
        InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(CommonUsages.triggerButton, out var l_Button);
        triggerPressed = r_Button || l_Button || Input.GetKeyDown(KeyCode.Space);
        return triggerPressed;
    }

    private bool ControllerReset()
    {
        Vector2 headPos2D = new Vector2(Head.position.x, Head.position.z);
        bool leftControllerClose = Vector2.Distance(new Vector2(leftController.position.x, leftController.position.z), headPos2D) < resetDistance;
        bool rightControllerClose = Vector2.Distance(new Vector2(rightController.position.x, rightController.position.z), headPos2D) < resetDistance;
        bool reset = leftControllerClose && rightControllerClose;
        return reset;
    }

    private void ChangeOffSet(Vector3 offSet)
    {
        foreach (VisualFeedback item in visuals)
        {
            item.SetOffset(offSet);
        }
    }

    private void SetVisualFeedback(bool feedBack)
    {
        foreach (VisualFeedback item in visuals)
        {
            item.giveFeedback = feedBack;
        }
    }
}