using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.Github.Knose1.MiniJam61.Settings;
using System;

namespace Com.Github.Knose1.MiniJam61.Game.Base
{

	[ExecuteInEditMode]
	public abstract class Piece : MonoBehaviour
	{
		
		[SerializeField] protected List<string> m_colors = new List<string>();
		[SerializeField] protected Material m_material = default;
		[SerializeField] protected GameTeam m_team = default;
		public GameTeam Team 
		{
			get => m_team;
			set => m_team = value;
		}

		[SerializeField] protected ColorSettings m_settings = default;
		[SerializeField] protected MeshRenderer m_renderer = null;
		
		protected Material materialClone = null;

		private void OnValidate()
		{
			Start();
		}

		private void Start()
		{
			if (m_material != null)
				materialClone = new Material(m_material);

			if (m_team == GameTeam.Player)		transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
			if (m_team == GameTeam.Opponent)	transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
		}

		private void Update()
		{
			if (materialClone == null) return;

			materialClone.CopyPropertiesFromMaterial(m_material);

			Color c = default;
			switch (m_team)
			{
				case GameTeam.Opponent:
					c = m_settings.OpponentColor;
					break;
				case GameTeam.Player:
					c = m_settings.PlayerColor;
					break;
			}

			for (int i = 0; i < m_colors.Count; i++)
			{
				materialClone.SetColor(m_colors[i], c);
			}

			m_renderer.material = materialClone;
		}

		public abstract List<Vector2Int> GetMouvement(Grid grid);
	}
}