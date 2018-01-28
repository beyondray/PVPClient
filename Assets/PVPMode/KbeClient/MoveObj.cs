using UnityEngine;
using System.Collections;

namespace KBEngine
{
    public class MoveObj : PropsEntity
    {

        public override void __init__()
        {
            Debug.Log("可移动物体初始化!!");
        }

        public override void onEnterWorld()
        {
            Debug.Log("MoveObj: onEnterWorld call");

            Event.fireOut("onMoveObjEnterWorld", new object[] { KBEngineApp.app.entity_uuid, id, this });

        }
     }

}
