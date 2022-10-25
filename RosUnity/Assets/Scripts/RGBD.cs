using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RGBD : MonoBehaviour
{   

    public Material TargetMaterial;
    public Camera cam;

    void Start()
    {
        // get material here
        cam.depthTextureMode = DepthTextureMode.Depth;
        //this.material = TargetMaterial;
    }

    // any other functions here

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, TargetMaterial);
    }

    public void GetRGBD()
    {
        Camera cam = GetComponent<Camera>();
        var render_tex = cam.targetTexture;
        var W = render_tex.width;
        var H = render_tex.height;
        var far_clip = cam.farClipPlane;

        // we will get the RGB-D image in this Texture2D on the CPU
        var rgbd_im = new Texture2D(W, H, TextureFormat.RGBAFloat, false);

        // GPU -> CPU
        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = render_tex;
        rgbd_im.ReadPixels(new Rect(0, 0, W, H), 0, 0);

        Debug.Log(rgbd_im);

        RenderTexture.active = prev;
    }
}