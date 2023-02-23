using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundScrollingScript : MonoBehaviour
{
    public MeshRenderer background;
    public float scrollSpeed;
    float y;
    Vector2 offset;

    void Start()
    {

    }

    void Update()
    {
        y = Mathf.Repeat(Time.time * scrollSpeed, 1);
        offset = new Vector2(0, y);
        background.sharedMaterial.SetTextureOffset("_MainTex", offset);
    }
}
