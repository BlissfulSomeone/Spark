﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PMesh
{
	public class ScaleRule : BaseRule
	{
		public string[] mScale = new string[3] { string.Empty, string.Empty, string.Empty };

		public override void SetVariables(string[] aVariables, string aCommandLine)
		{
			mScale[0] = aVariables[0];
			mScale[1] = aVariables[1];
			mScale[2] = aVariables[2];
		}

		public override eRuleReply Process(Shape aShape, ref List<Shape> aShapeList, ShuntingYard aExpressionParser)
		{
			float[] scale = new float[3] { 0.0f, 0.0f, 0.0f };
			for (int i = 0; i < 3; ++i)
			{
				if (mScale[i][0] == '\'')
					scale[i] = aShape.mScope.mScale[i] * aExpressionParser.Parse(mScale[i].Substring(1));
				else
					scale[i] = aExpressionParser.Parse(mScale[i]);
			}
			aShape.mScope.mScale = new Vector3(scale[0], scale[1], scale[2]);
			return eRuleReply.Success;
		}

		public override BaseRule DeepCopy()
		{
			ScaleRule copy = new ScaleRule();
			copy.mScale[0] = mScale[0];
			copy.mScale[1] = mScale[1];
			copy.mScale[2] = mScale[2];
			return copy;
		}
	}
}
