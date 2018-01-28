using UnityEngine;
using UnityEditor;

public class NetModeMenu : ScriptableObject
{
    [MenuItem("NetMode/Open")]
    static void NetOpen()
    {
        Globe.netMode = true;
        PlayerPrefs.SetInt("NetMode", 1);
        //EditorUtility.DisplayDialog("NetMode", "已切换到网络模式!!", "OK", "");
        Debug.LogWarning("NetMode: Open, 已切换到网络模式!!");
    }

    [MenuItem("NetMode/Close")]
    static void NetClose()
    {
        Globe.netMode = false;
        PlayerPrefs.SetInt("NetMode", 0);
        //EditorUtility.DisplayDialog("NetMode", "已切换到单机模式!!", "OK", "");
        Debug.LogWarning("NetMode: Close, 已切换到单机模式!!");
    }
}