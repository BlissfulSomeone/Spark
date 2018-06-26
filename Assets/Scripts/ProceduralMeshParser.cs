using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralMeshParser : MonoBehaviour
{
	private struct Command
	{
		public string name;
		public int arguments;
		public PMesh.BaseRule ruleObject;
	}

	private const string OPERATOR_SHAPE_START = "-->";
	private const string OPERATOR_SHAPE_END = "---";
	private const string OPERATOR_COMMAND_END = ";";
	private const string OPERATOR_ARGUMENTS_START = "(";
	private const string OPERATOR_ARGUMENTS_END = ")";
	private const string OPERATOR_ARGUMENTS_NEXT = ",";
	private const string OPERATOR_ASSIGN = "=";

	private static Dictionary<string, PMesh.RuleSet> mRuleSets = new Dictionary<string, PMesh.RuleSet>();
	private static PMesh.RuleSet mCurrentSet = null;
	private static Dictionary<string, float> mVariables = new Dictionary<string, float>();

	private static readonly Command[] mCommands = new Command[]
	{
		new Command { name = "extrude",		arguments = 1, ruleObject = new PMesh.ExtrudeRule() },
		new Command { name = "scale",		arguments = 3, ruleObject = new PMesh.ScaleRule() },
		new Command { name = "translate",	arguments = 3, ruleObject = new PMesh.TranslateRule() },
		new Command { name = "split",		arguments = 3, ruleObject = new PMesh.SplitRule() },
		new Command { name = "color",		arguments = 1, ruleObject = new PMesh.ColorRule() },
		new Command { name = "copy",		arguments = 1, ruleObject = new PMesh.CopyRule() },
		new Command { name = "case",        arguments = 3, ruleObject = new PMesh.CaseRule() },
	};
	
	public static void ParseInput(string aInput, string aVariables)
	{
		mRuleSets.Clear();
		mCurrentSet = null;
		mVariables.Clear();

		string[] variableBlocks = Split(aVariables, OPERATOR_COMMAND_END);
		for (int variableIndex = 0; variableIndex < variableBlocks.Length; ++variableIndex)
		{
			string[] variable = Split(variableBlocks[variableIndex], OPERATOR_ASSIGN);
			if (variable.Length == 2)
			{
				float variableValue = 0.0f;
				if (float.TryParse(variable[1].Trim(), out variableValue) == true)
				{
					mVariables.Add(variable[0].Trim(), variableValue);
				}
			}
		}
		
		string[] shapeBlocks = Split(aInput, OPERATOR_SHAPE_END);
		for (int shapeBlockIndex = 0; shapeBlockIndex < shapeBlocks.Length; ++shapeBlockIndex)
		{
			string[] shapeCommands = Split(shapeBlocks[shapeBlockIndex], OPERATOR_SHAPE_START);
			if (shapeCommands.Length != 2)
				continue;

			CreateNewSet(shapeCommands[0].Trim());

			string[] commands = Split(shapeCommands[1], OPERATOR_COMMAND_END);
			for (int commandIndex = 0; commandIndex < commands.Length; ++commandIndex)
			{
				string commandLine = commands[commandIndex].Trim();
				ProcessCommand(commandLine);
			}
		}

		FindObjectOfType<PMesh.ShapeBuilder>().BuildShape(mRuleSets, mVariables);
	}

	private static string[] Split(string aInput, string aSeparator)
	{
		return aInput.Split(new string[] { aSeparator }, System.StringSplitOptions.RemoveEmptyEntries);
	}

	private static void CreateNewSet(string aSetName)
	{
		if (mRuleSets.ContainsKey(aSetName) == true)
		{
			Debug.LogError("RuleSet \"" + aSetName + "\" already defined.");
			return;
		}

		mCurrentSet = new PMesh.RuleSet();
		mCurrentSet.mName = aSetName;
		mCurrentSet.mRules = new List<PMesh.BaseRule>();
		mRuleSets.Add(aSetName, mCurrentSet);
	}

	private static void ProcessCommand(string aCommandLine)
	{
		int argumentsStart = aCommandLine.IndexOf(OPERATOR_ARGUMENTS_START);
		string command = argumentsStart > 0 ? aCommandLine.Substring(0, argumentsStart) : aCommandLine;
		string[] arguments = GetArguments(aCommandLine);
		
		for (int i = 0; i < mCommands.Length; ++i)
		{
			if (mCommands[i].name == command && mCommands[i].arguments == arguments.Length)
			{
				PMesh.BaseRule rule = mCommands[i].ruleObject.DeepCopy();
				rule.SetVariables(arguments, aCommandLine);
				mCurrentSet.mRules.Add(rule);
			}
		}
	}

	private static string[] GetArguments(string aInput)
	{
		int start = aInput.IndexOf(OPERATOR_ARGUMENTS_START);
		bool hasEnd = aInput.Contains(OPERATOR_ARGUMENTS_END);
		if (start != -1 && hasEnd == true)
		{
			List<string> arguments = new List<string>();
			Stack<int> parenthesesStack = new Stack<int>();
			int lastCommaIndex = start + 1;
			for (int i = start; i < aInput.Length; ++i)
			{
				string input = aInput[i].ToString();
				switch (input)
				{
					case OPERATOR_ARGUMENTS_START:
						parenthesesStack.Push(i);
						break;

					case OPERATOR_ARGUMENTS_END:
						parenthesesStack.Pop();
						if (parenthesesStack.Count == 0)
						{
							arguments.Add(aInput.Substring(lastCommaIndex, i - lastCommaIndex).Trim());
							lastCommaIndex = i + 1;
						}
						break;

					case OPERATOR_ARGUMENTS_NEXT:
						if (parenthesesStack.Count == 1)
						{
							arguments.Add(aInput.Substring(lastCommaIndex, i - lastCommaIndex).Trim());
							lastCommaIndex = i + 1;
						}
						break;
				}
			}
			return arguments.ToArray();
		}
		return new string[] { };
	}
}
