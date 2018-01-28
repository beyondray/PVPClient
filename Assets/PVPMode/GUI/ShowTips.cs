using UnityEngine;
using System.Collections;

public class ShowTips : MonoBehaviour {
    public float m_ExistTime;
    public string m_Text;
    public SHOWREGION m_Region;
    public GUIStyle m_GUIStle;

    Rect m_Rect;
    float m_ExistAccumTime;

    [System.Serializable]
    public enum SHOWREGION
    {
        CENTER = 0,
        UP = 1,
        DOWN = 2
    }

    void Start()
    {
        m_GUIStle.fontSize = (int)((float)Screen.width / 500 * 14);
    }
	// Update is called once per frame
	void Update () {
        m_ExistAccumTime += Time.deltaTime;
        if (m_ExistAccumTime >= m_ExistTime)
            Destroy(this.gameObject);
	}

    void OnGUI()
    {
        switch (m_Region)
        {
            case SHOWREGION.CENTER:
                m_Rect = new Rect(0, 0, Screen.width, Screen.height);
                break;
            case SHOWREGION.UP:
                m_Rect = new Rect(0, 0, Screen.width, Screen.height/2);
                break;
            case SHOWREGION.DOWN:
                m_Rect = new Rect(0, Screen.height/2, Screen.width, Screen.height/2);
                break;
        }
        
        GUI.Label(m_Rect, m_Text, m_GUIStle);
    }
}
