using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour {
    public bool isLocalPlayer = true;

    //camera
    protected Camera m_Cam = null;
    public float m_TurnSpeed = 300.0f;
    public float m_Angle = 0.0f;
    public bool m_Invert = true;

    //role props
    protected Rigidbody m_Rigidbody;
    protected CombatProps m_CombatProps;
    public Vector2 m_MoveDir = Vector2.zero;
    protected bool bHasReqRelive = false;

    //skill press
    protected bool skill_1_pressed;
    protected bool skill_2_pressed;
    protected bool skill_3_pressed;
    protected bool skill_4_pressed;

    //ani state
    protected Animator m_Animator;
    protected bool bLastDie = false;
    protected bool bLastRun = false;
    protected int iLastAtk = 0;

    protected bool bDie = false;
    protected bool bRun = false;
    protected int iAtk = 0;

    //ani sync info
    Dictionary<string, object> aniSyncInfo = new Dictionary<string, object>();

    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Animator = gameObject.GetComponent<Animator>();
        m_CombatProps = gameObject.GetComponent<CombatProps>();
        if (m_Cam == null)
        {
            m_Cam = Camera.main;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLocalState();
        UpdateAnimateState();

        UpdatePlayer();
        UpdateCamera();
    }

    private void UpdateLocalState()
    {
        UpdateDieState();
        UpdateMoveState();
        UpdateAttackState();
        UpdateTransAniState();
        UpdatePlaySpeed();
    }

    //=====================================
    // Desc: player action state
    //=====================================
    private void UpdateDieState()
    {
        if (!isLocalPlayer) return;

        bDie = m_CombatProps.isDie();
    }

    private void UpdateMoveState()
    {
        if (bDie) return;
        if (m_CombatProps.isSleep()) return;
        if (!isLocalPlayer) return;

#if MOBILE_INPUT
        CrossPlatformInputManager.VirtualAxis virX = CrossPlatformInputManager.VirtualAxisReference("Virtual_X");
        CrossPlatformInputManager.VirtualAxis virY = CrossPlatformInputManager.VirtualAxisReference("Virtual_Y");
        float h = virX.GetValue;
        float v = virY.GetValue;
#else
        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        float v = CrossPlatformInputManager.GetAxis("Vertical");

        if (Globe.virtualButton)
        {
            if (h == 0 && v == 0)
            {
                CrossPlatformInputManager.VirtualAxis virX = CrossPlatformInputManager.VirtualAxisReference("Virtual_X");
                CrossPlatformInputManager.VirtualAxis virY = CrossPlatformInputManager.VirtualAxisReference("Virtual_Y");
                h = virX.GetValue;
                v = virY.GetValue;
            }
        }
#endif
        m_MoveDir = new Vector2(h, v);

        if (h == 0 && v == 0)
        {
            bRun = false;
        }
        else
        {
            bRun = true;
        }
    }

    protected virtual bool UpdateAttackState()
    {
        if (m_CombatProps.isSleep()) return false;
        if (!isLocalPlayer) return false;

        InputTest();

        if (bDie) return false;

#if MOBILE_INPUT
        skill_1_pressed = CrossPlatformInputManager.GetButtonDown("Virtual_Skill_1");
        skill_2_pressed = CrossPlatformInputManager.GetButtonDown("Virtual_Skill_2");
        skill_3_pressed = CrossPlatformInputManager.GetButtonDown("Virtual_Skill_3");
        skill_4_pressed = CrossPlatformInputManager.GetButtonDown("Virtual_Skill_4");     
#else
        skill_1_pressed = CrossPlatformInputManager.GetButtonDown("Skill_1");
        skill_2_pressed = CrossPlatformInputManager.GetButtonDown("Skill_2");
        skill_3_pressed = CrossPlatformInputManager.GetButtonDown("Skill_3");
        skill_4_pressed = CrossPlatformInputManager.GetButtonDown("Skill_4");

        if (Globe.virtualButton)
        {
            if (!skill_1_pressed) skill_1_pressed = CrossPlatformInputManager.GetButtonDown("Virtual_Skill_1");
            if (!skill_2_pressed) skill_2_pressed = CrossPlatformInputManager.GetButtonDown("Virtual_Skill_2");
            if (!skill_3_pressed) skill_3_pressed = CrossPlatformInputManager.GetButtonDown("Virtual_Skill_3");
            if (!skill_4_pressed) skill_4_pressed = CrossPlatformInputManager.GetButtonDown("Virtual_Skill_4");
        }
#endif
        return true;
    }

    private void InputTest()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            m_CombatProps.addDamage(AttackType.Frozen, 500);
        }
    }

    protected virtual bool UpdateTransAniState()
    {
        if (!isLocalPlayer) return false;

        AnimatorStateInfo asInfo = m_Animator.GetCurrentAnimatorStateInfo(0);

        if (asInfo.IsName("die"))
        {
            if (!bHasReqRelive && asInfo.normalizedTime >= 0.95f)
            {
                KBEngine.PropsEntity player = GetComponent<SyncPosRot>().entity;
                if (player != null)
                {
                    player.cellCall("reqRelive");
                }
                bHasReqRelive = true;
            }
        }
        return true;
    }

    private void UpdatePlaySpeed()
    {
        m_Animator.SetFloat("moveSpeed", m_CombatProps.m_MoveSpeed * 0.2f);
    }

    private void UpdateAnimateState()
    {
        if (isLocalPlayer)
        {
            bool change = false;
            if (bLastDie != bDie)
            {
                DieOrRelive();
                change = true;
            }

            if (bLastRun != bRun)
            {
                m_Animator.SetBool("run", bRun);
                bLastRun = bRun;
                change = true;
            }

            if (iLastAtk != iAtk)
            {
                m_Animator.SetInteger("atk", iAtk);
                iLastAtk = iAtk;
                change = true;
            }

            if (Globe.netMode && change)
            {
                aniSyncInfo["die"] = bDie;
                aniSyncInfo["run"] = bRun;
                aniSyncInfo["atk"] = iAtk;
                KBEngine.PropsEntity player = GetComponent<SyncPosRot>().entity;
                if (player != null)
                {
                    player.cellCall("reqSyncAniState", new object[] { aniSyncInfo });
                }
            }
        }
    }

    //=====================================
    // Desc: real performance
    //=====================================
    private bool MoveTurn(Vector2 moveDir)
    {
        if (moveDir != Vector2.zero)
        {
            Transform m_Cam = Camera.main.transform;
            Vector3 dir = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized * moveDir.y + m_Cam.right * moveDir.x;
            Quaternion quat = Quaternion.LookRotation(dir);
            if (Vector3.Distance(transform.eulerAngles, quat.eulerAngles) > 0.0004f)
            {
                gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, quat, 10f * Time.deltaTime);
            }
            else
            {
                gameObject.transform.rotation = quat;
            }
            //gameObject.transform.position += dir * m_CombatProps.m_MoveSpeed * Time.deltaTime;
            m_Rigidbody.velocity = new Vector3(m_CombatProps.m_MoveSpeed * dir.x, m_Rigidbody.velocity.y, m_CombatProps.m_MoveSpeed * dir.z);
            return true;
        }
        return false;
    }

    protected virtual bool DieOrRelive()
    {
        if (bDie)
        {
            bHasReqRelive = false;
            m_Animator.SetTrigger("die");
            GetComponent<Collider>().enabled = false;
            GetComponent<Rigidbody>().isKinematic = true;
        }
        else
        {
            m_Animator.SetTrigger("relive");
            GetComponent<Collider>().enabled = true;
            GetComponent<Rigidbody>().isKinematic = false;
        }
        bLastDie = bDie;
        return bDie;
    }

    //=====================================
    // Desc: try show performance,
    //       and return the result.
    //=====================================
    private bool CheckAttack()
    {
        bool bAtck = iAtk != 0;
        if(bAtck)
        {
            m_Rigidbody.velocity = new Vector3(0, m_Rigidbody.velocity.y, 0);
        }
        return bAtck;
    }

    private bool TryMoveTurn()
    {
        AnimatorStateInfo asInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
        AnimatorTransitionInfo atInfo = m_Animator.GetAnimatorTransitionInfo(0);
        if (atInfo.IsName("idle -> run") || asInfo.IsName("run"))
        {
            if(MoveTurn(m_MoveDir))
            {
                return true;
            }
        }

        return false;
    }

    //=====================================
    // Desc: update performance
    //=====================================
    private void UpdatePlayer()
    {
        //Die
        if (bDie) return;

        //Attack
        if (CheckAttack()) return;

        //Movement
        TryMoveTurn();

    }

    private void UpdateCamera()
    {
        if (!isLocalPlayer) return;

        float _fRotation = Input.GetAxis("RH") * m_TurnSpeed * Time.deltaTime;
        if (m_Invert)
            m_Angle -= _fRotation;
        else
            m_Angle += _fRotation;
        float _fAngle = Mathf.Deg2Rad * m_Angle;
        Vector3 _vXOZ = new Vector3(Mathf.Cos(_fAngle), -1.0f, Mathf.Sin(_fAngle));
        _vXOZ.y = -1.0f;

        Vector3 _pos = gameObject.transform.position - 7.0f * _vXOZ;
        m_Cam.transform.position = _pos;
        m_Cam.transform.LookAt(gameObject.transform);
    }

    //=====================================
    // Desc: non-local player sync func
    //====================================
    public void SyncDie(bool die)
    {
        if (!isLocalPlayer)
        {
            if (bDie != die)
            {
                bDie = die;
                DieOrRelive();
            }
        }
    }

    public void SyncRun(bool run)
    {
        if (!isLocalPlayer)
        {
            bRun = run;
            bLastRun = bRun;
            m_Animator.SetBool("run", bRun);
        }
    }

    public void SyncAtk(int atk)
    {
        if(!isLocalPlayer)
        {
            iAtk = atk;
            iLastAtk = iAtk;
            m_Animator.SetInteger("atk", iAtk);
        }
    }

}
