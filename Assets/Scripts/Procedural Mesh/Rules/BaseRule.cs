using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PMesh
{
	public enum eRuleReply
	{
		Success,
		Failed,
	}

	public class BaseRule
	{
		public virtual void SetVariables(string[] aVariables, string aCommandLine)
		{
		}
		
		public virtual eRuleReply Process(Shape aShape, ref List<Shape> aShapeList, ShuntingYard aExpressionParser)
		{
			return eRuleReply.Success;
		}

		public virtual BaseRule DeepCopy()
		{
			return (BaseRule)MemberwiseClone();
		}
	}
}
