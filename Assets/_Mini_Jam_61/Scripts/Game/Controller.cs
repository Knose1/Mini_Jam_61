using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Com.Github.Knose1.MiniJam61.Game
{
	public class Controller : ScriptableObject
	{
		[SerializeField] protected string m_mouseX = "Mouse X";
		[SerializeField] protected string m_mouseY = "Mouse Y";
		[SerializeField] protected string m_mouseLeft = "Fire1";
		[SerializeField] protected string m_mouseRight = "Fire2";
		[SerializeField] protected KeyCode m_escape = KeyCode.Escape;

		public Vector2 MouseMove		{ get; protected set; } = default;
		public bool MouseLeftPressed	{ get; protected set; } = default;
		public bool MouseLeftClick		{ get; protected set; } = default;
		public bool MouseLeftDown		{ get; protected set; } = default;

		public bool MouseRightPressed	{ get; protected set; } = default;
		public bool MouseRightClick		{ get; protected set; } = default;
		public bool MouseRightDown		{ get; protected set; } = default;

		public bool EscapePressed		{ get; protected set; } = default;
		public bool EscapeUp			{ get; protected set; } = default;
		public bool EscapeDown			{ get; protected set; } = default;

		public void UpdateControles()
		{
			MouseMove = new Vector2(
				x: Input.GetAxis(m_mouseX),
				y: Input.GetAxis(m_mouseY)
			);

			MouseLeftPressed = Input.GetButton(m_mouseLeft);
			MouseLeftClick = Input.GetButtonUp(m_mouseLeft);
			MouseLeftDown = Input.GetButtonDown(m_mouseLeft);
			
			MouseRightPressed = Input.GetButton(m_mouseRight);
			MouseRightClick = Input.GetButtonUp(m_mouseRight);
			MouseRightDown = Input.GetButtonDown(m_mouseRight);

			EscapePressed = Input.GetKey(m_escape);
			EscapeUp = Input.GetKeyUp(m_escape);
			EscapeDown = Input.GetKeyDown(m_escape);
		}
	}
}
