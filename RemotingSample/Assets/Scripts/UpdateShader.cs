using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateShader : MonoBehaviour
{
    [Range(-0.1f, 0.1f)]
    public float offsetAmount = 0.0f;
    // Update is called once per frame
    void Update()
    {
        Shader.SetGlobalVector("_MainCamPos", transform.position);
        Shader.SetGlobalFloat("_CamOffset", offsetAmount);
        // offsetMaterial.SetVector("_MainCamPos", transform.position);
    }

    private void OnValidate()
    {
        Shader.SetGlobalVector("_MainCamPos", transform.position);
        Shader.SetGlobalFloat("_CamOffset", offsetAmount);
    }
}
