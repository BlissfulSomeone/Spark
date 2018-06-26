using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PMesh
{
	public class TranslateRule : BaseRule
	{
		public string[] mTranslation = new string[3] { string.Empty, string.Empty, string.Empty };

		public override void SetVariables(string[] aVariables, string aCommandLine)
		{
			mTranslation[0] = aVariables[0];
			mTranslation[1] = aVariables[1];
			mTranslation[2] = aVariables[2];
		}

		public override eRuleReply Process(Shape aShape, ref List<Shape> aShapeList, ShuntingYard aExpressionParser)
		{
			float moveX = aExpressionParser.Parse(mTranslation[0]);
			float moveY = aExpressionParser.Parse(mTranslation[1]);
			float moveZ = aExpressionParser.Parse(mTranslation[2]);
			aShape.mScope.mPosition += new Vector3(moveX, moveY, moveZ);
			return eRuleReply.Success;
		}

		public override BaseRule DeepCopy()
		{
			TranslateRule copy = new TranslateRule();
			copy.mTranslation[0] = mTranslation[0];
			copy.mTranslation[1] = mTranslation[1];
			copy.mTranslation[2] = mTranslation[2];
			return copy;
		}
	}
}
