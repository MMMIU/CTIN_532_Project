using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GetMainLightDirection : MonoBehaviour
{
    [SerializeField]
    private Material skyboxMaterial;

    private void Update()
    {
        skyboxMaterial.SetVector("_MainLightDirection", transform.forward);
    }

    private void OnEnable()
    {
        skyboxMaterial.SetVector("_MainLightDirection", transform.forward);
    }

    private void OnDisable()
    {
        skyboxMaterial.SetVector("_MainLightDirection", new Vector3(0, 1, 0));
    }
}
