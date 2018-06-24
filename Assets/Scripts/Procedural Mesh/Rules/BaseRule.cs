﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PMesh
{
	public class BaseRule
	{
		public virtual void Process(Shape aShape, ref List<Shape> aShapeList, ShuntingYard aExpressionParser)
		{
		}
	}
}
