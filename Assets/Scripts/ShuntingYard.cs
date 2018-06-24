using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShuntingYard
{
	private enum eTokenType
	{
		NONE,
		NUMBER,
		OPERATOR,
		PARENTHESIS,
		FUNCTION,
		FUNCTION_DIVIDER,
	}
	
	private class Token
	{
		public eTokenType mType = eTokenType.NONE;
		public string mOperator = string.Empty;
		public float mValue = 0.0f;
		public int mPrecedence = 0;
		public bool mRightToLeftAssociativity = false;

		public Token ShallowCopy()
		{
			return (Token)MemberwiseClone();
		}
	}

	private readonly Dictionary<string, Token> mOperators = new Dictionary<string, Token>()
	{
		{ "+", new Token { mType = eTokenType.OPERATOR, mOperator = "+", mPrecedence = 2 } },
		{ "-", new Token { mType = eTokenType.OPERATOR, mOperator = "-", mPrecedence = 2 } },
		{ "*", new Token { mType = eTokenType.OPERATOR, mOperator = "*", mPrecedence = 3 } },
		{ "/", new Token { mType = eTokenType.OPERATOR, mOperator = "/", mPrecedence = 3 } },
		{ "^", new Token { mType = eTokenType.OPERATOR, mOperator = "^", mPrecedence = 4, mRightToLeftAssociativity = true } },
		{ "(", new Token { mType = eTokenType.PARENTHESIS, mOperator = "(", mPrecedence = 1 } },
		{ ")", new Token { mType = eTokenType.PARENTHESIS, mOperator = ")", mPrecedence = 1 } },
		{ "max", new Token { mType = eTokenType.FUNCTION, mOperator = "max" } },
		{ "min", new Token { mType = eTokenType.FUNCTION, mOperator = "min" } },
		{ "rand", new Token { mType = eTokenType.FUNCTION, mOperator = "rand" } },
		{ "cos", new Token { mType = eTokenType.FUNCTION, mOperator = "cos" } },
		{ "sin", new Token { mType = eTokenType.FUNCTION, mOperator = "sin" } },
		{ "tan", new Token { mType = eTokenType.FUNCTION, mOperator = "tan" } },
		{ ",", new Token { mType = eTokenType.FUNCTION_DIVIDER, mOperator = "," } },
	};

	private bool mDebugMode;
	private Dictionary<string, float> mVariables = new Dictionary<string, float>();

	public ShuntingYard()
	{
		mDebugMode = false;
	}

	public ShuntingYard(bool aDebugMode)
	{
		mDebugMode = aDebugMode;
	}

	public void SetVariables(Dictionary<string, float> aVariables)
	{
		mVariables = aVariables;
	}
	
	public float Parse(string aInput, string aVariables)
	{
		// Get variables
		Dictionary<string, float> variables = new Dictionary<string, float>();
		string[] variableStrings = aVariables.Split(';');
		float value = 0.0f;
		foreach(string variableInput in variableStrings)
		{
			string[] variable = variableInput.Split('=');
			if (variable.Length == 2)
			{
				if (float.TryParse(variable[1].Trim(), out value) == true)
				{ 
					variables.Add(variable[0].Trim(), value);
				}
			}
		}
		if (mDebugMode == true)
		{
			string variablesOutput = "Variables: ";
			foreach(KeyValuePair<string, float> variable in variables)
			{
				variablesOutput += variable.Key + "=" + variable.Value.ToString() + "; ";
			}
			Debug.Log(variablesOutput);
		}

		return Parse(aInput, variables);
	}

	public float Parse(string aInput)
	{
		return Parse(aInput, mVariables);
	}

	private float Parse(string aInput, Dictionary<string, float> aVariables)
	{
		// Get tokens
		List<Token> tokens = GetTokens(aInput, aVariables);
		if (mDebugMode == true)
		{
			string tokensOutput = "Tokens: ";
			foreach (Token token in tokens)
			{
				tokensOutput += (token.mType == eTokenType.NUMBER ? token.mValue.ToString() : token.mOperator) + "; ";
			}
			Debug.Log(tokensOutput);
		}

		// Infix to postfix
		Queue<Token> sortedTokens = Algorithm(tokens);
		if (mDebugMode == true)
		{
			string sortedTokensOutput = "Sorted Tokens: ";
			foreach (Token token in sortedTokens)
			{
				sortedTokensOutput += (token.mType == eTokenType.NUMBER ? token.mValue.ToString() : token.mOperator) + "; ";
			}
			Debug.Log(sortedTokensOutput);
		}

		// Calculate
		return Calculate(sortedTokens);
	}

	private List<Token> GetTokens(string aInput, Dictionary<string, float> aVariables)
	{
		List<Token> tokens = new List<Token>();
		for (int i = 0; i < aInput.Length; ++i)
		{
			if (char.IsWhiteSpace(aInput[i]) == true)
				continue;

			if (char.IsNumber(aInput[i]) == true)
			{
				string numberString = string.Empty;
				while (i < aInput.Length && (char.IsNumber(aInput[i]) == true || aInput[i] == '.'))
				{
					numberString += aInput[i].ToString();
					if (i < aInput.Length - 1 && (char.IsNumber(aInput[i + 1]) == true || aInput[i + 1] == '.'))
						++i;
					else
						break;
				}
				Token token = new Token { mType = eTokenType.NUMBER, mValue = float.Parse(numberString) };
				tokens.Add(token);
			}
			else
			{
				bool isVariable = false;
				foreach (KeyValuePair<string, float> kvp in aVariables)
				{
					if (aInput.IndexOf(kvp.Key, i) == i)
					{
						tokens.Add(new Token { mType = eTokenType.NUMBER, mValue = kvp.Value });
						i += kvp.Key.Length - 1;
						isVariable = true;
						break;
					}
				}
				if (isVariable == false)
				{
					foreach (KeyValuePair<string, Token> kvp in mOperators)
					{
						if (aInput.IndexOf(kvp.Key, i) == i)
						{
							tokens.Add(kvp.Value.ShallowCopy());
							i += kvp.Key.Length - 1;
							break;
						}
					}
				}
			}
		}
		return tokens;
	}
	
	private Queue<Token> Algorithm(List<Token> aTokens)
	{
		Queue<Token> output = new Queue<Token>();
		Stack<Token> operatorStack = new Stack<Token>();

		for (int i = 0; i < aTokens.Count; ++i)
		{
			if (aTokens[i].mType == eTokenType.NUMBER)
			{
				output.Enqueue(aTokens[i]);
			}
			else if (aTokens[i].mType == eTokenType.OPERATOR)
			{
				if (operatorStack.Count == 0)
				{
					operatorStack.Push(aTokens[i]);
				}
				else
				{
					if (aTokens[i].mPrecedence > operatorStack.Peek().mPrecedence ||
						(aTokens[i].mPrecedence == operatorStack.Peek().mPrecedence && aTokens[i].mRightToLeftAssociativity == true))
					{
						operatorStack.Push(aTokens[i]);
					}
					else if (aTokens[i].mPrecedence < operatorStack.Peek().mPrecedence ||
						(aTokens[i].mPrecedence == operatorStack.Peek().mPrecedence && aTokens[i].mRightToLeftAssociativity == false))
					{
						while (operatorStack.Count > 0 && aTokens[i].mPrecedence <= operatorStack.Peek().mPrecedence)
						{
							output.Enqueue(operatorStack.Pop());
						}
						operatorStack.Push(aTokens[i]);
					}
				}
			}
			else if (aTokens[i].mType == eTokenType.PARENTHESIS)
			{
				if (aTokens[i].mOperator == "(")
				{
					operatorStack.Push(aTokens[i]);
				}
				else if (aTokens[i].mOperator == ")")
				{
					while (operatorStack.Count > 0)
					{
						if (operatorStack.Peek().mOperator != "(")
						{
							output.Enqueue(operatorStack.Pop());
						}
						else
						{
							operatorStack.Pop();
							if (operatorStack.Count > 0 && operatorStack.Peek().mType == eTokenType.FUNCTION)
							{
								output.Enqueue(operatorStack.Pop());
							}
							break;
						}
					}
				}
			}
			else if (aTokens[i].mType == eTokenType.FUNCTION)
			{
				operatorStack.Push(aTokens[i]);
			}
			else if (aTokens[i].mType == eTokenType.FUNCTION_DIVIDER)
			{
				while (operatorStack.Count > 0)
				{ 
					if (operatorStack.Peek().mOperator != "(")
					{
						output.Enqueue(operatorStack.Pop());
					}
					else
					{
						break;
					}
				}
			}
		}
		while (operatorStack.Count > 0)
		{
			output.Enqueue(operatorStack.Pop());
		}

		return output;
	}

	private float Calculate(Queue<Token> aTokens)
	{
		Stack<float> stack = new Stack<float>();
		while (aTokens.Count > 0)
		{
			Token token = aTokens.Dequeue();
			if (token.mType == eTokenType.NUMBER)
			{
				stack.Push(token.mValue);
			}
			else if (stack.Count > 0)
			{
				float value1 = 0.0f;
				float value2 = 0.0f;
				if (token.mOperator == "+" && stack.Count >= 2)
				{
					stack.Push(stack.Pop() + stack.Pop());
				}
				else if (token.mOperator == "-" && stack.Count >= 2)
				{
					value1 = stack.Pop();
					value2 = stack.Pop();
					stack.Push(value2 - value1);
				}
				else if (token.mOperator == "*" && stack.Count >= 2)
				{
					stack.Push(stack.Pop() * stack.Pop());
				}
				else if (token.mOperator == "/" && stack.Count >= 2)
				{
					value1 = stack.Pop();
					value2 = stack.Pop();
					stack.Push(value2 / value1);
				}
				else if (token.mOperator == "^" && stack.Count >= 2)
				{
					value1 = stack.Pop();
					value2 = stack.Pop();
					stack.Push(Mathf.Pow(value2, value1));
				}
				else if (token.mOperator == "min" && stack.Count >= 2)
				{
					stack.Push(Mathf.Min(stack.Pop(), stack.Pop()));
				}
				else if (token.mOperator == "max" && stack.Count >= 2)
				{
					stack.Push(Mathf.Max(stack.Pop(), stack.Pop()));
				}
				else if (token.mOperator == "rand" && stack.Count >= 2)
				{
					stack.Push(Random.Range(stack.Pop(), stack.Pop()));
				}
				else if (token.mOperator == "cos")
				{
					stack.Push(Mathf.Cos(stack.Pop()));
				}
				else if (token.mOperator == "sin")
				{
					stack.Push(Mathf.Sin(stack.Pop()));
				}
				else if (token.mOperator == "tan")
				{
					stack.Push(Mathf.Tan(stack.Pop()));
				}
			}
			else
			{
				return 0.0f;
			}
		}
		if (stack.Count == 1)
		{
			return stack.Pop();
		}
		else
		{
			return 0.0f;
		}
	}
}
