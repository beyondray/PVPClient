using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [HideInInspector] public GameObject master = null;
    [HideInInspector] public float damage = 200.0f;
    [HideInInspector] public float speed = 20.0f;
    [HideInInspector] public float life = 3.0f;

    // Update is called once per frame
    void Update()
    {
        life -= Time.deltaTime;
        if (life < 0.0f)
        {
            Safe_Destroy();
        }

        //movement
        gameObject.transform.position += gameObject.transform.forward.normalized * speed * Time.deltaTime;
    }

    public void Safe_Destroy()
    {
        if (Globe.netMode)
        {
            GetComponent<SyncPosRot>().entity.renderObj = null;
            GetComponent<SyncPosRot>().entity.cellCall("reqDestroySelf");
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != master)
        {
            if(other.gameObject.tag == "nonLocalPlayer" &&
                master.gameObject.tag == "localPlayer")
            {
                SyncPosRot syncScript = gameObject.GetComponent<SyncPosRot>();
                KBEngine.Arrow kbeArrow = syncScript.entity as KBEngine.Arrow;
                CombatProps cp = other.gameObject.GetComponent<CombatProps>();
                if(cp != null)
                {
                    cp.addDamage((AttackType)kbeArrow.attackType, damage);
                }
            }
            Safe_Destroy();
        }
    }
}