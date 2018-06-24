using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PMesh
{
	public class SplitRule : BaseRule
	{ 
		public enum eSplitAxis
		{
			X = 0,
			Y = 1,
			Z = 2,
		}

		public eSplitAxis mAxis = eSplitAxis.X;
		public string mNumberOfSplits = string.Empty;
		public string mNameOfChildren = string.Empty;
	}
}
