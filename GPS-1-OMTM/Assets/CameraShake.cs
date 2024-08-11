using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
  
    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector2 originalPos = transform.position;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector2(transform.position.x + x, transform.position.y + y);

            elapsed += Time.deltaTime;
            //Debug.Log("SHAKING");
            yield return null;
        }

        transform.position = originalPos;
        
    }
}
