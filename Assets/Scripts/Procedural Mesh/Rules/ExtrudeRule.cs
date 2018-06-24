using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PMesh
{
	public class ExtrudeRule : BaseRule
	{
		public string mExtrudeLength = string.Empty;

		public override void SetVariables(params string[] aVariables)
		{
			mExtrudeLength = aVariables[0];
		}

		public override void Process(Shape aShape, ref List<Shape> aShapeList, ShuntingYard aExpressionParser)
		{
		}
	}
}
