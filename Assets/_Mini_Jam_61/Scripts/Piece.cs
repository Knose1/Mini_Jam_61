using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Github.Knose1.MiniJam61
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(MeshRenderer))]
	public class Piece : MonoBehaviour
	{
		public enum PieceType
		{
			Player,
			Opponent
		}

		[SerializeField] protected List<string> m_colors = new List<string>();
		[SerializeField] protected Material m_material = default;
		[SerializeField] protected PieceType m_pieceType = default;
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

			MeshRenderer renderer = GetComponent<MeshRenderer>();
		}

		private void Update()
		{
			if (materialClone == null) return;

			materialClone.CopyPropertiesFromMaterial(m_material);

			Color c = default;
			switch (m_pieceType)
			{
				case PieceType.Player:
					c = m_settings.
					break;
				case PieceType.Opponent:
					break;
			}

			for (int i = 0; i < m_colors.Count; i++)
			{
				materialClone.SetColor(m_colors[i], scale);
			}

			renderer.material = materialClone;
		}
	}
}