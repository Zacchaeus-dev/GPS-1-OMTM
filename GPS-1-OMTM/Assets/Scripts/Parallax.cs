using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length, startpos;
    public GameObject cam;
    public float parallexEffect;

    public bool fog;
    void Start()
    {
        startpos = transform.position.x;
        //length = GetComponent<SpriteRenderer>().bounds.size.x;
    }
    void Update()
    {
        float temp = (cam.transform.position.x * (1 - parallexEffect));
        float dist = (cam.transform.position.x * parallexEffect);

        if (fog == true)
        {
            StartCoroutine(constantMove());
        }
        else
        {
            transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);
        }
        if (temp > startpos + length) startpos += length;
        else if (temp < startpos - length) startpos -= length;
    }


    IEnumerator constantMove()
    {
        while (fog == true)
        {
            transform.position = new Vector2(transform.position.x + 0.001f, transform.position.y);
            yield return new WaitForSeconds(0.1f);
        }
        
    }
}
