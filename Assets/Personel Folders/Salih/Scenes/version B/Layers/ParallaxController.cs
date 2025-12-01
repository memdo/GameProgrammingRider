using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    Transform cam; // Main Camera
    Vector3 camStartPos;

    GameObject[] backgrounds;
    Material[] mat;
    float[] backSpeed;

    float farthestBack;

    [Range(0.01f, 0.05f)]
    public float parallaxSpeed = 0.03f;

    void Start()
    {
        cam = Camera.main.transform;
        camStartPos = cam.position;

        int backCount = transform.childCount;
        mat = new Material[backCount];
        backSpeed = new float[backCount];
        backgrounds = new GameObject[backCount];

        for (int i = 0; i < backCount; i++)
        {
            backgrounds[i] = transform.GetChild(i).gameObject;

            if (backgrounds[i].TryGetComponent(out Renderer rend))
                mat[i] = rend.material;
        }

        BackSpeedCalculate(backCount);
    }

    void BackSpeedCalculate(int backCount)
    {
        // Find farthest background
        for (int i = 0; i < backCount; i++)
        {
            float dist = backgrounds[i].transform.position.z - cam.position.z;
            if (dist > farthestBack)
                farthestBack = dist;
        }

        // Calculate speed for each layer
        for (int i = 0; i < backCount; i++)
        {
            float dist = backgrounds[i].transform.position.z - cam.position.z;
            backSpeed[i] = 1 - (dist / farthestBack);
        }
    }

    private void LateUpdate()
    {
        float distX = cam.position.x - camStartPos.x;
        float distY = cam.position.y - camStartPos.y;

        // Move parallax container with camera
        transform.position = new Vector3(cam.position.x, cam.position.y, transform.position.z);

        // Scroll backgrounds (texture offset)
        for (int i = 0; i < backgrounds.Length; i++)
        {
            float speed = backSpeed[i] * parallaxSpeed;
            if (mat[i] != null)
            {
                mat[i].SetTextureOffset("_MainTex", new Vector2(distX, distY) * speed);
            }
        }
    }
}
