using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MocapLoader : MonoBehaviour
{
    public Transform[] joints; // Assign your cubes (joints) in the Inspector
    private List<Vector3[]> motionData = new List<Vector3[]>(); // Stores all the frames of motion data

    void Start()
    {
        string fileName = "test.0001.Joints"; // Your CSV file name (without .csv extension)
        LoadMocapData(fileName);
        StartCoroutine(AnimateSkeleton());
    }

    void LoadMocapData(string fileName)
    {
        TextAsset csvFile = Resources.Load<TextAsset>(fileName);
        if (csvFile == null)
        {
            Debug.LogError("CSV file not found in Resources folder!");
            return;
        }

        string[] lines = csvFile.text.Split('\n');
        List<Vector3> currentFrame = new List<Vector3>();

        foreach (string line in lines)
        {
            string[] values = line.Split(',');

            // Skip empty lines or lines with insufficient data
            if (values.Length < 6) continue;

            float timestamp = float.Parse(values[0]); // Timestamp (ignored)
            int frame = int.Parse(values[1]); // Frame number (ignored)
            int jointID = int.Parse(values[2]); // Joint ID

            // Extract the X, Y, Z values for this joint
            float x = float.Parse(values[3]);
            float y = float.Parse(values[4]);
            float z = float.Parse(values[5]);

            // Ensure we have a valid joint ID
            if (jointID > 0 && jointID <= joints.Length)
            {
                Vector3 jointPosition = new Vector3(x, y, z);

                // If we're in the same frame, add this joint position to the current frame
                if (frame > currentFrame.Count)
                {
                    currentFrame.Add(jointPosition);
                }
                else
                {
                    currentFrame[jointID - 1] = jointPosition; // Update position of this joint in the current frame
                }
            }

            // Once all joints are assigned for this frame, store the frame data
            if (currentFrame.Count == joints.Length)
            {
                motionData.Add(currentFrame.ToArray());
                currentFrame.Clear();
            }
        }
    }

    IEnumerator AnimateSkeleton()
    {
        while (true) // Loop the animation
        {
            for (int frame = 0; frame < motionData.Count; frame++)
            {
                // Apply the positions from the current frame to each joint (cube)
                for (int j = 0; j < joints.Length; j++)
                {
                    joints[j].localPosition = motionData[frame][j];
                }
                yield return new WaitForSeconds(1f / 30f); // Assuming 30 FPS
            }
        }
    }
}
