using UnityEngine;
using System.Collections;

public class Ali : PlayerController
{
    //effect
    public GameObject rebornEffect;

    //shot point
    public Transform shotTrans;

    //skill mp
    protected float skill_1_MP = 100f;
    protected float skill_2_MP = 100f;
    protected float skill_3_Phy = 500f;


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
                m_CombatProps.speedUp(10f, skill_3_Phy);
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

}
