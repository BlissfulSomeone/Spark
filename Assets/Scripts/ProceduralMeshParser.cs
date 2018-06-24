using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralMeshParser : MonoBehaviour
{
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

		//string lmao = string.Empty;
		//foreach (string argument in arguments)
		//{
		//	lmao += argument + "; ";
		//}
		//Debug.Log(lmao);
		//return;

		if (command.Equals("extrude") == true && arguments.Length == 1)
		{
			PMesh.ExtrudeRule extrudeRule = new PMesh.ExtrudeRule();
			extrudeRule.mExtrudeLength = arguments[0];
			mCurrentSet.mRules.Add(extrudeRule);
		}
		else if (command.Equals("scale") == true && arguments.Length  == 3)
		{
			PMesh.ScaleRule scaleRule = new PMesh.ScaleRule();
			scaleRule.mScale[0] = arguments[0];
			scaleRule.mScale[1] = arguments[1];
			scaleRule.mScale[2] = arguments[2];
			mCurrentSet.mRules.Add(scaleRule);
		}
		else if (command.Equals("translate") == true && arguments.Length == 3)
		{
			PMesh.TranslateRule translateRule = new PMesh.TranslateRule();
			translateRule.mTranslation[0] = arguments[0];
			translateRule.mTranslation[1] = arguments[1];
			translateRule.mTranslation[2] = arguments[2];
			mCurrentSet.mRules.Add(translateRule);
		}
		else if (command.Equals("copy") == true && arguments.Length == 1)
		{
			PMesh.CopyRule copyRule = new PMesh.CopyRule();
			copyRule.mNameOfChild = arguments[0];
			mCurrentSet.mRules.Add(copyRule);
		}
		else if (command.Equals("split") == true && arguments.Length == 3)
		{
			PMesh.SplitRule splitRule = new PMesh.SplitRule();
			if (arguments[0].Trim().ToLower().Equals("x") == true)
				splitRule.mAxis = PMesh.SplitRule.eSplitAxis.X;
			else if (arguments[0].Trim().ToLower().Equals("y") == true)
				splitRule.mAxis = PMesh.SplitRule.eSplitAxis.Y;
			else if (arguments[0].Trim().ToLower().Equals("z") == true)
				splitRule.mAxis = PMesh.SplitRule.eSplitAxis.Z;
			else
				return;
				
			splitRule.mNumberOfSplits = arguments[1];
			splitRule.mNameOfChildren = arguments[2];
			mCurrentSet.mRules.Add(splitRule);
		}
		else if (command.Equals("color") == true && arguments.Length == 1)
		{
			PMesh.ColorRule colorRule = new PMesh.ColorRule();
			colorRule.mHexColor = arguments[0];
			mCurrentSet.mRules.Add(colorRule);
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
		//int argumentsStart = aInput.IndexOf(OPERATOR_ARGUMENTS_START);
		//int argumentsEnd = aInput.IndexOf(OPERATOR_ARGUMENTS_END);
		//if (argumentsStart > 0 && argumentsEnd > argumentsStart)
		//{
		//	string argumentsString = aInput.Substring(argumentsStart + 1, argumentsEnd - argumentsStart - 1);
		//	string[] arguments = Split(argumentsString, OPERATOR_NEXT_ARGUMENT);
		//	return arguments;
		//}
		//return new string[] { };
	}
}
