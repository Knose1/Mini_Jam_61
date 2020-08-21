using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.Github.Knose1.MiniJam61.Settings;

namespace Com.Github.Knose1.MiniJam61.Game.Base
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(MeshRenderer))]
	public class Piece : MonoBehaviour
	{
		public enum PieceTeam
		{
			Player,
			Opponent
		}

		[SerializeField] protected List<string> m_colors = new List<string>();
		[SerializeField] protected Material m_material = default;
		[SerializeField] protected PieceTeam m_team = default;
		public PieceTeam Team 
		{
			get => m_team;
			set => m_team = value;
		}

		[SerializeField] protected ColorSettings m_settings = default;
		

		protected Material materialClone = null;
		new protected MeshRenderer renderer = null;

		private void OnValidate()
		{
			Start();
		}

		private void Start()
		{
			if (m_material != null)
				materialClone = new Material(m_material);

			renderer = GetComponent<MeshRenderer>();
		}

		private void Update()
		{
			if (materialClone == null) return;

			materialClone.CopyPropertiesFromMaterial(m_material);

			Color c = default;
			switch (m_team)
			{
				case PieceTeam.Player:
					c = m_settings.PlayerColor;
					break;
				case PieceTeam.Opponent:
					c = m_settings.OpponentColor;
					break;
			}

			for (int i = 0; i < m_colors.Count; i++)
			{
				materialClone.SetColor(m_colors[i], c);
			}

			renderer.material = materialClone;
		}
	}
}