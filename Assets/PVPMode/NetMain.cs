using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KBEngine;

public class NetMain : KBEMain {
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        KBEngine.Event.registerOut("onDisconnected", this, "onDisconnected");
    }

    void OnDestroy()
    {
        MonoBehaviour.print("clientapp::OnDestroy(): begin");
        if (KBEngineApp.app != null)
        {
            KBEngineApp.app.destroy();
            KBEngineApp.app = null;
        }
        MonoBehaviour.print("clientapp::OnDestroy(): end");
        KBEngine.Event.deregisterOut(this);
    }

    public void onDisconnected()
    {
        Application.Quit();

        //info("你已掉线...");
        //Invoke("onReloginBaseappTimer", 1.0f);
    }

    public void onReloginBaseappTimer()
    {

        KBEngineApp.app.reloginBaseapp();

    }

    public void Load()
    {
        Debug.Log("Load Network Client application");
    }
}
