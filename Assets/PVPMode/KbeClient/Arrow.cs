using UnityEngine;
using System;
using System.Collections;

namespace KBEngine
{
    public class Arrow : PropsEntity
    {
        public double bornTime
        {
            get
            {
                return (double)getDefinedProperty("bornTime");
            }
        }

        public int masterId
        {
            get
            {
                return (int)getDefinedProperty("masterId");
            }
        }

        public string resName
        {
            get
            {
                return (string)getDefinedProperty("resName");
            }
        }

        public int attackType
        {
            get
            {
                return (int)getDefinedProperty("attackType");
            }
        }

        public float damage
        {
            get
            {
                return (float)getDefinedProperty("damage");
            }
        }

        public virtual void set_kill(object old)
        {
            Byte isKill = (Byte)getDefinedProperty("kill");
            if (isKill > 0)
            {
                Event.fireOut("onObjKilled", new object[] { this });
            }
        }

        public override void __init__()
        {
            Debug.Log("Arrow初始化!!");
            installEvents();
        }

        public void installEvents()
        {
            Event.registerIn("reqDestroySelf", this, "reqDestroySelf");
        }

        public override void onDestroy()
        {
            KBEngine.Event.deregisterIn(this);
        }

        public override void onEnterWorld()
        {
            Event.fireOut("onArrowEnterWorld", new object[] { KBEngineApp.app.entity_uuid, id, this });
        }

        public virtual void reqDestroySelf()
        {
            baseCall("reqDestroySelf");
        }

        public override void onLeaveSpace()
        {
            base.onLeaveSpace();
        }
    }
}
