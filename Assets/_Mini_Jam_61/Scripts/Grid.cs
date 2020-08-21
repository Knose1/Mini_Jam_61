using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Github.Knose1.MiniJam61
{
	public class Grid : MonoBehaviour
	{
		[SerializeField] protected Vector2Int size;

		private void OnValidate()
		{
			transform.localScale = new Vector3(size.x, (size.x + size.y)/2f, size.y);
			transform.position = new Vector3(size.x - 1f, 1f, size.y - 1f) / -2;
		}

		private void Start()
		{

		}

		private void Update()
		{

		}
	}
}
