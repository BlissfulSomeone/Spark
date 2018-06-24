using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum eProceduralMeshAxis
{
	X = 1,
	Y = 2,
	Z = 4,
}

public class ProceduralMeshSet : ScriptableObject
{
	public string mName = "Unnamed Set";
	public List<ProceduralMeshRule> mRules = new List<ProceduralMeshRule>();
}

public class ProceduralMeshRule
{
}

public class ProceduralMeshRuleExtrude : ProceduralMeshRule
{
	public float mLength = 1.0f;
}

public class ProceduralMeshRuleScale : ProceduralMeshRule
{
	public Vector2 mScale = Vector2.one;
}

public class ProceduralMeshRuleSplit : ProceduralMeshRule
{
	public eProceduralMeshAxis mSplitAxis = eProceduralMeshAxis.X;
	public float mNewSegmentLength = 0.0f;
	public int mNumberOfSegments = 0;
}
