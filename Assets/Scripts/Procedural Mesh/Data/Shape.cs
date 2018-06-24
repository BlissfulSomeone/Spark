using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PMesh
{
	public class Shape
	{
		public Scope mScope = null;
		public string mName = string.Empty;
		public SplitAttributes mSplitData = null;

		public Shape()
		{
			mScope = new Scope();
			mScope.mShape = this;
		}
	}
}
