using UnityEngine;
using System.Collections;

public class FramePerSecond : MonoBehaviour {

    float frames = 0f;
    float accumTime = 0f;
    float showFrames = 0f;
    public float updateTimes = 1f;

	// Update is called once per frame
	void Update () {
        if (!Globe.showFPS) return;
        frames+=1f;
        accumTime += Time.deltaTime;      
	}

    void OnGUI ()
    {
        if (!Globe.showFPS) return;
        if (accumTime > updateTimes)
        {
            showFrames = frames / accumTime;          
            frames = 0f;
            accumTime = 0f;
        }

        GUILayout.BeginArea(new Rect(10, Screen.height - 60, 100, 60));
        GUILayout.Label("FPS: " + showFrames.ToString("f2"));
        GUILayout.EndArea();  
    }
}
