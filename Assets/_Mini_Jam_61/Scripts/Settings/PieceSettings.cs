using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Com.Github.Knose1.MiniJam61.Settings
{
	[CreateAssetMenu(fileName = nameof(PieceSettings), menuName = nameof(MiniJam61) + "/" + nameof(PieceSettings))]
	public class PieceSettings : ScriptableObject
	{
		[SerializeField] protected float m_octahedronCost = default;
		public float OctahedronCost => m_octahedronCost;

		[SerializeField] protected float m_triangleCost = default;
		public float TriangleCost => m_triangleCost;

		[SerializeField] protected float m_cubeCost = default;
		public float CubeCost => m_cubeCost;



		[SerializeField] protected float m_octahedronGivesMultiple = default;
		public float OctahedronGivesMultiple => m_octahedronGivesMultiple;

		[SerializeField] protected float m_triangleGivesMultiple = default;
		public float TriangleGivesMultiple => m_triangleGivesMultiple;

		[SerializeField] protected float m_cubeGivesMultiple = default;
		public float CubeGivesMultiple => m_cubeGivesMultiple;



		[SerializeField] protected float m_octahedronGivesMax = default;
		public float OctahedronGivesMax => m_octahedronGivesMax;

		[SerializeField] protected float m_triangleGivesMax = default;
		public float TriangleGivesMax => m_triangleGivesMax;

		[SerializeField] protected float m_cubeGivesMax = default;
		public float CubeGivesMax => m_cubeGivesMax;
	}
}
