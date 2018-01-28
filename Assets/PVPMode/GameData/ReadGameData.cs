using UnityEngine;
using System.Collections;

public class ReadGameData : MonoBehaviour {

    [RuntimeInitializeOnLoadMethod]
    public static void initSettings()
    {
        //NetMode
        if (!PlayerPrefs.HasKey("NetMode"))
            Globe.netMode = true;
        else
            Globe.netMode = PlayerPrefs.GetInt("NetMode") > 0;

        //ShowFPS
        if (!PlayerPrefs.HasKey("ShowFPS"))
            Globe.showFPS = true;
        else
            Globe.showFPS = PlayerPrefs.GetInt("ShowFPS") > 0;

        //PlayMusic
        if (!PlayerPrefs.HasKey("PlayMusic"))
            Globe.playMusic = true;
        else
            Globe.playMusic = PlayerPrefs.GetInt("PlayMusic") > 0;
        if (!Globe.playMusic) AudioListener.pause = true;

        //VirtualButton
        if (!PlayerPrefs.HasKey("VirtualButton"))
            Globe.virtualButton = true;
        else
            Globe.virtualButton = PlayerPrefs.GetInt("VirtualButton") > 0 ? true : false;
    }

}
