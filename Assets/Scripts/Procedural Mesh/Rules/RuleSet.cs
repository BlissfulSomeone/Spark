using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PMesh
{
	public class RuleSet
	{
		private static string UNNAMED = "Unnamed Ruleset";

		public string mName = UNNAMED;
		public List<BaseRule> mRules = new List<BaseRule>();
	}
}
