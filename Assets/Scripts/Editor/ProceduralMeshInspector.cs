using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//[CustomEditor(typeof(ProceduralMesh))]
//public class ProceduralMeshInspector : Editor
//{
//	private ProceduralMesh mMesh = null;
//	private ProceduralMesh PMesh
//	{
//		get
//		{
//			if (mMesh == null)
//			{
//				mMesh = target as ProceduralMesh;
//			}
//			return mMesh;
//		}
//	}
//
//	public override void OnInspectorGUI()
//	{
//		if (Application.isPlaying == false)
//			return;
//			
//		if (GUILayout.Button("Add new rule set") == true)
//		{
//			PMesh.mRuleSets.Add(CreateInstance<ProceduralMeshSet>());
//		}
//
//		for (int i = 0; i < PMesh.mRuleSets.Count; ++i)
//		{
//			OnGUI_DrawSet(PMesh.mRuleSets[i]);
//		}
//	}
//
//	private void OnGUI_DrawSet(ProceduralMeshSet aSet)
//	{
//		EditorGUILayout.BeginVertical();
//		{
//			EditorGUI.indentLevel = 0;
//			EditorGUILayout.BeginHorizontal();
//			{
//				if (GUILayout.Button(aSet.mIsOpen ? "-" : "+", GUILayout.Width(20.0f), GUILayout.Height(15.0f)) == true)
//				{
//					aSet.mIsOpen = !aSet.mIsOpen;
//				}
//				aSet.mName = EditorGUILayout.TextField(aSet.mName);
//				GUI.color = Color.red;
//				if (GUILayout.Button("x", GUILayout.Width(20.0f), GUILayout.Height(15.0f)) == true)
//				{
//					PMesh.mRuleSets.Remove(aSet);
//				}
//				GUI.color = Color.white;
//			}
//			EditorGUILayout.EndHorizontal();
//
//			if (aSet.mIsOpen == true)
//			{
//				eProceduralMeshRule gbbnrbhrie = (eProceduralMeshRule)EditorGUILayout.EnumPopup("Add new rule: ", eProceduralMeshRule.RULE);
//				if (gbbnrbhrie != eProceduralMeshRule.RULE)
//				{
//					switch (gbbnrbhrie)
//					{
//						case eProceduralMeshRule.EXTRUDE:
//							aSet.mRules.Add(new ProceduralMeshRuleExtrude());
//							break;
//
//						case eProceduralMeshRule.SCALE:
//							aSet.mRules.Add(new ProceduralMeshRuleScale());
//							break;
//					}
//				}
//				for (int i = 0; i < aSet.mRules.Count; ++i)
//				{
//					int reply = aSet.mRules[i].OnGUI_Draw();
//					if (reply == ProceduralMeshRule.REPLY_DELETE)
//					{
//						aSet.mRules.RemoveAt(i);
//					}
//					else if (reply == ProceduralMeshRule.REPLY_MOVE_UP && i > 0)
//					{
//						ProceduralMeshRule temp = aSet.mRules[i - 1];
//						aSet.mRules[i - 1] = aSet.mRules[i];
//						aSet.mRules[i] = temp;
//					}
//					else if (reply == ProceduralMeshRule.REPLY_MOVE_DOWN && i < aSet.mRules.Count - 1)
//					{
//						ProceduralMeshRule temp = aSet.mRules[i + 1];
//						aSet.mRules[i + 1] = aSet.mRules[i];
//						aSet.mRules[i] = temp;
//					}
//				}
//			}
//		}
//		EditorGUILayout.EndVertical();
//	}
//}
