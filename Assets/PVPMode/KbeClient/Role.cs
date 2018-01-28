using UnityEngine;
using KBEngine;
using System;
using System.Collections;

namespace KBEngine
{
    public class Role : PropsEntity
    {
        public Byte career
        {
            get
            {
                return (Byte)getDefinedProperty("career");
            }
        }

        public string career_name
        {
            get
            {
                Byte career = (Byte)getDefinedProperty("career");
                return RoleList.getCareerName(career); 
            }
        }

        public string role_name
        {
            get
            {
                Byte career = (Byte)getDefinedProperty("career");
                return RoleList.getRoleName(career);
            }
        }

        public Byte die
        {
            get
            {
                return (Byte)getDefinedProperty("die");
            }
        }

        public Byte run
        {
            get
            {
                return (Byte)getDefinedProperty("run");
            }
        }

        public int atk
        {
            get
            {
                return (int)getDefinedProperty("atk");
            }
        }

        public virtual void set_die(object old)
        {
            Event.fireOut("set_die", new object[] { this });
        }

        public virtual void set_run(object old)
        {
            Event.fireOut("set_run", new object[] { this });
        }

        public virtual void set_atk(object old)
        {
            Event.fireOut("set_atk", new object[] { this });
        }

        public override void __init__()
        {
            Debug.Log("角色初始化!!");

            if (isPlayer())
            {
                installEvents();
            }
        }

        public void installEvents()
        {
            Event.registerIn("reqShot", this, "reqShot");
        }

        public override void onDestroy()
        {
            if (isPlayer())
            {
                KBEngine.Event.deregisterIn(this);
            }
        }

        public override void onEnterWorld()
        {
            Debug.Log("Role: onEnterWorld call");

            Event.fireOut("onRoleEnterWorld", new object[] { KBEngineApp.app.entity_uuid, id, this });
        }

        public virtual void reqShot(int masterId, string resName, int attackType, float atk, Vector3 pos, Vector3 euler)
        {
            baseCall("reqShot", masterId, resName, attackType, atk, pos, euler);
        }

        public virtual void recShot(Byte res, Double timestamp)
        {
            Event.fireOut("recShot", new object[] { this, res, timestamp });
        }

        public virtual void recAttack(int attackType, float damage)
        {
            Event.fireOut("recAttack", new object[] { this, attackType, damage });
        }
        public virtual void recCure(int cureType, float hp)
        {
            Event.fireOut("recCure", new object[] { this, cureType, hp });
        }

        public virtual void recSpeedUp(float speed)
        {
            Event.fireOut("recSpeedUp", new object[] { this, speed });
        }

        public virtual void recRelief(int type)
        {
            Event.fireOut("recRelief", new object[] { this, type });
        }

        public virtual void recRelive(Vector3 position)
        {
            Event.fireOut("recRelive", new object[] { this, position });
        }

    }
}