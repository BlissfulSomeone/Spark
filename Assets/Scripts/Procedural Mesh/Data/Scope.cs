using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PMesh
{
	public class Scope
	{
		public Shape mShape = null;
		public Vector3 mPosition = Vector3.zero;
		public Vector3 mScale = Vector3.one;
		public Color mColor = Color.white;
		public List<Scope> mChildren = new List<Scope>();
	}
}
