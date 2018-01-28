using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SyncPosRot : MonoBehaviour{
    public bool isLocalPlayer = false;
    public bool isDynamicMode = false;
    public bool isLerpMotion = false;
    public KBEngine.PropsEntity entity = null;

	public Vector3 syncPos;
    public Vector3 syncEuler;
    public UInt32 spaceID;
    private bool isControlled = false;

	private float lerpRate;

	private Vector3 lastPos;
	private float threshold = 0.11f;

    private Vector3 _position;
    public Vector3 position
    {
        get
        {
            return _position;
        }

        set
        {
            _position = value;
            transform.position = value; 
        }
    }

    private Vector3 _euler;
    public Vector3 eulerAngles
    {
        get
        {
            return _euler;
        }
        set
        {
            rotation = Quaternion.Euler(value);         
        }
    }

    public Quaternion rotation
    {
        get
        {
            return Quaternion.Euler(_euler);
        }

        set
        {
            _euler = value.eulerAngles;
            transform.rotation = value; 
        }
    }

    public float speed
    {
        get
        {
            if (entity != null)
            {
                return entity.speed;
            }
            return 0;
        }
    }

    void Awake()
    {
        enabled = false;
    }

    void Start ()
	{
        RealSync(gameObject);
		lerpRate = 10;
        if(isDynamicMode)
        {
            ReqCtrlCollider reqCtrlScript = gameObject.AddComponent<ReqCtrlCollider>();
            reqCtrlScript.enabled = true;
        }
	}

    void Update ()
	{
        SyncPos();
        SyncRot();
	}

	void FixedUpdate () 
	{
        TransmitPos();
        TransmitRot();
    }

	void SyncPos ()
	{
        if (entity != null && entity.isControlled) return;

        //force sync consistent
        if (isDynamicMode)
        {
            position = transform.position;
        }

        if (!isLocalPlayer)
		{
            //simulate lerp moving
            if (isLerpMotion)
            {
                position = Vector3.Slerp(position, syncPos, Time.deltaTime * lerpRate);
                return;
            }

            //simulate player moving
            float dis = Vector3.Distance(syncPos, position);
            if (dis == 0f)
                return;
            else if (dis < threshold)
            {
                position = Vector3.Slerp(position, syncPos, Time.deltaTime * lerpRate);
                //position = syncPos;
            }
            else
            {
                Vector3 dirc = syncPos - position;
                //dirc.y = 0;
                Vector3 _direction = (dirc).normalized;
                _direction.Normalize();
                float deltaSpeed = speed * Time.deltaTime;
                if (dis > deltaSpeed)
                    position += _direction * deltaSpeed;
                else
                    position = Vector3.Slerp(position, syncPos, Time.deltaTime * lerpRate);
            }
        }
	}

    void SyncRot()
    {
        if (entity != null && entity.isControlled) return;

        //force sync consistent
        if (isDynamicMode)
        {
            rotation = transform.rotation;
        }

        if (!isLocalPlayer)
        {
            if (Vector3.Distance(transform.eulerAngles, syncEuler) > 0.0004f)
            {
                //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(syncEuler), 8f * Time.deltaTime);
                rotation = Quaternion.Slerp(rotation, Quaternion.Euler(syncEuler), 10f * Time.deltaTime);
            }
            else
            {
                rotation = Quaternion.Euler(syncEuler);
            }
        }
    }

	void TransmitPos ()
	{
        if (isDynamicMode)
        {
            //when change controller, sync the latest position
            if (isControlled != entity.isControlled)
            {
                isControlled = entity.isControlled;
                position = entity.position;
            }

            if (entity != null && entity.isControlled)
                entity.position = transform.position;
        }

        if (isLocalPlayer)
		{
            if (entity != null)
            {
                // [optimize]
                //if (Vector3.Distance(transform.position, entity.position) > threshold)
                //{
                //    entity.position = transform.position;
                // }
                entity.position = transform.position;
            }

        }
    }

    void TransmitRot()
    {
        if (isDynamicMode)
        {
            //when change controller, sync the latest position
            if (isControlled != entity.isControlled)
            {
                isControlled = entity.isControlled;
                transform.eulerAngles = entity.eulerAngles;
            }

            if (entity != null && entity.isControlled)
                entity.eulerAngles = transform.eulerAngles;
        }

        if (isLocalPlayer)
        {
            if(entity != null)
            {
                //entity.direction.z = transform.eulerAngles.y;
                entity.eulerAngles = transform.eulerAngles;
            }
        }
    }

    public void RealSyncPosition(GameObject obj)
    {
        if(obj != null)
        {
            position = obj.transform.position;
            syncPos = position;
        }
    }

    public void RealSyncRotation(GameObject obj)
    {
        if (obj != null)
        {
            eulerAngles = obj.transform.eulerAngles;
            syncEuler = eulerAngles;
        }
    }

    public void RealSync(GameObject obj)
    {
        RealSyncPosition(obj);
        RealSyncRotation(obj);
    }

    public void RealSyncPosition(KBEngine.PropsEntity entity)
    {
        if(entity != null)
        {
            position = entity.position;
            syncPos = position;
        }
    }

    public void RealSyncRotation(KBEngine.PropsEntity entity)
    {
        if(entity != null)
        {
            eulerAngles = entity.eulerAngles;
            syncEuler = eulerAngles;
        }
    }

    public void RealSync(KBEngine.PropsEntity entity)
    {
        RealSyncPosition(entity);
        RealSyncRotation(entity);
    }
}
