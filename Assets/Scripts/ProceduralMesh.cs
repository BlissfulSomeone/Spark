using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Vertex
{
	public Vector3 position = Vector3.zero;
	public Vector3 normal = Vector3.zero;
	public Vector2 uv = Vector2.zero;
}

public class FaceHandle
{
	public List<Vertex> vertices = new List<Vertex>();
	public string name;

	public FaceHandle() { }

	public FaceHandle(params Vertex[] aVertices)
	{
		vertices.AddRange(aVertices);
	}
}

public class ProceduralMesh : MonoBehaviour
{
	private Mesh mMesh;

	public List<ProceduralMeshSet> mRuleSets = new List<ProceduralMeshSet>();

	private List<Vector3> mVertices = new List<Vector3>();
	private List<Vector3> mNormals = new List<Vector3>();
	private List<Vector2> mUvs = new List<Vector2>();
	private List<int> mIndices = new List<int>();

	private List<FaceHandle> mFaces = new List<FaceHandle>();

	private void Start()
	{
		mMesh = new Mesh();
		mMesh.name = "Procedural Mesh";

		ResetMesh();

		UpdateMesh();
	}

	public List<FaceHandle> ExtrudeFace(FaceHandle aFace, float aLength)
	{
		List<FaceHandle> newFaces = new List<FaceHandle>();
		if (aFace.vertices.Count < 3)
		{
			Debug.LogWarning("Invalid face. A face needs at least 3 vertices. The face extrusion will be skipped.");
			return newFaces;
		}
		Vector3 averageNormal = Vector3.zero;
		for (int triangleIndex = 0; triangleIndex < aFace.vertices.Count - 2; ++triangleIndex)
		{
			Vector3 AB = aFace.vertices[(triangleIndex + 1) % aFace.vertices.Count].position - aFace.vertices[triangleIndex].position;
			Vector3 AC = aFace.vertices[(triangleIndex + 2) % aFace.vertices.Count].position - aFace.vertices[triangleIndex].position;
			Vector3 normal = Vector3.Cross(AB, AC).normalized;
			averageNormal += normal;
		}
		averageNormal.Normalize();

		Vertex[] tempVertices = new Vertex[aFace.vertices.Count];
		for (int vertexIndex = 0; vertexIndex < aFace.vertices.Count; ++vertexIndex)
		{
			tempVertices[vertexIndex] = new Vertex { position = aFace.vertices[vertexIndex].position + averageNormal * aLength, normal = Vector3.up, uv = Vector2.up };
		}
		for (int faceIndex = 0; faceIndex < aFace.vertices.Count; ++faceIndex)
		{
			int numVertices = aFace.vertices.Count;
			int i0 = faceIndex;
			int i1 = (faceIndex + 1) % numVertices;
			mFaces.Add(new FaceHandle(aFace.vertices[i0], aFace.vertices[i1], tempVertices[i1], tempVertices[i0]));
			newFaces.Add(mFaces.Last());
		}
		mFaces.Add(new FaceHandle(tempVertices));
		mFaces.Remove(aFace);
		return newFaces;
	} 

	public void ScaleFace(FaceHandle aFace, Vector2 aScale)
	{
		if (aFace.vertices.Count < 3)
		{
			Debug.LogWarning("Invalid face. A face needs at least 3 vertices. The face scaling will be skipped.");
			return;
		}

		Vector3 center = Vector3.zero;
		for (int vertexIndex = 0; vertexIndex < aFace.vertices.Count; ++vertexIndex)
		{
			center += aFace.vertices[vertexIndex].position;
		}
		center /= aFace.vertices.Count;
		for (int vertexIndex = 0; vertexIndex < aFace.vertices.Count; ++vertexIndex)
		{
			Vector3 original = aFace.vertices[vertexIndex].position;
			aFace.vertices[vertexIndex].position = center * (1.0f - aScale.x) + original * aScale.x;
		}
	}

	//public void Split()

	private void AddTriangle(int aIndex0, int aIndex1, int aIndex2)
	{
		mIndices.Add(aIndex0);
		mIndices.Add(aIndex1);
		mIndices.Add(aIndex2);
	}

