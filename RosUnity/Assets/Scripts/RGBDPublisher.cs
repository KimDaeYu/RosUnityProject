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
    public class RGBDPublisher : MonoBehaviour
    {
        // Variables required for ROS communication
        [SerializeField]
        string m_TopicName = "/RGBD";

        [SerializeField]
        Camera sensorCamera;
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
            // SendImage();
            SendRGBD();
        }
        
        public void SendRGBD()
        {
            var render_tex = sensorCamera.targetTexture;
            var W = render_tex.width;
            var H = render_tex.height;
            var far_clip = sensorCamera.farClipPlane;

            // we will get the RGB-D image in this Texture2D on the CPU
            var rgbd_im = new Texture2D(W, H, TextureFormat.RGBAFloat, false);
            
            // GPU -> CPU
            RenderTexture prev = RenderTexture.active;
            RenderTexture.active = render_tex;
            rgbd_im.ReadPixels(new Rect(0, 0, W, H), 0, 0);
            // rgbd_im.Apply();
            RenderTexture.active = prev;
            
            // Encode the texture as a PNG, and send to ROS
            byte[] imageBytes = rgbd_im.EncodeToPNG();
            var message = new Sensor.CompressedImageMsg(new Std.HeaderMsg(), "png", imageBytes);
            m_Ros.Publish(m_TopicName, message);

            Texture2D.DestroyImmediate(rgbd_im);
        }

    }
}