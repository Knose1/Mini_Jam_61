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
		private const int ADD_RAY_DISTANCE = 10;
		[SerializeField] private Controller m_controller = null;
		[SerializeField] protected Camera m_camera;
		[SerializeField] protected Transform m_cameraRotationUp;
		[SerializeField] protected Transform m_cameraRotationLeft;
		[SerializeField] protected float m_cameraSpeed = 20;

		[SerializeField] protected string m_mouseX = "Mouse X";
		[SerializeField] protected string m_mouseY = "Mouse Y";
		[SerializeField] protected string m_mouseLeftClick = "Fire1";
		[SerializeField] protected string m_mouseRightClick = "Fire2";

		private Action doAction;

		[SerializeField] protected Vector2 rotation = default;
		[SerializeField] protected Vector2 m_minRotation = default;
		[SerializeField] protected Vector2 m_maxRotation = default;

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
			if (Input.GetButtonUp(m_mouseLeftClick))
			{
				Ray();
			}
			else if (Input.GetButton(m_mouseRightClick))
			{
				Rotate();
			}

		}

		public void SetStateVoid() => doAction = null;
		

		private void Rotate()
		{
			float speed = Time.deltaTime * m_cameraSpeed;
			rotation.x += Input.GetAxis(m_mouseX) * speed;
			rotation.y += Input.GetAxis(m_mouseY) * speed;

			rotation.x %= 360;
			rotation.y %= 360;

			rotation.x = Mathf.Clamp(rotation.x, m_minRotation.x, m_maxRotation.x);
			rotation.y = Mathf.Clamp(rotation.y, m_minRotation.y, m_maxRotation.y);

			m_cameraRotationUp.localRotation = Quaternion.AngleAxis(rotation.x, Vector3.up);
			m_cameraRotationLeft.localRotation = Quaternion.AngleAxis(rotation.y, Vector3.left);
		}

		private void Ray()
		{
			float cameraRadius = -m_camera.transform.localPosition.z;
			Physics.Raycast(m_camera.ScreenPointToRay(Input.mousePosition), ADD_RAY_DISTANCE + cameraRadius);
		}
	}
}
