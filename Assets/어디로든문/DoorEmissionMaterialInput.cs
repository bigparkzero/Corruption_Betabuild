using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorEmissionMaterialInput : MonoBehaviour
{
    public Interaction_Gimmick interaction_Gimmick;

    public Renderer targetRenderer;
    public Material[] materials;

    public float smoothtime;
    private void Start()
    {
        materials = targetRenderer.materials;
    }

    void Update()
    {
        if (interaction_Gimmick == null)
            return;
        if (materials[1].GetFloat("_Fill") >= 100)
        {
            materials[1].SetFloat("_Fill", 100);
        }
        else
        {
            materials[1].SetFloat("_Fill", Mathf.Lerp(interaction_Gimmick.currentGaugeValue, materials[1].GetFloat("_Fill"), smoothtime * Time.deltaTime));
        }
    }
}
