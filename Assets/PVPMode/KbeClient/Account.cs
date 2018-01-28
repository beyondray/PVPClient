using UnityEngine;
using KBEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace KBEngine
{
    public class Account : Entity
    {
        public Dictionary<UInt64, Dictionary<string, object>> roles = new Dictionary<UInt64, Dictionary<string, object>>();

        public override void __init__() 
        {
            Event.fireOut("onLoginSuccess", new object[] { KBEngineApp.app.entity_uuid, id, this });

            Event.registerIn("reqRoleList", this, "reqRoleList");
            Event.registerIn("reqCreateRole", this, "reqCreateRole");
            Event.registerIn("reqRemoveRole", this, "reqRemoveRole");
        }

        public override void onDestroy()
        {
            KBEngine.Event.deregisterIn(this);
        }

        public void reqRoleList()
        {
            baseCall("reqRoleList");
        }

        public void recRoleList(Byte res, Dictionary<string, object> roleListInfos)
        {
            roles.Clear();
            List<object> listinfos = (List<object>)roleListInfos["value"];
            foreach (Dictionary<string, object> info in listinfos)
            {
                UInt64 dbid = (UInt64)info["dbid"];
                roles[dbid] = info;
            }
            Event.fireOut("recRoleList", new object[] { res, roles });
        }

        public void reqCreateRole(String name, Byte career)
        {
            baseCall("reqCreateRole", new object[] { name, career});
        }

        public void recCreateRole(Byte res, Dictionary<string, object> roleInfo)
        {
            if(res == 0)
            {
                roles[(UInt64)roleInfo["dbid"]] = roleInfo;
            }
            Event.fireOut("recCreateRole", new object[] { res, roleInfo});
        }

        public void reqRemoveRole(UInt64 dbid)
        {
            baseCall("reqRemoveRole", new object[] { dbid });
        }

        public void recRemoveRole(UInt64 dbid)
        {
            roles.Remove(dbid);
            Event.fireOut("recRemoveRole", new object[] { dbid });
        }

        public void reqEnterGame(UInt64 dbid)
        {
            baseCall("reqEnterGame", new object[] { dbid });
        }
    }
}

