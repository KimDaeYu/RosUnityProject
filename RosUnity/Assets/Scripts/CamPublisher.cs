using System;
using RosMessageTypes.Geometry;
using RosMessageTypes.NiryoMoveit;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using Unity.Robotics.UrdfImporter;
using UnityEngine;
using UnityEngine.UI;

using Unity.Robotics.ROSTCPConnector.MessageGeneration;

namespace RosMessageTypes.Sensor{
    public class CamPublisher : MonoBehaviour
    {
        const int k_NumRobotJoints = 6;

        public static readonly string[] LinkNames =
            { "base/link1", "/link2", "/link3", "/link4", "/link5", "/link6" };

        // Variables required for ROS communication
        [SerializeField]
        string m_TopicName = "/chatter";

        [SerializeField]
        Camera sensorCamera;

        [SerializeField]
        GameObject greencamp;
        [SerializeField]
        GameObject m_Target;
        [SerializeField]
        GameObject m_TargetPlacement;
        readonly Quaternion m_PickOrientation = Quaternion.Euler(90, 90, 0);

        // Robot Joints
        UrdfJointRevolute[] m_JointArticulationBodies;

        // ROS Connector
        ROSConnection m_Ros;

        void Start()
        {
            // Get ROS connection static instance
            m_Ros = ROSConnection.GetOrCreateInstance();
            m_Ros.RegisterPublisher<Sensor.CompressedImageMsg>(m_TopicName);

        }

        void FixedUpdate()
        {
            SendImage();
        }
        
        public void SendImage()
        {
            var oldRT = RenderTexture.active;
            RenderTexture.active = sensorCamera.targetTexture;
            sensorCamera.Render();
            
            // Copy the pixels from the GPU into a texture so we can work with them
            // For more efficiency you should reuse this texture, instead of creating a new one every time
            Texture2D camText = new Texture2D(sensorCamera.targetTexture.width, sensorCamera.targetTexture.height);
            camText.ReadPixels(new Rect(0, 0, sensorCamera.targetTexture.width, sensorCamera.targetTexture.height), 0, 0);
            camText.Apply();
            RenderTexture.active = oldRT;
            
            // Encode the texture as a PNG, and send to ROS
            byte[] imageBytes = camText.EncodeToPNG();
            var message = new Sensor.CompressedImageMsg(new Std.HeaderMsg(), "png", imageBytes);
            m_Ros.Publish(m_TopicName, message);

            Texture2D.DestroyImmediate(camText);
        }

    }
}