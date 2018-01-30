using UnityEngine;
using UnityEngine.UI;
using KBEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MainNetEvent : MonoBehaviour
{    
    private Camera ctrlCamera;
    private static int skillCount = 4;
    private Transform[] skillArr = new Transform[skillCount + 1];

    // Use this for initialization
    void Start()
    {
        gameObject.AddComponent<AniNetEvent>();
        gameObject.AddComponent<ArrowNetEvent>();
        installEvents();
        ctrlCamera = Camera.main;
    }

    void installEvents()
    {
        // register by kbe plugin
        KBEngine.Event.registerOut("addSpaceGeometryMapping", this, "addSpaceGeometryMapping");
        KBEngine.Event.registerOut("onEnterWorld", this, "onEnterWorld");
        KBEngine.Event.registerOut("onLeaveWorld", this, "onLeaveWorld");
        KBEngine.Event.registerOut("set_position", this, "set_position");
        //KBEngine.Event.registerOut("set_direction", this, "set_direction");
        KBEngine.Event.registerOut("updatePosition", this, "updatePosition");
        KBEngine.Event.registerOut("updateDirection", this, "updateDirection");
        KBEngine.Event.registerOut("onControlled", this, "onControlled");
        KBEngine.Event.registerOut("onLoseControlledEntity", this, "onLoseControlledEntity");

        // register by scripts
        KBEngine.Event.registerOut("onSceneAlloc", this, "onSceneAlloc");
        KBEngine.Event.registerOut("onMoveObjEnterWorld", this, "onMoveObjEnterWorld");
        KBEngine.Event.registerOut("onRoleEnterWorld", this, "onRoleEnterWorld");
        KBEngine.Event.registerOut("onObjKilled", this, "onObjKilled");
    }


    public void onSceneAlloc(UInt64 rndUUID, Int32 eid, KBEngine.SceneAlloc alloc)
    {
        SceneSyncBind.reqMoveObjsSync();
    }

    public void onMoveObjEnterWorld(UInt64 rndUUID, Int32 eid, KBEngine.MoveObj entity)
    {
        //instantiate
        GameObject obj = GameObject.Find(entity.name);
        if (obj == null) return;
        obj.GetComponent<Renderer>().enabled = true;
        entity.renderObj = obj;

        //set init props
        set_position(entity);
        set_direction(entity);

        //set sync entity
        obj.GetComponent<SyncPosRot>().entity = entity;
        obj.GetComponent<SyncPosRot>().isDynamicMode = true;
        obj.GetComponent<SyncPosRot>().isLerpMotion = true;
        obj.GetComponent<SyncPosRot>().enabled = true;
    }

    private void loadRoleSkillIcon(KBEngine.Role entity)
    {
        GameObject ui = Resources.Load("Prefabs/UI/Play_Canvas") as GameObject;
        ui = Instantiate(ui);

        //head
        Transform headBlood = ui.transform.FindChild("HeadBlood");
        Transform frameImage = headBlood.transform.FindChild("Frame_Image");
        Transform head = frameImage.FindChild("Head_Image");
        Sprite headSp = Resources.Load<Sprite>("ui/" + entity.role_name + "_circle");
        head.GetComponent<Image>().sprite = headSp;

        //skill
        Transform actionRight = ui.transform.FindChild("Action_Right");
        for (int i = 1; i <= skillCount; i++)
        {
            skillArr[i] = actionRight.FindChild("Skill" + i + "_C_Image"); ;
            Sprite skillSp = Resources.Load<Sprite>("Atlas/skill/" + entity.role_name + i);
            skillArr[i].GetComponent<Image>().sprite = skillSp;
        }
    }

    PlayerController getPlayerController(KBEngine.Role e)
    {
        GameObject obj = (GameObject)e.renderObj;
        PlayerController _e = null;

        if (obj != null)
        {
            switch (e.career)
            {
                case 1:
                    _e = obj.GetComponent<Ashe>();
                    break;

                case 2:
                    _e = obj.GetComponent<Ali>();
                    break;
            }
        }
        return _e;
    }
    
    public void onRoleEnterWorld(UInt64 rndUUID, Int32 eid, KBEngine.Role entity)
    {
        Debug.Log("当前玩家[" + entity.id + "]进入游戏世界...");

        //instantiate
        float y = entity.position.y;
        if (entity.isOnGround)
            y = 1.3f;

        Vector3 pos = new Vector3(entity.position.x, y, entity.position.z);
        GameObject hero = Resources.Load("Prefabs/Heros/" + entity.role_name) as GameObject;
        GameObject player = Instantiate(hero, pos, entity.rotation) as GameObject;
        entity.renderObj = player;

        //set init props
        set_position(entity);
        set_direction(entity);

        //set sync entity
        player.GetComponent<SyncPosRot>().entity = entity;
        player.GetComponent<SyncPosRot>().enabled = true;

        //set local controll
        bool isPlayer = entity.isPlayer();
        player.GetComponent<SyncPosRot>().isLocalPlayer = isPlayer;
        player.GetComponent<BloodHUD>().enabled = !isPlayer;
        player.GetComponent<RoleHUD>().enabled = isPlayer;
        getPlayerController(entity).isLocalPlayer = isPlayer;

        if (isPlayer)
        {
            player.tag = "localPlayer";
            loadRoleSkillIcon(entity);
        }
        else
        {
            player.tag = "nonLocalPlayer";
        }
    }

    public void onObjKilled(KBEngine.PropsEntity entity)
    {
        if (entity.renderObj == null)
            return;

        Destroy((GameObject)entity.renderObj);
        entity.renderObj = null;
    }
    
    public void onEnterWorld(KBEngine.PropsEntity entity)
    {
       
    }

    public void onLeaveWorld(KBEngine.PropsEntity entity)
    {
        if (entity.renderObj == null)
            return;

        if (entity.className == "Projectile")
            return;

        Destroy((GameObject)entity.renderObj);
        entity.renderObj = null;
    }

    public void addSpaceGeometryMapping(string respath)
    {
        Debug.Log("加载场景(" + respath + ")...");
    }

    public void set_position(KBEngine.PropsEntity entity)
    {
        if (entity.renderObj == null)
            return;

        SyncPosRot syncScript = ((GameObject)entity.renderObj).GetComponent<SyncPosRot>();
        syncScript.RealSyncPosition(entity);
        syncScript.spaceID = KBEngineApp.app.spaceID;
    }

    public void updatePosition(KBEngine.PropsEntity entity)
    {
        if (entity.renderObj == null)
            return;

        SyncPosRot syncScript = ((GameObject)entity.renderObj).GetComponent<SyncPosRot>();
        syncScript.syncPos = entity.position;
        syncScript.spaceID = KBEngineApp.app.spaceID;
    }

    public void set_direction(KBEngine.PropsEntity entity)
    {
        if (entity.renderObj == null)
            return;

        SyncPosRot syncScript = ((GameObject)entity.renderObj).GetComponent<SyncPosRot>();
        syncScript.RealSyncRotation(entity);
        syncScript.spaceID = KBEngineApp.app.spaceID;
    }

    public void updateDirection(KBEngine.PropsEntity entity)
    {
        if (entity.renderObj == null)
            return;

        SyncPosRot syncScript = ((GameObject)entity.renderObj).GetComponent<SyncPosRot>();
        syncScript.syncEuler = entity.eulerAngles;
        syncScript.spaceID = KBEngineApp.app.spaceID;
    }

    public void onControlled(KBEngine.Entity entity, bool isControlled)
    {
        if (entity.renderObj == null)
            return;

        Debug.Log("onControlled: " + entity.isControlled);

    }

    public void onLoseControlledEntity(KBEngine.Entity entity)
    {
        return;
    }
}
