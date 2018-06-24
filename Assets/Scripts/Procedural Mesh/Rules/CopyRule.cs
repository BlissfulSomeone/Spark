using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PMesh
{
	public class CopyRule : BaseRule
	{
		public string mNameOfChild = string.Empty;

		public override void SetVariables(params string[] aVariables)
		{
			mNameOfChild = aVariables[0];
		}

		public override void Process(Shape aShape, ref List<Shape> aShapeList, ShuntingYard aExpressionParser)
		{
			Shape copyShape = new Shape();
			copyShape.mName = mNameOfChild;
			copyShape.mScope.mPosition = aShape.mScope.mPosition;
			copyShape.mScope.mScale = aShape.mScope.mScale;
			copyShape.mScope.mColor = aShape.mScope.mColor;
			aShapeList.Add(copyShape);
			aShape.mScope.mChildren.Add(copyShape.mScope);
		}
	}
}
