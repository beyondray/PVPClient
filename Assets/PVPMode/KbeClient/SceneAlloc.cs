using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KBEngine
{
    public class SceneAlloc : PropsEntity
    {
        public override void __init__()
        {
            Debug.Log("场景分配器初始化!!");
            Event.fireOut("onSceneAlloc", new object[] { KBEngineApp.app.entity_uuid, id, this });
            installEvents();
        }

        public void installEvents()
        {
            Event.registerIn("reqMoveObjSync", this, "reqMoveObjSync");
            Event.registerIn("reqMoveObjsSync", this, "reqMoveObjsSync");
        }

        public override void onDestroy()
        {
            KBEngine.Event.deregisterIn(this);
        }

        public void reqMoveObjSync(string name, Vector3 pos, Vector3 dir)
        {
            baseCall("reqMoveObjSync", new object[] { name, pos, dir });
        }

        public void reqMoveObjsSync(List<object> moveobjsInfosList)
        {
            baseCall("reqMoveObjsSync", new object[] { moveobjsInfosList });
        }

        public void recMoveObjSync()
        {
            //Event.fireOut("onMoveObjEnterWorld", new object[] { KBEngineApp.app.entity_uuid, id, obj });
        }
    }

}
