using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Resource.Utils;
using Resource;

namespace Architect {

	public class ArchitectManager : Singleton<ArchitectManager> {
		[SerializeField]
		private InputManager inputManager;
		[SerializeField]
		private List<string> definitions;

		#region Getters & Setters

		#endregion

		#region Utility Functions
		public void Reset() {

		}

		public void Clear() {

		}
		#endregion

	}

}
