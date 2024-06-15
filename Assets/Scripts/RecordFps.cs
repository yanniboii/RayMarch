using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RecordFps : MonoBehaviour
{
    string path = @"C:\RayMarch\Assets\Data\RecordFPSTest3.csv";
    float updateInterval = 1.0f;
    float timeSinceLastUpdate = 0.0f;
    int frameCount = 0;
    float fps;

    void Start()
    {
        int i = 1;
        // Create the file and write the header
        while (File.Exists(path))
        {

            path = @"C:\RayMarch\Assets\Data\RecordFPSTest"+i+".csv";

            i++;
        }
        if (!File.Exists(path))
        {
            using (StreamWriter writer = new StreamWriter(path, false))
            {
                writer.Write("Time, FPS");
                writer.Write(System.Environment.NewLine);
            }
        }

    }

    void Update()
    {
        frameCount++;
        timeSinceLastUpdate += Time.deltaTime;

        if (timeSinceLastUpdate >= updateInterval)
        {
            fps = frameCount / timeSinceLastUpdate;
            // Append FPS to the file in a safe manner
            try
            {
                using (StreamWriter writer = new StreamWriter(path, true))
                {
                    writer.Write(Time.time);
                    writer.Write(",");
                    writer.Write(fps);
                    writer.Write(System.Environment.NewLine);
                }
            }
            catch (IOException ex)
            {
                Debug.LogError("Error writing to the file: " + ex.Message);
            }

            frameCount = 0;
            timeSinceLastUpdate = 0.0f;
        }

    }
}