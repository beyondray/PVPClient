using UnityEngine;
using System.Collections;

public class CombatNetEvent : MonoBehaviour
{
    //popup prefabs
    public GameObject damagePopupObliPref;
    public GameObject curePopupPref;

    //effect prefafs
    public GameObject cureNormalEffect;
    public GameObject speedUpEffect;
    public GameObject sleepEffect;
    public GameObject dieEffect;

    // Use this for initialization
    void Start()
    {
        installEvents();
    }

    void installEvents()
    {
        KBEngine.Event.registerOut("set_name", this, "set_entityName");
        KBEngine.Event.registerOut("set_moveSpeed", this, "set_moveSpeed");
        KBEngine.Event.registerOut("set_HP", this, "set_HP");
        KBEngine.Event.registerOut("set_MP", this, "set_MP");
        KBEngine.Event.registerOut("set_PP", this, "set_PP");
        KBEngine.Event.registerOut("set_MaxHP", this, "set_MaxHP");
        KBEngine.Event.registerOut("set_MaxMP", this, "set_MaxMP");
        KBEngine.Event.registerOut("set_MaxPP", this, "set_MaxPP");

        KBEngine.Event.registerOut("recAttack", this, "recAttack");
        KBEngine.Event.registerOut("recCure", this, "recCure");
        KBEngine.Event.registerOut("recSpeedUp", this, "recSpeedUp");
        KBEngine.Event.registerOut("recRelief", this, "recRelief");
        KBEngine.Event.registerOut("recRelive", this, "recRelive");
    }

    //=================================================================
    //Props process here
    //=================================================================
    private delegate void process(CombatProps e);
    private void SAFE_PROCESS(KBEngine.PropsEntity e, process p)
    {
        GameObject obj = (GameObject)e.renderObj;
        if (obj != null)
        {
            CombatProps cp = obj.GetComponent<CombatProps>();
            if (cp != null)
            {
                p(cp);
            }
        }
    }

    public void set_entityName(KBEngine.PropsEntity e)
    {
        process _p = (cp) =>
        {
            cp.name = e.name;
        };
        SAFE_PROCESS(e, _p);
    }

    public void set_moveSpeed(KBEngine.PropsEntity e)
    {
        process _p = (cp) =>
        {
            cp.m_MoveSpeed = e.speed;
        };
        if (e.className == "Projectile")
        {
            GameObject obj = (GameObject)e.renderObj;
            if (obj != null)
            {
                Arrow a = obj.GetComponent<Arrow>();
                if (a != null)
                {
                    a.speed = e.speed;
                }
            }
        }
        else
            SAFE_PROCESS(e, _p);
    }

    public void set_HP(KBEngine.PropsEntity e, float HP)
    {
        process _p = (cp) =>
        {
            cp.m_HP = HP;
        };
        SAFE_PROCESS(e, _p);
    }

    public void set_MP(KBEngine.PropsEntity e, float MP) {
        process _p = (cp) =>
        {
            cp.m_MP = MP;
        };
        SAFE_PROCESS(e, _p);
    }

    public void set_PP(KBEngine.PropsEntity e, float PP)
    {
        process _p = (cp) =>
        {
            cp.m_PhysPower = PP;
        };
        SAFE_PROCESS(e, _p);
    }

    public void set_MaxHP(KBEngine.PropsEntity e, float HP)
    {
        process _p = (cp) =>
        {
            cp.m_MaxHP = HP;
        };
        SAFE_PROCESS(e, _p);
    }

    public void set_MaxMP(KBEngine.PropsEntity e, float MP)
    {
        process _p = (cp) =>
        {
            cp.m_MaxMP = MP;
        };
        SAFE_PROCESS(e, _p);
    }

    public void set_MaxPP(KBEngine.PropsEntity e, float PP)
    {
        process _p = (cp) =>
        {
            cp.m_MaxPhysPower = PP;
        };
        SAFE_PROCESS(e, _p);
    }

    //=================================================================
    //Combat rec here
    //=================================================================
    public void recAttack(KBEngine.Role e, int attackType, float damage)
    {
        if (e.renderObj == null) return;

        GameObject player = e.renderObj as GameObject;
        GameObject dp = (GameObject)Instantiate(damagePopupObliPref, player.transform.position, Quaternion.identity);
        dp.GetComponent<ValuePopup>().m_Player = player;
        dp.GetComponent<ValuePopup>().m_Value = (int)damage;
        CombatProps cp = player.GetComponent<CombatProps>();
        if (cp == null) return;

        switch ((AttackType)attackType)
        {
            case AttackType.Normal:
                break;

            case AttackType.Frozen:
                if (!cp.isInFrozenDBuff)
                {
                    cp.inFrozenDBuff();
                }
                break;
            case AttackType.Strong:
                if (!cp.isSleep())
                {
                    Instantiate(sleepEffect, player.transform.position, player.transform.rotation, player.transform);
                    cp.inSleepDBuff();
                }
                break;
        }

    }

    public void recCure(KBEngine.Role e, int cureType, float hp)
    {
        if (e.renderObj == null) return;

        GameObject player = e.renderObj as GameObject;
        GameObject dp = (GameObject)Instantiate(curePopupPref, player.transform.position, Quaternion.identity);
        dp.GetComponent<ValuePopup>().m_Player = player;
        dp.GetComponent<ValuePopup>().m_Value = (int)hp;

        switch ((CureType)cureType)
        {
            case CureType.Normal:
                Instantiate(cureNormalEffect, player.transform.position, player.transform.rotation, player.transform);
                break;
            case CureType.Bless:
                break;
        }
    }

    public void recSpeedUp(KBEngine.Role e, float speed)
    {
        if (e.renderObj == null) return;

        GameObject player = e.renderObj as GameObject;
        CombatProps cp = player.GetComponent<CombatProps>();
        cp.inSpeedUpBuff();

        Instantiate(speedUpEffect, player.transform.position, player.transform.rotation, player.transform);

    }

    public void recRelief(KBEngine.Role e, ReliefType type)
    {
        if (e.renderObj == null) return;

        GameObject player = e.renderObj as GameObject;
        CombatProps cp = player.GetComponent<CombatProps>();

        switch (type)
        {
            case ReliefType.Frozen:
                cp.outFrozenDBuff();
                break;
            case ReliefType.SpeedUp:
                cp.outSpeedUpBuff();
                break;
            case ReliefType.Sleep:
                cp.outSleepDBuff();
                break;
        }
    }

    public void recRelive(KBEngine.Role e, Vector3 relivePos)
    {
        if (e.renderObj == null) return;
        GameObject player = e.renderObj as GameObject;

        Transform dieTrans = player.transform;
        switch (e.career)
        {
            case 1:
                Ashe ashe = player.GetComponent<Ashe>();
                dieTrans = ashe.hipTrans;
                break;
            case 2:
                Ashe ali = player.GetComponent<Ashe>();
                dieTrans = ali.hipTrans;
                break;
        }
        Instantiate(dieEffect, dieTrans.position, player.transform.rotation, player.transform);

    }

}
