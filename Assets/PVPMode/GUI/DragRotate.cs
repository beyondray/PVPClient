using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class DragRotate : MonoBehaviour
{
    public float dragSpeed = 30f;
    Vector3 beginPos;

    private void OnMouseDown()
    {
        beginPos = Input.mousePosition;
    }

    private void OnMouseDrag()
    {
        Vector3 delta = Input.mousePosition - beginPos;
        beginPos = Input.mousePosition;
        transform.Rotate(Vector3.up, -delta.x * dragSpeed * Time.deltaTime);
    }
}
