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
    Shadow = 3,
}

public enum ReliefType
{
    Frozen = 0,
    SpeedUp = 1,
    Sleep = 2,
    Mess = 3,
}

public class CombatProps : MonoBehaviour
{
    public GameObject[] matRenderList;

    //props
    public float m_MoveSpeed = 7f;

    public float m_MaxHP = 1000;
    public float m_HP = 1000;
    public float m_HPAddSpeed = 10;

    public float m_MaxMP = 1000;
    public float m_MP = 1000;
    public float m_MPAddSpeed = 100;

    public float m_MaxPhysPower = 1000;
    public float m_PhysPower = 1000;
    public float m_PhyAddSpeed = 30;

    //DBuff
    public bool isInFrozenDBuff = false;
    public bool isInSleepDBuff = false;
    public bool isInMessDBuff = false;
    
    //Buff
    public bool isInSpeedUpBuff = false;

    //Mess
    protected float m_MessTime = 3f;
    protected float m_MessAccumTime = 0f;
    protected int m_MessRunCount = 0;

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
    string getHeroName()
    {
        string name = "";
        KBEngine.Role player = (KBEngine.Role)gameObject.GetComponent<SyncPosRot>().entity;
        if (player != null) name = player.role_name;
        return name;
    }

    public void inFrozenDBuff()
    {
        isInFrozenDBuff = true;
        isInSpeedUpBuff = false;

        //set frozen mat
        Material frozenMat = Resources.Load<Material>("Materials/Frozen_"+ getHeroName());
        for (int i = 0; i < matRenderList.Length; i++)
        {
            matRenderList[i].GetComponent<Renderer>().material = frozenMat;            
        }
        Invoke("outFrozenDBuff", 5f);
    }

    public void outFrozenDBuff()
    {
        //set init mat
        Material orgMat = Resources.Load<Material>("Materials/" + getHeroName());
        for (int i = 0; i < matRenderList.Length; i++)
        {
            matRenderList[i].GetComponent<Renderer>().material = orgMat;
        }

        //TODO: ali头发暂时做特殊处理
        if(getHeroName() == "ali")
        {
            Material mat_hair = Resources.Load<Material>("Materials/ali_hair");
            matRenderList[matRenderList.Length - 1].GetComponent<Renderer>().material = mat_hair;
        }
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

    public void inMessDBuff()
    {
        isInMessDBuff = true;
        Invoke("outMessDBuff", m_MessTime);
    }

    public void outMessDBuff()
    {
        isInMessDBuff = false;
        m_MessAccumTime = 0f;
        m_MessRunCount = 0;
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

    public bool isMess()
    {
        return isInMessDBuff;
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

    public bool teleportBeg(float speed, float needMP)
    {
        if (m_MP < needMP) return false;

        if (!Globe.netMode)
        {
            m_MP -= needMP;
            m_MoveSpeed = speed;
        }
        else
        {
            //req speedUp
            safeNetCall("reqTeleportBeg", new object[] { speed, needMP });
        }
        return true;
    }

    public void teleportEnd()
    {
        if(!Globe.netMode)
        {
            m_MoveSpeed = 7f;
        }
        else
        {
            safeNetCall("reqTeleportEnd");
        }
    }

    public void MessRun(ref float h, ref float v)
    {
        if (isMess())
        {
            m_MessAccumTime += Time.deltaTime;
            if (m_MessAccumTime >= m_MessRunCount * 0.5f)
            {
                h = 0f;
                v = 0f;
                while (h == 0f && v == 0f)
                {
                    h = (float)Random.Range(-1, 2);
                    v = (float)Random.Range(-1, 2);
                }
                m_MessRunCount++;
            }
        }
    }
}
