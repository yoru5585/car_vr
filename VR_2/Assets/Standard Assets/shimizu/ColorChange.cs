using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChange : MonoBehaviour
{
    [SerializeField] Renderer rd;
    float r, g, b, a;
    // Start is called before the first frame update
    void Start()
    {
        init();
    }

    // Update is called once per frame
    void Update()
    {
        a = 0;
    }

    void init()
    {
        r = rd.material.color.r;
        g = rd.material.color.g;
        b = rd.material.color.b;
        a = rd.material.color.a;
    }

    void setColor(float a)
    {

    }

}
