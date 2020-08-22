using Com.Github.Knose1.MiniJam61.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Com.Github.Knose1.MiniJam61.Game
{
	public class PlayerCamera : MonoBehaviour
	{
		public event Action<RaycastHit> OnRay;

		private const int ADD_RAY_DISTANCE = 10;
		[SerializeField] private Controller m_controller = null;
		[SerializeField] protected Camera m_camera;
		[SerializeField] protected Transform m_cameraRotationUp;
		[SerializeField] protected Transform m_cameraRotationLeft;
		[SerializeField] protected float m_cameraSpeed = 20;

		private Action doAction;

		[SerializeField] protected Vector2 rotation = default;
		[SerializeField] protected Vector2 m_minRotation = default;
		[SerializeField] protected Vector2 m_maxRotation = default;

		[SerializeField] protected LayerMask m_layerMask = default;

		public Controller Controller => m_controller;

		private void OnValidate()
		{
			Rotate();
		}

		public void ManualUpdate()
		{
			doAction?.Invoke();
		}

		public void SetStateNormal() => doAction = DoActionNormal;
		private void DoActionNormal()
		{
			if (m_controller.MouseLeftClick)
			{
				RaycastHit? hit = Ray();
				if (hit.HasValue)
				{
					OnRay(hit.Value);
				}
			}
			else if (m_controller.MouseRightPressed)
			{
				Rotate();
			}

		}

		public void SetStateVoid() => doAction = null;
		

		private void Rotate()
		{
			float speed = Time.deltaTime * m_cameraSpeed;
			rotation += m_controller.MouseMove * speed;

			rotation.x %= 360;
			rotation.y %= 360;

			rotation.x = Mathf.Clamp(rotation.x, m_minRotation.x, m_maxRotation.x);
			rotation.y = Mathf.Clamp(rotation.y, m_minRotation.y, m_maxRotation.y);

			m_cameraRotationUp.localRotation = Quaternion.AngleAxis(rotation.x, Vector3.up);
			m_cameraRotationLeft.localRotation = Quaternion.AngleAxis(rotation.y, Vector3.left);
		}

		private RaycastHit? Ray()
		{
			float cameraRadius = -m_camera.transform.localPosition.z;
			if (Physics.Raycast(m_camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo , ADD_RAY_DISTANCE + cameraRadius, m_layerMask)) 
			{
				return hitInfo;
			}

			return null;
		}
	}
}
