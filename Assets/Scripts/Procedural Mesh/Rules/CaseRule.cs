using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PMesh
{
	public class CaseRule : BaseRule
	{
		private enum eCaseOperator
		{
			LESS,
			LESS_EQUAL,
			EQUAL,
			GREATER_EQUAL,
			GREATER,
		}

		private string mLHS;
		private eCaseOperator mOperator;
		private string mRHS;

		public override void SetVariables(string[] aVariables, string aCommandLine)
		{
			mLHS = aVariables[0];
			switch (aVariables[1])
			{
				case "<":
					mOperator = eCaseOperator.LESS;
					break;

				case "<=":
					mOperator = eCaseOperator.LESS_EQUAL;
					break;

				case "==":
					mOperator = eCaseOperator.EQUAL;
					break;

				case ">=":
					mOperator = eCaseOperator.GREATER_EQUAL;
					break;

				case ">":
					mOperator = eCaseOperator.GREATER;
					break;
			}
			mRHS = aVariables[2];
		}

		public override eRuleReply Process(Shape aShape, ref List<Shape> aShapeList, ShuntingYard aExpressionParser)
		{
			float lhs = aExpressionParser.Parse(mLHS);
			float rhs = aExpressionParser.Parse(mRHS);
			switch (mOperator)
			{
				case eCaseOperator.LESS:
					return (lhs < rhs ? eRuleReply.Success : eRuleReply.Failed);

				case eCaseOperator.LESS_EQUAL:
					return (lhs <= rhs ? eRuleReply.Success : eRuleReply.Failed);

				case eCaseOperator.EQUAL:
					return (lhs == rhs ? eRuleReply.Success : eRuleReply.Failed);

				case eCaseOperator.GREATER_EQUAL:
					return (lhs >= rhs ? eRuleReply.Success : eRuleReply.Failed);

				case eCaseOperator.GREATER:
					return (lhs > rhs ? eRuleReply.Success : eRuleReply.Failed);
			}
			return eRuleReply.Success;
		}

		public override BaseRule DeepCopy()
		{
			CaseRule copy = new CaseRule();
			copy.mLHS = mLHS;
			copy.mOperator = mOperator;
			copy.mRHS = mRHS;
			return copy;
		}
	}
}
