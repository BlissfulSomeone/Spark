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
					bool shapeProcessed = false;
					if (aRuleSets.ContainsKey(mShapes[i].mName) == true && mShapes[i].mIsDone == false)
					{
						shapeProcessed = true;
						done = false;
						RuleSet ruleSet = aRuleSets[mShapes[i].mName];
						for (int j = 0; j < ruleSet.mRules.Count; ++j)
						{
							if (ProcessRule(ruleSet.mRules[j], mShapes[i]) == eRuleReply.Failed)
							{
								break;
							}
						}
					}
					mShapes[i].mIsDone = true;
					if (shapeProcessed == false)
					{
						mTempShapes.Add(mShapes[i]);
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

		private eRuleReply ProcessRule(BaseRule aRule, Shape aShape)
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

			return aRule.Process(aShape, ref mTempShapes, mExpressionParser);
		}
	}
}
