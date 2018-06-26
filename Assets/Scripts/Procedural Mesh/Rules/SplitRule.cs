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

		public override void SetVariables(string[] aVariables, string aCommandLine)
		{
			switch (aVariables[0])
			{
				case "x":
					mAxis = eSplitAxis.X;
					break;

				case "y":
					mAxis = eSplitAxis.Y;
					break;

				case "z":
					mAxis = eSplitAxis.Z;
					break;
			}
			mNumberOfSplits = aVariables[1];
			mNameOfChildren = aVariables[2];
		}

		public override eRuleReply Process(Shape aShape, ref List<Shape> aShapeList, ShuntingYard aExpressionParser)
		{
			bool isRelative = mNumberOfSplits[0] == '\'';
			float inputNumberOfSplits = aExpressionParser.Parse(mNumberOfSplits);
			float newSize = 0.0f;
			int numberOfSplits = 0;
			Vector3 axis = Vector3.zero;
			axis[(int)mAxis] = 1;
			if (isRelative)
			{
				mNumberOfSplits = mNumberOfSplits.Substring(1);
				newSize = aShape.mScope.mScale[(int)mAxis] / inputNumberOfSplits;
				numberOfSplits = (int)(inputNumberOfSplits);
			}
			else
			{
				newSize = inputNumberOfSplits;
				numberOfSplits = Mathf.FloorToInt(aShape.mScope.mScale[(int)mAxis] / inputNumberOfSplits);
			}

			for (int i = 0; i < numberOfSplits; ++i)
			{
				Shape splitShape = new Shape();
				splitShape.mName = mNameOfChildren;
				splitShape.mScope.mPosition = aShape.mScope.mPosition + axis * newSize * i;
				splitShape.mScope.mScale = aShape.mScope.mScale;
				splitShape.mScope.mScale[(int)mAxis] = newSize;
				splitShape.mScope.mColor = aShape.mScope.mColor;
				splitShape.mSplitData = new SplitAttributes { mSplitIndex = i, mSplitTotal = numberOfSplits };
				aShapeList.Add(splitShape);
				aShape.mScope.mChildren.Add(splitShape.mScope);
			}
			return eRuleReply.Success;
		}

		public override BaseRule DeepCopy()
		{
			SplitRule copy = new SplitRule();
			copy.mAxis = mAxis;
			copy.mNumberOfSplits = mNumberOfSplits;
			copy.mNameOfChildren = mNameOfChildren;
			return copy;
		}
	}
}
