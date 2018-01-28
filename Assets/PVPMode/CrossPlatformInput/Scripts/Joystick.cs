using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityStandardAssets.CrossPlatformInput
{
	public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
	{
		public enum AxisOption
		{
			// Options for which axes to use
			Both, // Use both
			OnlyHorizontal, // Only horizontal
			OnlyVertical // Only vertical
		}
        public GameObject parentObj;
        public float radiusScale = 1f;
        public int radius { get; set; }
		public AxisOption axesToUse = AxisOption.Both; // The options for the axes that the still will use
		public string horizontalAxisName = "Virtual_X"; // The name given to the horizontal axis for the cross platform input
		public string verticalAxisName = "Virtual_Y"; // The name given to the vertical axis for the cross platform input

		Vector3 m_StartPos;
		bool m_UseX; // Toggle for using the x axis
		bool m_UseY; // Toggle for using the Y axis
		CrossPlatformInputManager.VirtualAxis m_HorizontalVirtualAxis; // Reference to the joystick in the cross platform input
		CrossPlatformInputManager.VirtualAxis m_VerticalVirtualAxis; // Reference to the joystick in the cross platform input

        void Start()
        {
            m_StartPos = transform.position;
            RectTransform parTrans = parentObj.GetComponent<RectTransform>();
            float scale = parTrans.lossyScale.x * radiusScale;
            radius = (int)(parTrans.rect.width * scale) / 2;
        }

		void OnEnable()
		{
			m_StartPos = transform.position;
			CreateVirtualAxes();
		}

		void UpdateVirtualAxes(Vector3 value)
		{
			var delta = value - m_StartPos;
            if(delta.magnitude != 0)
			    delta /= delta.magnitude;

			if (m_UseX)
			{
				m_HorizontalVirtualAxis.Update(delta.x);
			}

			if (m_UseY)
			{
				m_VerticalVirtualAxis.Update(delta.y);
			}
		}

		void CreateVirtualAxes()
		{
			// set axes to use
			m_UseX = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyHorizontal);
			m_UseY = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyVertical);

			// create new axes based on axes to use
			if (m_UseX)
			{
				m_HorizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(horizontalAxisName);
				CrossPlatformInputManager.RegisterVirtualAxis(m_HorizontalVirtualAxis);
			}
			if (m_UseY)
			{
				m_VerticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(verticalAxisName);
				CrossPlatformInputManager.RegisterVirtualAxis(m_VerticalVirtualAxis);
			}
		}


		public void OnDrag(PointerEventData data)
		{
            Move(data);
		}


		public void OnPointerUp(PointerEventData data)
		{
			transform.position = m_StartPos;
			UpdateVirtualAxes(m_StartPos);
		}


		public void OnPointerDown(PointerEventData data) 
        {
            Move(data);
        }

        public void Move(PointerEventData data)
        {
            int deltaX = m_UseX ? (int)(data.position.x - m_StartPos.x) : 0;
            int deltaY = m_UseY ? (int)(data.position.y - m_StartPos.y) : 0;
            float ratio = Mathf.Sqrt(deltaX * deltaX + deltaY * deltaY) / radius;

            if (ratio > 1.0f)
            {
                deltaX = (int)(deltaX / ratio);
                deltaY = (int)(deltaY / ratio);
            }

            transform.position = new Vector3(m_StartPos.x + deltaX, m_StartPos.y + deltaY, m_StartPos.z);
            UpdateVirtualAxes(transform.position);
        }

		void OnDisable()
		{
			// remove the joysticks from the cross platform input
			if (m_UseX)
			{
				m_HorizontalVirtualAxis.Remove();
			}
			if (m_UseY)
			{
				m_VerticalVirtualAxis.Remove();
			}
		}
	}
}