using UnityEngine;
using System.Collections;

public class Ali : PlayerController
{
    public GameObject shotEffect;
    //effect
    public GameObject rebornEffect;
    public GameObject teleportEffect;

    //shot point
    public Transform shotTrans;
    public Transform hipTrans;

    //skill mp
    protected float skill_1_MP = 150f;
    protected float skill_2_MP = 150f;
    protected float skill_3_Phy = 500f;
    protected float skill_4_MP = 350f;

    //skill atk
    protected float skill_1_atk = 400f;
    protected float skill_2_speed = 25f;
    protected float skill_3_speed = 10f;
    protected float skill_4_addhp = 250f;

    protected override bool UpdateTransAniState()
    {
        if (!base.UpdateTransAniState()) return false;

        AnimatorStateInfo asInfo = m_Animator.GetCurrentAnimatorStateInfo(0);

        if (asInfo.IsName("attack_1"))
        {
            if (asInfo.normalizedTime >= 0.68f)
            {
                iAtk = 0;
            }
        }
        else if (asInfo.IsName("attack_2"))
        {
            if (asInfo.normalizedTime >= 0.01f)
            {
                iAtk = 0;
            }
        }
        else if (asInfo.IsName("attack_3"))
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

        if (skill_4_pressed)
        {
            if (m_CombatProps.isMPEnough(skill_4_MP))
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
        if (!die)
        {
            Instantiate(rebornEffect, gameObject.transform.position, gameObject.transform.rotation, gameObject.transform);
        }
        return die;
    }

    protected override bool CheckAttack()
    {
        bool bAtck = iAtk != 0;
        if (bAtck && iAtk != 2 && iAtk != 3)
        {
            m_Rigidbody.velocity = new Vector3(0, m_Rigidbody.velocity.y, 0);
        }
        return bAtck;
    }

    void TeleportBeg(int v)
    {
        if (!isLocalPlayer) return;

        AddMoveVelocity(skill_2_speed);
        Instantiate(teleportEffect, gameObject.transform.position, gameObject.transform.rotation, gameObject.transform);

        Invoke("_teleportBeg", 0.1f);
    }

    void _teleportBeg()
    {
        m_CombatProps.teleportBeg(skill_2_speed, skill_2_MP);
    }

    void TeleportEnd(int v)
    {
        if (!isLocalPlayer) return;
        m_Rigidbody.velocity = new Vector3(0, m_Rigidbody.velocity.y, 0);
        Invoke("_teleportEnd", 0.3f);
    }

    void _teleportEnd()
    {
        m_CombatProps.teleportEnd();
    }

    public void AddMoveVelocity(float speed)
    {
        float velY = m_Rigidbody.velocity.y;
        Vector3 v = transform.forward * speed;
        m_Rigidbody.velocity = new Vector3(v.x, velY, v.z);
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

    public void netShot(string resName, int attackType)
    {
        Vector3 euler = Quaternion.AngleAxis(0.0f, Vector3.up).eulerAngles;
        Vector3 dir = Vector3Ex.ToDir(euler);
        int masterId = GetComponent<SyncPosRot>().entity.id;
        float atk = 0;
        switch ((AttackType)attackType)
        {
            case AttackType.Shadow:
                atk = skill_1_atk;
                break;
;
        }
        KBEngine.Event.fireIn("reqShot", masterId, resName, attackType, atk, shotTrans.position, dir);
    }

    void AttackEvent(int v)
    {
        if (!isLocalPlayer) return;

        string resName = "ShadowProjectileSmall";
        if (!Globe.netMode)
        {
            createArrow(resName);
        }
        else
        {
            netShot(resName, (int)AttackType.Shadow);
        }
        m_CombatProps.useMP(skill_1_MP);
    }
}
