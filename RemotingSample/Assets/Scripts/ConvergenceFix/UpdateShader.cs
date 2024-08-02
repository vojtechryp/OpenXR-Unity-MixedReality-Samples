using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateShader : MonoBehaviour
{
    [Range(-0.1f, 0.1f)]
    public float offsetAmount = 0.0f;

    public bool DoDisplacement = false;
    // Update is called once per frame
    void Update()
    {
        Shader.SetGlobalVector("_MainCamPosGlobal", transform.position);
        Shader.SetGlobalFloat("_CamOffsetGlobal", offsetAmount);
        Shader.SetGlobalInteger("_DoDisplacementGlobal", DoDisplacement ? 1 : 0);
    }

    private void OnValidate()
    {
        //Shader.SetGlobalVector("_MainCamPos", transform.position);
        //Shader.SetGlobalFloat("_CamOffset", offsetAmount);
        //Shader.SetGlobalInteger("_DoDisplacementGlobal", DoDisplacement ? 1 : 0);
    }
}
