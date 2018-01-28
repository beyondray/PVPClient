using UnityEngine;
using KBEngine;
using System;
using System.Collections;

public class AsyncSceneMgr : MonoBehaviour
{
    public static AsyncSceneMgr singleton;
    public static AsyncOperation mainScene = null;
    public UInt64 roleDbid = 0;

    private Coroutine loopReqEnterGameCo = null;

    static AsyncSceneMgr()
    {
        GameObject sceneMgr = new GameObject("sceneMgr");
        singleton = sceneMgr.AddComponent<AsyncSceneMgr>();
        DontDestroyOnLoad(sceneMgr);
    }

    IEnumerator LoopReqEnterGame()
    {
        while(true)
        {
            //_____主场景加载完成______
            if (mainScene != null && mainScene.isDone)
            {
                //Preloader.Initialize();
                reqEnterGame();
            }
            yield return null;
        }
    }

    public void loadMainScene(UInt64 _roleDbid)
    {
        roleDbid = _roleDbid;
        mainScene = Application.LoadLevelAsync("main");
        loopReqEnterGameCo = StartCoroutine(LoopReqEnterGame());
    }

    public void reqEnterGame()
    {
        Account account = (Account)KBEngineApp.app.player();
        if (account != null)
        {
            account.reqEnterGame(roleDbid);
            StopCoroutine(loopReqEnterGameCo);
            loopReqEnterGameCo = null;
        }   
    }
}
