using UnityEngine;

public class BulletTracer : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public float duration = 0.2f; // How long the tracer should be visible

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        Destroy(gameObject, duration);
    }

    public void SetPositions(Vector3 startPosition, Vector3 endPosition)
    {
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
    }
}
