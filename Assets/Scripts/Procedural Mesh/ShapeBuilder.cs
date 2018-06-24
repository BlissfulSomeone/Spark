using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PMesh
{
	public class ShapeBuilder : MonoBehaviour
	{
		private List<Shape> mShapes = new List<Shape>();
		private List<Shape> mTempShapes = new List<Shape>();

		private List<GameObject> mObjects = new List<GameObject>();

		public GameObject mCubePrefab;

		private ShuntingYard mExpressionParser = new ShuntingYard();
		private Dictionary<string, float> mVariables = new Dictionary<string, float>();

		public void BuildShape(Dictionary<string, RuleSet> aRuleSets, Dictionary<string, float> aVariables)
		{
			mVariables = aVariables;
			mExpressionParser.SetVariables(mVariables);
			StopAllCoroutines();
			StartCoroutine(Coroutine_BuildShape(aRuleSets));
		}

		private IEnumerator Coroutine_BuildShape(Dictionary<string, RuleSet> aRuleSets)
		{
			Debug.Log("Start");

			for (int i = 0; i < mObjects.Count; ++i)
			{
				Destroy(mObjects[i].gameObject);
			}
			mObjects.Clear();

			mShapes.Clear();
			Shape initialShape = new Shape();
			initialShape.mName = "Footprint";
			mShapes.Add(initialShape);

			bool done = false;
			while (done == false)
			{
				mTempShapes.Clear();
				done = true;
				for (int i = 0; i < mShapes.Count; ++i)
				{
					if (aRuleSets.ContainsKey(mShapes[i].mName) == true && mShapes[i].mScope.mChildren.Count == 0)
					{
						done = false;
						RuleSet ruleSet = aRuleSets[mShapes[i].mName];
						for (int j = 0; j < ruleSet.mRules.Count; ++j)
						{
							ProcessRule(ruleSet.mRules[j], mShapes[i]);
						}
					}
				}
				if (done == false)
				{
					mShapes.Clear();
					mShapes.AddRange(mTempShapes);
				}
			}
			for (int i = 0; i < mShapes.Count; ++i)
			{
				if (mShapes[i].mScope.mChildren.Count > 0)
					continue;
					
				Vector3 size = mShapes[i].mScope.mScale;
				Vector3 position = mShapes[i].mScope.mPosition + size * 0.5f;
				GameObject go = Instantiate(mCubePrefab) as GameObject;
				go.transform.position = position;
				go.transform.localScale = size;
				go.GetComponent<MeshRenderer>().materials[0].color = mShapes[i].mScope.mColor;
				go.GetComponent<MeshRenderer>().materials[1].color = mShapes[i].mScope.mColor;
				mObjects.Add(go);
			}
			yield return new WaitForEndOfFrame();
		}

		private void ProcessRule(BaseRule aRule, Shape aShape)
		{
			Dictionary<string, float> variables = new Dictionary<string, float>();
			foreach (KeyValuePair<string, float> kvp in mVariables)
			{
				variables.Add(kvp.Key, kvp.Value);
			}
			variables.Add("scope.position.x", aShape.mScope.mPosition.x);
			variables.Add("scope.position.y", aShape.mScope.mPosition.y);
			variables.Add("scope.position.z", aShape.mScope.mPosition.z);
			variables.Add("scope.scale.x", aShape.mScope.mScale.x);
			variables.Add("scope.scale.y", aShape.mScope.mScale.y);
			variables.Add("scope.scale.z", aShape.mScope.mScale.z);
			if (aShape.mSplitData != null)
			{
				variables.Add("split.index", aShape.mSplitData.mSplitIndex);
				variables.Add("split.total", aShape.mSplitData.mSplitTotal);
			}
			mExpressionParser.SetVariables(variables);

			if (SparkUtilities.Cast<ExtrudeRule>(aRule) != null)
				ProcessRule(SparkUtilities.Cast<ExtrudeRule>(aRule), aShape);

			else if (SparkUtilities.Cast<ScaleRule>(aRule) != null)
				ProcessRule(SparkUtilities.Cast<ScaleRule>(aRule), aShape);

			else if (SparkUtilities.Cast<SplitRule>(aRule) != null)
				ProcessRule(SparkUtilities.Cast<SplitRule>(aRule), aShape);

			else if (SparkUtilities.Cast<TranslateRule>(aRule) != null)
				ProcessRule(SparkUtilities.Cast<TranslateRule>(aRule), aShape);

			else if (SparkUtilities.Cast<CopyRule>(aRule) != null)
				ProcessRule(SparkUtilities.Cast<CopyRule>(aRule), aShape);

			else if (SparkUtilities.Cast<ColorRule>(aRule) != null)
				ProcessRule(SparkUtilities.Cast<ColorRule>(aRule), aShape);
		}

		private void ProcessRule(ExtrudeRule aRule, Shape aShape)
		{
			Debug.Log("Extrude");
		}

		private void ProcessRule(ScaleRule aRule, Shape aShape)
		{
			Debug.Log("Scale");
			float[] scale = new float[3] { 0.0f, 0.0f, 0.0f };
			for (int i = 0; i < 3; ++i)
			{
				if (aRule.mScale[i][0] == '\'')
					scale[i] = aShape.mScope.mScale[i] * mExpressionParser.Parse(aRule.mScale[i].Substring(1));
				else
					scale[i] = mExpressionParser.Parse(aRule.mScale[i]);
			}
			aShape.mScope.mScale = new Vector3(scale[0], scale[1], scale[2]);
		}

		private void ProcessRule(SplitRule aRule, Shape aShape)
		{
			Debug.Log("Split");
			bool isRelative = aRule.mNumberOfSplits[0] == '\'';
			float inputNumberOfSplits = mExpressionParser.Parse(aRule.mNumberOfSplits);
			float newSize = 0.0f;
			int numberOfSplits = 0;
			Vector3 axis = Vector3.zero;
			axis[(int)aRule.mAxis] = 1;
			if (isRelative)
			{
				aRule.mNumberOfSplits = aRule.mNumberOfSplits.Substring(1);
				newSize = aShape.mScope.mScale[(int)aRule.mAxis] / inputNumberOfSplits;
				numberOfSplits = (int)(inputNumberOfSplits);
			}
			else
			{
				newSize = inputNumberOfSplits;
				numberOfSplits = Mathf.FloorToInt(aShape.mScope.mScale[(int)aRule.mAxis] / inputNumberOfSplits);
			}

			for (int i = 0; i < numberOfSplits; ++i)
			{
				Shape splitShape = new Shape();
				splitShape.mName = aRule.mNameOfChildren;
				splitShape.mScope.mPosition = aShape.mScope.mPosition + axis * newSize * i;
				splitShape.mScope.mScale = aShape.mScope.mScale;
				splitShape.mScope.mScale[(int)aRule.mAxis] = newSize;
				splitShape.mSplitData = new SplitAttributes { mSplitIndex = i, mSplitTotal = numberOfSplits };
				mTempShapes.Add(splitShape);
				aShape.mScope.mChildren.Add(splitShape.mScope);
			}
		}

		private void ProcessRule(TranslateRule aRule, Shape aShape)
		{
			Debug.Log("Translate");
			float moveX = mExpressionParser.Parse(aRule.mTranslation[0]);
			float moveY = mExpressionParser.Parse(aRule.mTranslation[1]);
			float moveZ = mExpressionParser.Parse(aRule.mTranslation[2]);
			aShape.mScope.mPosition += new Vector3(moveX, moveY, moveZ);
		}

		private void ProcessRule(CopyRule aRule, Shape aShape)
		{
			Debug.Log("Copy");
			Shape copyShape = new Shape();
			copyShape.mName = aRule.mNameOfChild;
			copyShape.mScope.mPosition = aShape.mScope.mPosition;
			copyShape.mScope.mScale = aShape.mScope.mScale;
			copyShape.mScope.mColor = aShape.mScope.mColor;
			mTempShapes.Add(copyShape);
			aShape.mScope.mChildren.Add(copyShape.mScope);
		}

		private void ProcessRule(ColorRule aRule, Shape aShape)
		{
			Debug.Log("Color");
			ColorUtility.TryParseHtmlString(aRule.mHexColor, out aShape.mScope.mColor);
		}
	}
}