	private void ResetMesh()
	{
		Vertex v0 = new Vertex { position = new Vector3(-1.0f, 0.0f, -1.0f), normal = Vector3.up, uv = new Vector2(0.0f, 0.0f) };
		Vertex v1 = new Vertex { position = new Vector3(-1.0f, 0.0f, 1.0f), normal = Vector3.up, uv = new Vector2(0.0f, 1.0f) };
		Vertex v2 = new Vertex { position = new Vector3(1.0f, 0.0f, 1.0f), normal = Vector3.up, uv = new Vector2(1.0f, 1.0f) };
		Vertex v3 = new Vertex { position = new Vector3(1.0f, 0.0f, -1.0f), normal = Vector3.up, uv = new Vector2(1.0f, 0.0f) };

		mFaces.Clear();
		mFaces.Add(new FaceHandle(v0, v1, v2, v3));
		mFaces[0].name = "Lot";
	}

	private void UpdateMesh()
	{
		mVertices.Clear();
		mNormals.Clear();
		mUvs.Clear();
		mIndices.Clear();
		int triangle = 0;

		for (int faceIndex = 0; faceIndex < mFaces.Count; ++faceIndex)
		{
			FaceHandle face = mFaces[faceIndex];
			if (face.vertices.Count < 3)
			{
				Debug.LogWarning("Invalid face. A face needs at least 3 vertices. The mesh will exlude this face.");
				continue;
			}
			for (int vertexIndex = 0; vertexIndex < face.vertices.Count; ++vertexIndex)
			{
				Vertex vertex = face.vertices[vertexIndex];
				mVertices.Add(vertex.position);

				Vector3 AB = face.vertices[(vertexIndex + 1) % face.vertices.Count].position - vertex.position;
				Vector3 AC = face.vertices[(vertexIndex + 2) % face.vertices.Count].position - vertex.position;
				Vector3 normal = Vector3.Cross(AB, AC).normalized;
				mNormals.Add(normal);
			}
			for (int triangleIndex = 0; triangleIndex < face.vertices.Count - 2; ++triangleIndex)
			{
				AddTriangle(triangle, triangle + 1 + triangleIndex, triangle + 2 + triangleIndex);
			}
			triangle += face.vertices.Count;
		}
		mMesh.Clear();
		mMesh.vertices = mVertices.ToArray();
		mMesh.normals = mNormals.ToArray();
		mMesh.triangles = mIndices.ToArray();
		mMesh.RecalculateBounds();
		MeshFilter meshFilter = GetComponent<MeshFilter>();
		if (meshFilter != null)
		{
			meshFilter.mesh = mMesh;
		}
	}

	private void Update()
	{
		ResetMesh();

		for (int ruleSetIndex = 0; ruleSetIndex < mRuleSets.Count; ++ruleSetIndex)
		{
			List<FaceHandle> facesToEdit = new List<FaceHandle>();
			for (int faceIndex = 0; faceIndex < mFaces.Count; ++faceIndex)
			{
				if (mFaces[faceIndex].name == mRuleSets[ruleSetIndex].mName)
				{
					facesToEdit.Add(mFaces[faceIndex]);
				}
			}
			for (int faceIndex = 0; faceIndex < facesToEdit.Count; ++faceIndex)
			{
				FaceHandle currentFaceHandle = facesToEdit[faceIndex];
				for (int ruleIndex = 0; ruleIndex < mRuleSets[ruleSetIndex].mRules.Count; ++ruleIndex)
				{
					ProceduralMeshRule rule = mRuleSets[ruleSetIndex].mRules[ruleIndex];
					if (SparkUtilities.Cast<ProceduralMeshRuleExtrude>(rule) != null)
					{
						ProceduralMeshRuleExtrude extrudeRule = SparkUtilities.Cast<ProceduralMeshRuleExtrude>(rule);
						float length = extrudeRule.mLength;
						List<FaceHandle> newFaces = ExtrudeFace(currentFaceHandle, length);
						for (int newFaceIndex = 0; newFaceIndex < newFaces.Count; ++newFaceIndex)
						{
							//newFaces[newFaceIndex].name = extrudeRule.mNewFaceNames;
						}
						currentFaceHandle = mFaces.Last();
					}
					else if (SparkUtilities.Cast<ProceduralMeshRuleScale>(rule) != null)
					{
						ProceduralMeshRuleScale scaleRule = SparkUtilities.Cast<ProceduralMeshRuleScale>(rule);
						Vector2 scale = scaleRule.mScale;
						ScaleFace(currentFaceHandle, scale);
					}
				}
			}
		}

		UpdateMesh();
	}
}
