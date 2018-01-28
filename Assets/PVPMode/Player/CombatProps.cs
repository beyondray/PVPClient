using UnityEngine;
using KBEngine;
using System.Collections;

public enum CureType
{
    Normal = 0,
    Bless = 1,
}

public enum AttackType
{
    Normal = 0,
    Frozen = 1,
    Strong = 2,
}

public enum ReliefType
{
    Frozen = 0,
    SpeedUp = 1,
    Sleep = 2,
}

public class CombatProps : MonoBehaviour
{
    //props
    public float m_MoveSpeed = 7f;

    public float m_MaxHP = 1000;
    public float m_HP = 1000;
    public float m_HPAddSpeed = 10;

    public float m_MaxMP = 1000;
    public float m_MP = 1000;
    public float m_MPAddSpeed = 10;

    public float m_MaxPhysPower = 1000;
    public float m_PhysPower = 1000;
    public float m_PhyAddSpeed = 100;

    //DBuff
    public bool isInFrozenDBuff = false;
    public bool isInSleepDBuff = false;

    //Buff
    public bool isInSpeedUpBuff = false;

    // Use this for initialization
    void Start()
    {
    }

    void Update()
    {
        if(!Globe.netMode)
        {
            if(!isDie())
            {
                addValue(ref m_HP, Time.deltaTime * m_HPAddSpeed, m_MaxHP);
                addValue(ref m_MP, Time.deltaTime * m_MPAddSpeed, m_MaxMP);
                addValue(ref m_PhysPower, Time.deltaTime * m_PhyAddSpeed, m_MaxPhysPower);
            }

        }
    }

   public float addValue(ref float curV, float deltaV, float maxV)
   {
        curV += deltaV;
        if (curV > maxV)
        {
            curV = maxV;
        }
        return curV;
   }
        
    public float cutValue(ref float curV, float deltaV, float minV)
    {
        curV -= deltaV;
        if (curV < minV)
        {
            curV = minV;
        }
        return curV;
    }

    public void inFrozenDBuff()
    {
        isInFrozenDBuff = true;
        isInSpeedUpBuff = false;
        Transform ashe = gameObject.transform.Find("transformAshe");
        Material frozenMat = Resources.Load<Material>("Materials/Frozen");
        ashe.gameObject.GetComponent<Renderer>().material = frozenMat;
        Invoke("outFrozenDBuff", 5f);
    }

    public void outFrozenDBuff()
    {
        Transform ashe = gameObject.transform.Find("transformAshe");
        Material frozenMat = Resources.Load<Material>("Materials/ashe");
        ashe.gameObject.GetComponent<Renderer>().material = frozenMat;
        isInFrozenDBuff = false;
    }

    public void inSleepDBuff()
    {
        isInSleepDBuff = true;
        Invoke("outSleepDBuff", 3f);
    }

    public void outSleepDBuff()
    {
        isInSleepDBuff = false;
    }

    public void inSpeedUpBuff()
    {
        isInSpeedUpBuff = true;
    }

    public void outSpeedUpBuff()
    {
        isInSpeedUpBuff = false;
    }


    public void safeNetCall(string method, params object[] args)
    {
        if (Globe.netMode)
        {
            PropsEntity kbePlayer = GetComponent<SyncPosRot>().entity;
            if (kbePlayer != null)
            {
                kbePlayer.cellCall(method, args);
            }
        }
    }

    public void cureHealth(CureType type, float hp)
    {
        // add blood
        if(!Globe.netMode)
        {
            addValue(ref m_HP, hp, m_MaxHP);
        }
        else
        {
            // req sync add blood
            safeNetCall("reqBeCure", new object[] { (int)type, hp });
        }

    }

    public void addDamage(AttackType type, float damage)
    {
        if (m_HP <= 0) return;

        // cut blood
        if (!Globe.netMode)
        {
            cutValue(ref m_HP, damage, 0);
        }
        else
        {
            // req sync cut blood
            safeNetCall("reqBeAttack", new object[] { (int)type, damage });
        }
    }

    public bool isHPEnough(float HP)
    {
        return m_HP >= HP;
    }

    public bool isMPEnough(float MP)
    {
        return m_MP >= MP;
    }

    public bool isPhysPowerEnough(float Phy)
    {
        return m_PhysPower >= Phy;
    }

    public bool isDie()
    {
        return m_HP <= 0;
    }

    public bool isSleep()
    {
        return isInSleepDBuff;
    }

    public void useMP(float value)
    {
        if (m_MP < value) return;

        if (!Globe.netMode)
        {
            m_MP -= value;
        }
        else
        {
            //req sync mp
            safeNetCall("reqUseMP", new object[] { value });
        }
    }

    public void relive()
    {
        m_HP = m_MaxHP;

        //req Relive
        safeNetCall("reqRelive");
    }

    public void speedUp(float speed, float needPhy)
    {
        if (isInFrozenDBuff) return;
        if (isInSpeedUpBuff) return;
        if (m_PhysPower < needPhy) return;
        //if (speed <= m_MoveSpeed) return;

        if (!Globe.netMode)
        {
            m_PhysPower -= needPhy;
            m_MoveSpeed = speed;
        }
        else
        {
            //req speedUp
            safeNetCall("reqSpeedUp", new object[] { speed, needPhy });
        }
    }
}
