using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PMesh
{
	public class ColorRule : BaseRule
	{
		public string mHexColor = "ffffff";

		public override void SetVariables(string[] aVariables, string aCommandLine)
		{
			mHexColor = aVariables[0];
		}

		public override eRuleReply Process(Shape aShape, ref List<Shape> aShapeList, ShuntingYard aExpressionParser)
		{
			ColorUtility.TryParseHtmlString(mHexColor, out aShape.mScope.mColor);
			return eRuleReply.Success;
		}

		public override BaseRule DeepCopy()
		{
			ColorRule copy = new ColorRule();
			copy.mHexColor = mHexColor;
			return copy;
		}
	}
}
