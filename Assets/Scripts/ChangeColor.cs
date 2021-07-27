using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class ChangeColor : MonoBehaviour
{
    [SerializeField]
    private Material material;
   
    public void ChangeMaterial()
    {
        GetComponent<MeshRenderer>().material = material;
    }
}
