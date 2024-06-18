using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using UnityEditor;

public class RecordFps : MonoBehaviour
{
    [SerializeField] string FileName;
    string path;
    float updateInterval = 1.0f;
    float timeSinceLastUpdate = 0.0f;
    int frameCount = 0;
    float fps;
    bool write = false;

    void Start()
    {
        path = @"C:\RayMarch\Assets\Data\" + FileName + ".csv";
        int i = 1;
        // Create the file and write the header
        while (File.Exists(path))
        {

            path = @"C:\RayMarch\Assets\Data\"+FileName+i+".csv";

            i++;
        }
        if (!File.Exists(path))
        {
            using (StreamWriter writer = new StreamWriter(path, false))
            {
                writer.Write("TimeÜFPS");
                writer.Write(System.Environment.NewLine);
            }
        }

    }

    void Update()
    {
        frameCount++;
        timeSinceLastUpdate += Time.deltaTime;
        if(timeSinceLastUpdate > 5)
        {
            write = true;
        }
        if (write && timeSinceLastUpdate >= updateInterval)
        {
            fps = frameCount / timeSinceLastUpdate;
            // Append FPS to the file in a safe manner
            try
            {
                using (StreamWriter writer = new StreamWriter(path, true))
                {
                    string[] data = { ""+(Time.time-5), ""+fps };
                    foreach (var item in data)
                    {
                        item.Replace(',','.'); // this doesn't work so now i'm using Ü to sepparate values
                    }
                    writer.WriteLine(String.Join("Ü", data));
                }
            }
            catch (IOException ex)
            {
                Debug.LogError("Error writing to the file: " + ex.Message);
            }

            frameCount = 0;
            timeSinceLastUpdate = 0.0f;
        }
        if (Time.time >= 25)
        {
            EditorApplication.isPlaying = false;
        }

    }

}