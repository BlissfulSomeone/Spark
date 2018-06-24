using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PMesh
{
	public class ColorRule : BaseRule
	{
		public string mHexColor = "ffffff";

		public override void Process(Shape aShape, ref List<Shape> aShapeList, ShuntingYard aExpressionParser)
		{
			ColorUtility.TryParseHtmlString(mHexColor, out aShape.mScope.mColor);
		}
	}
}
