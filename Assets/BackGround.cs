using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackGround : MonoBehaviour
{
    public float yOffset;
    public float backgroundWidth;
    [Range(0,2)]
    public float[] speeds = new float[10];

    Material[] layerMats;
    Transform mainCam;
    Transform trans;
    private float speedConvert;
    
    private void Awake()
    {
        speedConvert = 64 / backgroundWidth;
        mainCam = Camera.main.transform;
        trans = transform;
        layerMats = new Material[speeds.Length];
        Vector2 offset = new Vector2(0, yOffset);
        for(int i = 0; i < 10; i++)
        {
            var ins = transform.GetChild(i);
            layerMats[i] = ins.GetComponent<Image>().material;
            layerMats[i].mainTextureOffset = offset;
        }
    }

    private void LateUpdate()
    {
        trans.position = new Vector3(mainCam.position.x, trans.position.y, trans.position.z);

        for(int i = 0; i < 10; i++)
        {
            layerMats[i].mainTextureOffset = new Vector2(mainCam.position.x * speeds[i] * speedConvert, yOffset);
        }
    }


}
