using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PMesh
{
	public class ExtrudeRule : BaseRule
	{
		public string mExtrudeLength = string.Empty;

		public override void SetVariables(string[] aVariables, string aCommandLine)
		{
			mExtrudeLength = aVariables[0];
		}

		public override eRuleReply Process(Shape aShape, ref List<Shape> aShapeList, ShuntingYard aExpressionParser)
		{
			return eRuleReply.Success;
		}

		public override BaseRule DeepCopy()
		{
			ExtrudeRule copy = new ExtrudeRule();
			copy.mExtrudeLength = mExtrudeLength;
			return copy;
		}
	}
}
