using UnityEngine;
using System;
using System.Collections;

public class ReqCtrlCollider : MonoBehaviour
{
    public KBEngine.PropsEntity entity = null;

    void Awake()
    {
        enabled = false;
    }

    void Start()
    {
        if(entity == null)
        {
            SyncPosRot syncScript = GetComponent<SyncPosRot>();
            if(syncScript != null)
            {
                entity = syncScript.entity;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        SyncPosRot script = collision.collider.gameObject.GetComponent<SyncPosRot>();
        if (script != null)
        {
            KBEngine.Entity player = KBEngine.KBEngineApp.app.player();
            if (script.entity != null && player != null)
            {
                if (script.entity.id == player.id)
                {
                    if ((Int32)entity.getDefinedProperty("controllId") != player.id)
                        entity.cellCall("reqControll", new object[] { player.id });
                }
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        //OnCollisionEnter(collision);
    }
}
