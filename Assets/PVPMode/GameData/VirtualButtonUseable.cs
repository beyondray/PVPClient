using UnityEngine;
using System.Collections;

public class VirtualButtonUseable : MonoBehaviour {
    public GameObject virtualControl;
    public GameObject actionLeft;
    public GameObject actionRight;
    void Awake()
    {
        if (!Globe.virtualButton)
        {
            Destroy(virtualControl);
            Destroy(actionLeft);
            Destroy(actionRight);
        }
        Destroy(this);
    }
}
