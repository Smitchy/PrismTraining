using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SimClick : MonoBehaviour
{
    private Button btn;
    private void Start()
    {
        btn = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            btn.onClick.Invoke();
    }
}
