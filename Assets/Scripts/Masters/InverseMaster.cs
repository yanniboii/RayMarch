using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseMaster : MonoBehaviour
{
    [SerializeField] ComputeShader m_ComputeShader;
    private RenderTexture renderTexture;
    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
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
            renderTexture = new RenderTexture(_camera.pixelWidth, _camera.pixelHeight, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            renderTexture.enableRandomWrite = true;
            renderTexture.Create();
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        InitRenderTexture();
        m_ComputeShader.SetTexture(0, "Result", renderTexture);
        m_ComputeShader.SetTexture(0,"Source",source);

        int threadGroupsX = Mathf.CeilToInt(_camera.pixelWidth / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(_camera.pixelHeight / 8.0f);
        m_ComputeShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);
        Graphics.Blit(renderTexture, destination);

    }
}
