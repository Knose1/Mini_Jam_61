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

		private const int ADD_RAY_DISTANCE = 100;
		[SerializeField] private Controller m_controller = null;
		[SerializeField] protected Camera m_camera;
		[SerializeField] protected Transform m_cameraRotationUp;
		[SerializeField] protected Transform m_cameraRotationLeft;

		[Header("Rotation")]
		[SerializeField] protected float m_cameraSpeed = 20;
		[SerializeField] protected Vector2 rotation = default;
		[SerializeField] protected Vector2 m_minRotation = default;
		[SerializeField] protected Vector2 m_maxRotation = default;


		[Header("Zoom")]
		[SerializeField] protected float m_cameraZoomSpeed = 10;
		[SerializeField] protected float m_cameraZoom = 2;
		[SerializeField] protected float m_cameraMaxZoom = 2;
		[SerializeField] protected float m_cameraMinZoom = 0.1f;

		[SerializeField] protected LayerMask m_layerMask = default;

		private Action doAction;
		public Controller Controller => m_controller;

		public bool IsStateVoid => doAction == null;

		private void OnValidate()
		{
			Rotate();
			Scroll();
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
				float speed = Time.deltaTime * m_cameraSpeed;
				rotation += m_controller.MouseMove * speed;
				Rotate();
			}

			float scroll = Input.mouseScrollDelta.y;
			if (scroll != 0)
			{
				m_cameraZoom += scroll * Time.deltaTime * m_cameraZoomSpeed;
				Scroll();
			}
		}

		private void Scroll()
		{
			m_cameraZoom = Mathf.Clamp(m_cameraZoom, m_cameraMinZoom, m_cameraMaxZoom);
			m_camera.transform.localPosition = new Vector3(0, 0, -m_cameraZoom);
		}

		public void SetStateVoid() => doAction = null;
		

		private void Rotate()
		{
			rotation.x %= 360;
			rotation.y %= 360;

			rotation.x = Mathf.Clamp(rotation.x, m_minRotation.x, m_maxRotation.x);
			rotation.y = Mathf.Clamp(rotation.y, m_minRotation.y, m_maxRotation.y);

			m_cameraRotationUp.localRotation = Quaternion.AngleAxis(rotation.x, Vector3.up);
			m_cameraRotationLeft.localRotation = Quaternion.AngleAxis(rotation.y, Vector3.left);
		}

		private RaycastHit? Ray()
		{
			Ray ray = m_camera.ScreenPointToRay(Input.mousePosition);

			float cameraRadius = -m_camera.transform.localPosition.z;
			float distance = ADD_RAY_DISTANCE + cameraRadius;

			Debug.DrawRay(ray.origin, ray.direction * distance, Color.red, 2);
			if (Physics.Raycast(ray, out RaycastHit hitInfo , distance, m_layerMask)) 
			{
				return hitInfo;
			}

			return null;
		}
	}
}
