using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Github.Knose1.MiniJam61.UI
{
	public class UIContainer : MonoBehaviour
	{
		[Flags]
		public enum ActionOnClose
		{
			unactivate  = 0 >> 0,
			unparent    = 0 >> 1,
			destroy     = 0 >> 2,
		}

		[Serializable]
		public class Layer
		{
			public GameObject screen;
			
			
			public ActionOnClose actionOnClose;

			public Layer(GameObject screen, ActionOnClose actionOnClose)
			{
				this.actionOnClose = actionOnClose;
				this.screen = screen;
			}

			public bool IsDestroy		=> (actionOnClose & ActionOnClose.destroy) == ActionOnClose.destroy;
			public bool IsUnparent		=> (actionOnClose & ActionOnClose.unparent) == ActionOnClose.unparent;
			public bool IsUnactivate	=> (actionOnClose & ActionOnClose.unactivate) == ActionOnClose.unactivate;

			public static implicit operator bool(Layer l) => l != null;
		}

		private const string LOG_PREFIX = "["+nameof(UIContainer)+"]";

		private static UIContainer _instance = null;
		public static UIContainer Instance => _instance = _instance ?? FindObjectOfType<UIContainer>();

		private List<Layer> _layers = new List<Layer>();

		private void Awake()
		{
			if (!_instance) _instance = this;
		}

		/// <summary>
		/// Add a screen on the UIContainer
		/// </summary>
		/// <param name="screen">The screen to add</param>
		/// <param name="overlay">If it's an overlay or not (if not, Close the old screen)</param>
		/// <param name="actionOnClose">Destroy this screen when closed</param>
		public void Add(GameObject screen, bool overlay = false, ActionOnClose actionOnClose = ActionOnClose.unparent)
		{
			screen.transform.SetParent(transform, false);
			screen.SetActive(false);//This is a patch for unity UI masks
			screen.SetActive(true); //This is a patch for unity UI masks

			if (_layers.Count == 0)
			{
				_layers.Add(new Layer(screen, actionOnClose));
				return;
			}

			if (overlay)
			{
				_layers.Add(new Layer(screen, actionOnClose));
				return;
			}

			Layer currentLayer = _layers[_layers.Count];
			CloseScreen(currentLayer);

			currentLayer.screen = screen;
			currentLayer.actionOnClose = actionOnClose;

		}

		public GameObject Close(GameObject gm)
		{
			for (int i = _layers.Count - 1; i >= 0; i--)
			{
				if (_layers[i].screen == gm)
					return Close(_layers[i]);
			}

			return null;
		}
		public GameObject Close(Layer layerToClose)
		{
			if (!_layers.Contains(layerToClose))
			{
				Debug.LogWarning(LOG_PREFIX + " The UI layer provided is not on the UIContainer " + gameObject.name);
				return null;
			}

			_layers.Remove(layerToClose);
			CloseScreen(layerToClose);

			return layerToClose.screen;
		}

		public GameObject Close()
		{
			if (_layers.Count == 0)
			{
				Debug.LogWarning(LOG_PREFIX + " There is no UI layer on " + gameObject.name);
				return null;
			}

			return Close(_layers[_layers.Count]);
		}

		private void CloseScreen(Layer currentLayer)
		{
			GameObject currentScreen = currentLayer.screen;
			if (currentLayer.IsDestroy) Destroy(currentScreen);
			else
			{
				if (currentLayer.IsUnactivate)
					currentScreen.SetActive(false);
				if (currentLayer.IsUnparent)
					currentScreen.transform.SetParent(null);
			}
		}

		private void CheckLayers()
		{
			for (int i = _layers.Count - 1; i >= 0; i--)
			{
				Layer layer = _layers[i];
				if (layer.screen && !layer.screen) _layers.RemoveAt(i);
				else if (layer.screen && layer.screen.transform.parent != this) _layers.RemoveAt(i);
				else if (layer.screen && !layer.screen.activeSelf) _layers.RemoveAt(i);
			}
		}

		// Rappel envoyé au graphique après une modification des enfants de Transform
		private void OnTransformChildrenChanged()
		{
			CheckLayers();
		}
	}
}