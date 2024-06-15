using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using static Unity.VisualScripting.Member;
using static UnityEngine.GraphicsBuffer;

public class RayMarchingMaster : MonoBehaviour
{
    [SerializeField] ComputeShader m_ComputeShader;

    [Range(-0.5f,0.5f)]
    [SerializeField] float smoothing;
    [SerializeField] float ambientIntensity;
    [SerializeField] float diffuseIntensity;
    [SerializeField] float specularIntensity;
    [SerializeField] bool hasLight;
    [SerializeField] bool showDepth;

    float speed = 0.001f;

    private RenderTexture renderTexture;
    private RenderTexture depthTexture;
    private Camera _camera;

    List<ComputeBuffer> deleteComputeBuffers = new List<ComputeBuffer>();

    Sphere[] spheres;
    Cube[] cubes;
    Triangle[] triangles;

    private void Awake()
    {
        spheres = GenerateRandomSpheres();
        cubes = GenerateRandomCubes();
        triangles = GenerateRandomTriangles();
        _camera = GetComponent<Camera>();
        _camera.depthTextureMode = DepthTextureMode.Depth;
    }

    // Start is called before the first frame update
    void Start()
    {
        depthTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
        depthTexture.enableRandomWrite = true;
        depthTexture.Create();
    }

    void Init()
    {
        _camera = GetComponent<Camera>();
    }

    public void SliderValue(float value)
    {
        smoothing = value;
    }

    void CreateScene()
    {
        spheres[0].position = new Vector3(0, 3, 0);
        spheres[0].radius = 1;
        spheres[0].color = new Vector3(1, 0, 0);

        ComputeBuffer sphereBuffer = new ComputeBuffer(spheres.Length, Sphere.GetSize());
            sphereBuffer.SetData(spheres);
            m_ComputeShader.SetInt("_NumSpheres", spheres.Length);
            m_ComputeShader.SetBuffer(0, "Spheres", sphereBuffer);
            deleteComputeBuffers.Add(sphereBuffer);
        



        ComputeBuffer cubeBuffer = new ComputeBuffer(cubes.Length, Cube.GetSize());
        cubeBuffer.SetData(cubes);
        m_ComputeShader.SetInt("_NumCubes", cubes.Length);
        m_ComputeShader.SetBuffer(0, "Cubes", cubeBuffer);
        deleteComputeBuffers.Add(cubeBuffer);

        ComputeBuffer trianlgeBuffer = new ComputeBuffer(triangles.Length, Triangle.GetSize());
        trianlgeBuffer.SetData(triangles);
        m_ComputeShader.SetInt("_NumTriangles", triangles.Length);
        m_ComputeShader.SetBuffer(0, "Triangles", trianlgeBuffer);
        deleteComputeBuffers.Add(trianlgeBuffer);


        m_ComputeShader.SetBool("hasLight", hasLight);
        m_ComputeShader.SetBool("showDepth", showDepth);


        m_ComputeShader.SetFloat("ambientIntensity", ambientIntensity);
        m_ComputeShader.SetFloat("diffuseIntensity", diffuseIntensity);
        m_ComputeShader.SetFloat("specularIntensity", specularIntensity);
        m_ComputeShader.SetFloat("smoothing", smoothing);

        Light[] lights = Light.GetLights(LightType.Directional, 0);
        //ComputeBuffer lightBuffer = new ComputeBuffer(lights.Length, Light);

        renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();

        // Set render texture in shader
    }

    Sphere[] GenerateRandomSpheres()
    {
        Sphere[] spheres = new Sphere[15];
        for (int i = 0; i < spheres.Length - 1; i++)
        {
            spheres[i].position = new Vector3(Random.Range(0f, 10f), Random.Range(0f, 10f), Random.Range(0f, 10f));
            spheres[i].radius = Random.Range(0, 0.1f);
            spheres[i].color = new Vector3(Random.Range(0,1f), Random.Range(0, 1f), Random.Range(0, 1f));
        }
        return spheres;
    }
    Cube[] GenerateRandomCubes()
    {
        Cube[] cubes = new Cube[20];
        for (int i = 0; i < cubes.Length - 1; i++)
        {
            cubes[i].position = new Vector3(Random.Range(0f, 10f), Random.Range(0f, 10f), Random.Range(0f, 10f));
            cubes[i].bounds = new Vector3(Random.Range(0.1f, 2f), Random.Range(0.1f, 2f), Random.Range(0.1f, 2f));
            cubes[i].color = new Vector3(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
        }
        return cubes;
    }

    Triangle[] GenerateRandomTriangles()
    {
        Triangle[] triangles = new Triangle[12];
        for (int i = 0; i < triangles.Length - 1; i++)
        {
            triangles[i].position = new Vector3(Random.Range(0f, 4f), Random.Range(0f, 4f), Random.Range(0f, 4f));
            triangles[i].vertex1 = new Vector3(Random.Range(0f, 4f), Random.Range(0f, 4f), Random.Range(0f, 4f));
            triangles[i].vertex2 = new Vector3(Random.Range(0f, 4f), Random.Range(0f, 4f), Random.Range(0f, 4f));
            triangles[i].vertex3 = new Vector3(Random.Range(0f, 4f), Random.Range(0f, 4f), Random.Range(0f, 4f));
            triangles[i].color = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        }
        return triangles;
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

    void SetParameters()
    {
        m_ComputeShader.SetMatrix("_CameraToWorld", _camera.cameraToWorldMatrix);
        m_ComputeShader.SetMatrix("_CameraInverseProjection", _camera.projectionMatrix.inverse);
    }
    void Update()
    {
    }

    private void FixedUpdate()
    {
        if (spheres.Length > 1)
        {
            if (spheres[1].position.x > 3)
            {
                speed = -speed;
            }
            if (spheres[1].position.x < -2)
            {
                speed = -speed;
            }
            spheres[1].position += new Vector3(speed, 0, 0);
        }

    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src,depthTexture);
        Init();

        InitRenderTexture();
        CreateScene();
        SetParameters();

        m_ComputeShader.SetTexture(0, "Source", src);
        m_ComputeShader.SetTexture(0, "_DepthTexture", depthTexture);
        m_ComputeShader.SetTexture(0, "Result", renderTexture);

        int threadGroupsX = Mathf.CeilToInt(_camera.pixelWidth / 32.0f);
        int threadGroupsY = Mathf.CeilToInt(_camera.pixelHeight / 32.0f);
        m_ComputeShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);

        // Display the result texture 
        Graphics.Blit(renderTexture, dest);
        for (int i = 0; i < deleteComputeBuffers.Count; i++)
        {
            deleteComputeBuffers[i].Dispose();
            deleteComputeBuffers.RemoveAt(i);
        }
    }

    private void OnDestroy()
    {
        //for (int i = 0; i < deleteComputeBuffers.Count; i++)
        //{
        //    deleteComputeBuffers[i].Dispose();
        //}
    }
}

public struct Sphere
{
    public Vector3 position;
    public float radius;
    public Vector3 color;

    public static int GetSize()
    {
        return (sizeof(float) * 7);
    }
}

public struct Cube
{
    public Vector3 position;
    public Vector3 bounds;
    public Vector3 color;

    public static int GetSize()
    {
        return (sizeof(float) * 9);
    }
}
public struct Triangle
{
    public Vector3 position;
    public Vector3 vertex1;
    public Vector3 vertex2;
    public Vector3 vertex3;
    public Vector3 color;
    public static int GetSize()
    {
        return (sizeof(float) * 15);
    }
};
