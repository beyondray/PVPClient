using UnityEngine;
using KBEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ArrowNetEvent : MonoBehaviour
{
    //local player shot data cache
    private Dictionary<double, List<GameObject>> arrowsDic = new Dictionary<double, List<GameObject>>();
    private List<double> timeoutkeys = new List<double>();
    private double timeout = 5.0f;

    //non-local player shot data cache
    private Dictionary<double, List<GameObject>> othersArrowsDic = new Dictionary<double, List<GameObject>>();

    // Use this for initialization
    void Start()
    {
        installEvents();
        StartCoroutine("loopWaitShot");
    }

    void installEvents()
    {
        // register by scripts
        KBEngine.Event.registerOut("onArrowEnterWorld", this, "onArrowEnterWorld");
        KBEngine.Event.registerOut("recShot", this, "recShot");
    }

    public void onArrowEnterWorld(UInt64 rndUUID, Int32 eid, KBEngine.Arrow entity)
    {
        //instantiate
        GameObject res = Resources.Load<GameObject>("Prefabs/Arrows/" + entity.resName);
        GameObject arrow = Instantiate(res, entity.position, entity.rotation);
        Arrow arrowScript = arrow.GetComponent<Arrow>();
        if (!arrowScript) arrow.AddComponent<Arrow>();

        arrow.GetComponent<SyncPosRot>().entity = entity;
        arrow.GetComponent<SyncPosRot>().enabled = true;
        entity.renderObj = arrow;

        //set sync identity
        KBEngine.Entity player = KBEngineApp.app.player();
        if (player != null)
        {
            if (entity.masterId == player.id)
            {
                arrow.GetComponent<SyncPosRot>().isLocalPlayer = true;
            }
            else
            {
                arrow.GetComponent<Arrow>().enabled = false;
            }
        }

        //set sync props
        arrow.GetComponent<SyncPosRot>().RealSync(entity);
        arrow.GetComponent<Arrow>().speed = entity.speed;
        arrow.GetComponent<Arrow>().damage = entity.damage;

        //set local props
        KBEngine.Entity masterEntity = KBEngineApp.app.findEntity(entity.masterId);
        if(masterEntity != null)
        {
            GameObject master = masterEntity.renderObj as GameObject;
            if (master != null)
            {
                arrow.transform.forward = master.transform.forward;
                arrow.GetComponent<Arrow>().master = master;
            }
        }

        // wait for parallel shot
        if (arrow.GetComponent<SyncPosRot>().isLocalPlayer)
        {
            arrow.SetActive(false);
            if (!arrowsDic.ContainsKey(entity.bornTime))
                arrowsDic[entity.bornTime] = new List<GameObject>();
            arrowsDic[entity.bornTime].Add(arrow);
        }
        else
        {
            arrow.GetComponent<Renderer>().enabled = false;
            if (!othersArrowsDic.ContainsKey(entity.bornTime))
                othersArrowsDic[entity.bornTime] = new List<GameObject>();
            othersArrowsDic[entity.bornTime].Add(arrow);
        }

    }

    IEnumerator loopWaitShot()
    {
        while(true)
        {
            waitShot();
            yield return null;
        }
    }

    void waitShot()
    {
        //iter for shot
        foreach (double timestamp in arrowsDic.Keys)
        {
            //Debug.Log("shotTimestamp: " + timestamp.ToString());
            //Debug.Log("nowTimestamp: " + TimeEx.getTimeStamp());

            //when receive timeout, means failing, we just mark it
            if (TimeEx.getTimeStamp() - timestamp > timeout)
            {
                timeoutkeys.Add(timestamp);
                continue;
            }

            //calculate count
            List<GameObject> arrows = arrowsDic[timestamp];
            AttackType attckType = 0;
            int parallelCount = 1;
            if (arrows.Count > 0)
            {
                KBEngine.Arrow kbeArrow = (KBEngine.Arrow)arrows[0].GetComponent<SyncPosRot>().entity;
                attckType = (AttackType)kbeArrow.attackType;
                switch (attckType)
                {
                    case AttackType.Frozen:
                        parallelCount = 5;
                        break;

                    case AttackType.Normal:
                    case AttackType.Strong:
                        parallelCount = 1;
                        break;
                }
            }

            //ready for shot
            if (arrows.Count >= parallelCount)
            {
                GameObject master = null;
                for (int i = 0; i < arrows.Count; i++)
                {
                    //set transform
                    GameObject arrow = arrows[i];
                    if (arrow.GetComponent<SyncPosRot>().isLocalPlayer)
                    {
                        master = arrow.GetComponent<Arrow>().master;
                        Transform shotPoint = master.GetComponent<Ashe>().shotTrans;
                        arrow.transform.position = shotPoint.transform.position;
                        float delta = 15f;
                        float yAngle = (int)((i + 1) / 2) * (i % 2 == 0 ? delta : -delta);
                        arrow.transform.Rotate(0, yAngle, 0);
                    }

                    //active
                    arrow.SetActive(true);
                }
                if (master != null)
                {
                    master.GetComponent<SyncPosRot>().entity.cellCall("reqActiveArrows", new object[] { timestamp } );
                }
                arrows.Clear();
                arrowsDic.Remove(timestamp);

                break;
            }
        }

        //clear timeout data
        foreach(double timestamp in timeoutkeys)
        {
            List <GameObject> arrows = arrowsDic[timestamp];
            for (int i = 0; i < arrows.Count; i++)
            {
                GameObject arrow = arrows[i];
                arrow.GetComponent<Arrow>().Safe_Destroy();
            }
            arrows.Clear();
            arrowsDic.Remove(timestamp);
        }
        timeoutkeys.Clear();
    }

    public void recShot(KBEngine.Role e, Byte res, Double timestamp)
    {
        if (e.isPlayer()) return;

        if(res == 0)
        {
            if(othersArrowsDic.ContainsKey(timestamp))
            {
                List<GameObject> otherArrows = othersArrowsDic[timestamp];
                for (int i = 0; i < otherArrows.Count; i++)
                {
                    otherArrows[i].GetComponent<Renderer>().enabled = true;
                }
                otherArrows.Clear();
                othersArrowsDic.Remove(timestamp);
            }
        }
    }
}
