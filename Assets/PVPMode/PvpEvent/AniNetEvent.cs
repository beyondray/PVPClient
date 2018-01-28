using UnityEngine;
using System.Collections;

public class AniNetEvent : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        installEvents();
    }

    void installEvents()
    {
        // register by scripts
        KBEngine.Event.registerOut("set_run", this, "set_run");
        KBEngine.Event.registerOut("set_atk", this, "set_atk");
        KBEngine.Event.registerOut("set_die", this, "set_die");
    }

    //=================================================================
    // Desc: Ani process here
    //=================================================================
    private delegate void process(PlayerController p);
    private void SAFE_PROCESS(KBEngine.Role e, process p)
    {
        GameObject obj = (GameObject)e.renderObj;
        if (obj != null)
        {
            PlayerController _e = null;
            switch (e.career)
            {
                case 1:
                    _e = obj.GetComponent<Ashe>();
                    break;

                case 2:
                    _e = obj.GetComponent<Ashe>();
                    break;
            }

            if (_e != null)
            {
                p(_e);
            }
        }
    }

    //===================================
    // Desc: ashe hero ani sync
    //===================================
    public void set_die(KBEngine.Role e)
    {
        process _p = (_e) =>
        {
            _e.SyncDie(e.die > 0);
        };

        SAFE_PROCESS(e, _p);
    }

    public void set_run(KBEngine.Role e)
    {
        process _p = (_e) =>
        {
            _e.SyncRun(e.run > 0);
        };

        SAFE_PROCESS(e, _p);
    }

    public void set_atk(KBEngine.Role e)
    {
        process _p = (_e) =>
        {
            _e.SyncAtk(e.atk);
        };

        SAFE_PROCESS(e, _p);
    }
}
