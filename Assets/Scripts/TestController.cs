using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class TestController : MonoBehaviour
{
    private readonly System.Random rng = new System.Random();

    //x is number of targets, y is visual feedback 1 for feedback 0 for no feedback, z is offset
    [SerializeField]
    List<Vector3> standardTrainingSchedule;
    [SerializeField]
    List<Vector3> mondayTrainingSchedule;
    [SerializeField]
    List<Vector3> demoTrainingSchedule;

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

    private Coroutine trainingSession = null;

    private void Update()
    {
        InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.primaryButton, out var B_Button);
        if (B_Button)
        {
            StopCurrentSession();
            SetVisuals(true);
            ChangeOffSet(Vector3.zero);
            SetVisualFeedback(false);
        }
    }

    #region Public Functions
    public void RunDemoTraining()
    {
        StopCurrentSession();
        SetVisuals(false);
        trainingSession = StartCoroutine(RunPrismTraining(demoTrainingSchedule));
    }

    public void RunStandardTraining()
    {
        StopCurrentSession();
        SetVisuals(false);
        trainingSession = StartCoroutine(RunPrismTraining(standardTrainingSchedule));
    }

    public void RunMondayTraining()
    {
        StopCurrentSession();
        SetVisuals(false);
        trainingSession = StartCoroutine(RunPrismTraining(mondayTrainingSchedule));
    }

    #endregion

    #region Private Functions

    private void StopCurrentSession()
    {
        if (trainingSession != null)
        {
            StopCoroutine(trainingSession);
            trainingSession = null;
        }
        if(currentTask != null)
        {
            StopCoroutine(currentTask);
            currentTask = null;
        }
    }

    private void SetVisuals(bool isVisible)
    {
        foreach (VisualFeedback visual in visuals)
        {
            visual.SetVisibility(isVisible);
        }
        rightController.GetComponent<XRRayInteractor>().enabled = isVisible;
        leftController.GetComponent<XRRayInteractor>().enabled = isVisible;
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

    private void Shuffle<T>(IList<T> list)
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
    #endregion

    #region Corutines

    IEnumerator RunPrismTraining(List<Vector3> trainingProgram)
    {
        foreach (Vector3 set in trainingProgram)
        {
            FillTargetOrder((int)set.x);
            ChangeOffSet(new Vector3(set.z, 0, 0));
            SetVisualFeedback(set.y == 1);
            currentTask = StartCoroutine(RunThroughTargets());
            yield return new WaitWhile(() => { return currentTask != null; });
            StartCoroutine(targetController.SetDone(timeBetweenSets));
            yield return new WaitForSeconds(timeBetweenSets + .1f);
        }
        trainingSession = null;
        SetVisuals(true);
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
    #endregion
}
