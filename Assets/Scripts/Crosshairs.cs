using UnityEngine;
using System.Collections;

public class Crosshairs : MonoBehaviour
{

    public LayerMask targetMask;
    public SpriteRenderer dot;
    public Color dotHighlightColour;
    Color originalDotColour;

    void Start()
    {
        Cursor.visible = false;
        originalDotColour = dot.color;
    }

    void Update()
    {
        transform.Rotate(Vector3.forward * -40 * Time.deltaTime);
    }

    public void DetectTargets(Ray ray)
    {
        if (Physics.Raycast(ray, 100, targetMask))
        {
            dot.color = dotHighlightColour;
        }
        else
        {
            dot.color = originalDotColour;
        }
    }

    public void SetFPSCrosshair(Camera camera)
    {
        // returns a point exactly 20 meters in front of the camera:
        var point = camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 20f));
        transform.position = point;
        transform.LookAt(camera.transform);


    }
}