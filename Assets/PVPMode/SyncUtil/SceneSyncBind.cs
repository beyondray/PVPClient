using UnityEngine;
using KBEngine;
using System.Collections;
using System.Collections.Generic;

public static class SceneSyncBind
{
    public static LayerMask moveObjLayer = LayerMask.NameToLayer("MoveObj");
    static List<object> moveobjs = new List<object>();

    static bool isMoveObjLayer(GameObject obj)
    {
        return moveObjLayer == obj.layer;
    }

    static void CollectMoveObjsInfos()
    {
        moveobjs.Clear();
        GameObject[] objs = Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];
        foreach (GameObject obj in objs)
        {
            if (isMoveObjLayer(obj))
            {
                Dictionary<string, object> info = new Dictionary<string, object>();
                info["name"] = obj.name;
                info["pos"] = obj.transform.position;
                info["dir"] = Vector3Ex.ToDir(obj.transform.eulerAngles);
                moveobjs.Add(info);
            }
        }

    }

    public static void reqMoveObjsSync()
    {
        CollectMoveObjsInfos();
        KBEngine.Event.fireIn("reqMoveObjsSync", new object[] { moveobjs });
    }

}
