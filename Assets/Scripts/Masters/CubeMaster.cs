using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMaster : MonoBehaviour
{
    [SerializeField] ComputeShader computeShader;
    [SerializeField] int repetitions;

    RenderTexture renderTexture;

    Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }


    void InitRenderTexture()
    {
        if (renderTexture == null || renderTexture.width != _camera.pixelWidth || renderTexture.height != _camera.pixelHeight)
        {
            if (renderTexture != null)
            {
                renderTexture.Release();
            }
            renderTexture = new RenderTexture(256, 256, 24, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            renderTexture.enableRandomWrite = true;
            renderTexture.Create();
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        InitRenderTexture();

        computeShader.SetTexture(0, "Result", renderTexture);
        computeShader.SetFloat("Resolution", renderTexture.width);

        computeShader.Dispatch(0,renderTexture.width/8, renderTexture.height/8,1);

        Graphics.Blit(renderTexture,destination);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
