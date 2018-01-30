using UnityEngine;
using System.Collections;

public class Ashe : PlayerController
{
    //effect
    public GameObject rebornEffect;

    //shot point
    public Transform shotTrans;
    public Transform hipTrans;

    //skill mp
    protected float skill_1_MP = 200f;
    protected float skill_2_MP = 250f;
    protected float skill_3_Phy = 500f;
    protected float skill_4_MP = 350f;

    //skill atk
    protected float skill_1_atk = 300f;
    protected float skill_2_atk = 200f;
    protected float skill_3_speed = 10f;
    protected float skill_4_addhp = 250f;

    protected override bool UpdateTransAniState()
    {
        if (!base.UpdateTransAniState()) return false;

        AnimatorStateInfo asInfo = m_Animator.GetCurrentAnimatorStateInfo(0);

        if (asInfo.IsName("crit_shot"))
        {
            if (asInfo.normalizedTime >= 0.01f)
            {
                iAtk = 0;
            }
        }
        else if (asInfo.IsName("spread_shot"))
        {
            if (asInfo.normalizedTime >= 0.01f)
            {
                iAtk = 0;
            }
        }

        return true;
    }

    protected override bool UpdateAttackState() 
    {
        if (!base.UpdateAttackState()) return false;

        if (skill_1_pressed)
        {
            if (m_CombatProps.isMPEnough(skill_1_MP))
            {
                iAtk = 1;
            }
        }

        if (skill_2_pressed)
        {
            if (m_CombatProps.isMPEnough(skill_2_MP))
            {
                iAtk = 2;
            }
        }

        if (skill_3_pressed)
        {
            if (m_CombatProps.isPhysPowerEnough(skill_3_Phy))
            {
                m_CombatProps.speedUp(skill_3_speed, skill_3_Phy);
            }
        }

        if(skill_4_pressed)
        {
            if(m_CombatProps.isMPEnough(skill_4_MP))
            {
                m_CombatProps.cureHealth(CureType.Normal, skill_4_addhp);
                m_CombatProps.useMP(skill_4_MP);
            }
        }
        return true;
    }

    protected override bool DieOrRelive()
    {
        bool die = base.DieOrRelive();
        if(!die)
        {
            Instantiate(rebornEffect, gameObject.transform.position, gameObject.transform.rotation, gameObject.transform);
        }
        return die;
    }

    public void netShot(string resName, int attackType)
    {
        Vector3 euler = Quaternion.AngleAxis(0.0f, Vector3.up).eulerAngles;
        Vector3 dir = Vector3Ex.ToDir(euler);
        int masterId = GetComponent<SyncPosRot>().entity.id;
        float atk = 0;
        switch((AttackType)attackType)
        {
            case AttackType.Strong:
                atk = skill_1_atk;
                break;

            case AttackType.Frozen:
                atk = skill_2_atk;
                break;
        }
        KBEngine.Event.fireIn("reqShot", masterId, resName, attackType, atk, shotTrans.position, dir);
    }

    private GameObject createArrow(string resName)
    {
        GameObject arrow = Resources.Load<GameObject>("Prefabs/Arrows/" + resName);
        arrow = Instantiate(arrow, shotTrans.position, Quaternion.AngleAxis(0.0f, Vector3.up));
        arrow.transform.forward = gameObject.transform.forward;
        Arrow arrowScript = arrow.GetComponent<Arrow>();
        if (!arrowScript) arrow.AddComponent<Arrow>();
        arrow.GetComponent<Arrow>().master = gameObject;
        return arrow;
    }

    public void spreadShot()
    {
        //get resname
        string resName = "FrostProjectileSmall";

        //choose mode to attack
        if (!Globe.netMode)
        {
            for (int i = 0; i < 5; i++)
            {
                GameObject arrow = createArrow(resName);
                arrow.transform.Rotate(0, 30.0f - 15.0f * i, 0);
            }
        }
        else
        {
            netShot(resName, (int)AttackType.Frozen);
        }
        m_CombatProps.useMP(skill_1_MP);
    }

    public void strongShot()
    {
        string resName = "AirProjectile";
        if (!Globe.netMode)
        {
            createArrow(resName);
        }
        else
        {
            netShot(resName, (int)AttackType.Strong);
        }
        m_CombatProps.useMP(skill_2_MP);
    }

    void ShotEvent(int shot)
    {
        if (!isLocalPlayer) return;

        switch ((AttackType)shot)
        {
            case AttackType.Frozen:
                spreadShot();
                break;

            case AttackType.Strong:
                strongShot();
                break;
        }
    }
}
