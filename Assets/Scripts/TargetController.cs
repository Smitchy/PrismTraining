using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    [SerializeField]
    private int activeTarget = 0;
    [SerializeField]
    private List<Target> Targets;

    public void ChangeActiveTarget(int target)
    {
        if (activeTarget > -1) Targets[activeTarget].On = false;
        Targets[target].On = true;
        activeTarget = target;
    }

    public void DisableActiveTarget()
    {
        if (activeTarget < 0) return;
        Targets[activeTarget].On = false;
        activeTarget = -1;
    }

    public IEnumerator SetDone(float time)
    {
        foreach (Target target in Targets)
        {
            target.On = true;
        }
        yield return new WaitForSeconds(time);
        foreach (Target target in Targets)
        {
            target.On = false;
        }
    }
}
