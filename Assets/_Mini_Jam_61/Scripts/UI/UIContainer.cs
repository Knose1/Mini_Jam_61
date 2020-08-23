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
			unactivate  = 1 << 0,
			unparent    = 1 << 1,
			destroy     = 1 << 2,
		}

		[Serializable]
		public class Layer
		{
			private UIContainer _container;
			private GameObject _screen;
			public GameObject Screen
			{
				get => _screen; 
				set
				{
					if (_screen && _screen != value) CloseScreen();

					value.transform.SetParent(_container.transform, false);
					value.SetActive(false);//This is a patch for unity UI masks
					value.SetActive(true); //This is a patch for unity UI masks

					_screen = value;
				}
			}


			public ActionOnClose actionOnClose;

			public Layer(UIContainer container, GameObject screen, ActionOnClose actionOnClose)
			{
				this._container = container;
				this.actionOnClose = actionOnClose;
				this.Screen = screen;
			}

			public bool IsDestroy => (actionOnClose & ActionOnClose.destroy) == ActionOnClose.destroy;
			public bool IsUnparent => (actionOnClose & ActionOnClose.unparent) == ActionOnClose.unparent;
			public bool IsUnactivate => (actionOnClose & ActionOnClose.unactivate) == ActionOnClose.unactivate;

			public static implicit operator bool(Layer l) => l != null;

			public void CloseScreen()
			{
				GameObject currentScreen = Screen;
				if (IsDestroy) Destroy(currentScreen);
				else
				{
					if (IsUnactivate)
						currentScreen.SetActive(false);
					if (IsUnparent)
						currentScreen.transform.SetParent(null);
				}
			}
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
		public Layer Add(GameObject screen, bool overlay = false, ActionOnClose actionOnClose = ActionOnClose.unparent)
		{
			Layer layerToReturn;
			if (_layers.Count == 0 || overlay)
			{
				layerToReturn = new Layer(this, screen, actionOnClose);
				_layers.Add(layerToReturn);
				return layerToReturn;
			}

			layerToReturn = _layers[_layers.Count];
			layerToReturn.Screen = screen;
			layerToReturn.actionOnClose = actionOnClose;

			return layerToReturn;
		}

		public GameObject Close(GameObject gm)
		{
			for (int i = _layers.Count - 1; i >= 0; i--)
			{
				if (_layers[i].Screen == gm)
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
			layerToClose.CloseScreen();

			return layerToClose.Screen;
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

		private void CheckLayers()
		{
			for (int i = _layers.Count - 1; i >= 0; i--)
			{
				Layer layer = _layers[i];
				if (layer.Screen && !layer.Screen) _layers.RemoveAt(i);
				else if (layer.Screen && layer.Screen.transform.parent != this) _layers.RemoveAt(i);
				else if (layer.Screen && !layer.Screen.activeSelf) _layers.RemoveAt(i);
			}
		}

		// Rappel envoyé au graphique après une modification des enfants de Transform
		private void OnTransformChildrenChanged()
		{
			CheckLayers();
		}
	}
}