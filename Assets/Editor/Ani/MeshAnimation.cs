using System.Collections.Generic;
using UnityEngine;

public class MeshAnimation {
	public string m_ActorName;
 
	public SubMesh m_Mesh = new SubMesh();
	private List<AniMeshClip> m_AniMeshClip = new List<AniMeshClip>();
	public List<AniMeshClip> AniMeshClip
	{
		get{ return m_AniMeshClip;}
	}
		
	private List<SkinBone> m_SkinBones = new List<SkinBone>();





	public int GetClipCount()
	{
		return m_AniMeshClip.Count;
	}


	public int GetBones()
	{
		int bones = 0;
		bones += m_Mesh.bones;
		return bones;
	}

	public int GetTotalFrame()
	{
		int totalframe = 0;
		foreach (AniMeshClip clip in m_AniMeshClip) {
			totalframe += clip.FrameCount;
		}
		return totalframe;
	}


	public int GetTotalVertices()
	{
		return m_Mesh.m_vertices.Length;
	}


	// 解析fbx
	public void AnalysisFbx(GameObject pFbxInstance, Quaternion quaternionOffset)
	{
		Clear();
		float frameInterval = 1.0f / IGGUtil.framerate;
		GameObject go = GameObject.Instantiate(pFbxInstance) as GameObject;
		if (!go)
		{
			Debug.Log("pFbxInstance is null!");
			return;
		}
		this.m_ActorName = go.name;

		Transform baseTransform = go.transform;

		Animation Ani = go.GetComponentInChildren<Animation>();
		if (!Ani)
		{
			Debug.Log("Target game object has no Animation component!");
			return;
		}

		int totalframe = 0;
		foreach (AnimationState state in Ani)
		{
			AniMeshClip clip = new AniMeshClip();
			clip.clipName = state.clip.name;
			clip.clip = state.clip;
			clip.StartFrame = totalframe;
			float frames = state.clip.length/ frameInterval + 1;
			clip.FrameCount = (int)frames;
			clip.EndFrame = clip.StartFrame + clip.FrameCount - 1;
			totalframe += clip.FrameCount;
			m_AniMeshClip.Add(clip);
		}
			
		SkinnedMeshRenderer  renderArray = go.GetComponentInChildren<SkinnedMeshRenderer>();
		m_Mesh = new SubMesh ();
		m_Mesh.bones = renderArray.bones.Length;
		m_SkinBones = new List<SkinBone>();
		for(int i = 0; i < renderArray.bones.Length; i++)
		{
			SkinBone skin = new SkinBone();
			skin.transform = renderArray.bones[i];
			skin.bindpose = renderArray.sharedMesh.bindposes[i];
			// 获取父骨骼节点。
			skin.parentBoneIndex = System.Array.IndexOf(renderArray.bones, skin.transform.parent);
			if (skin.transform.childCount > 0) 
			{
				skin.childrenBonesIndices = new int[skin.transform.childCount];
				for(int k =0; k < skin.transform.childCount; k++)
				{
					skin.childrenBonesIndices[k] = System.Array.IndexOf(renderArray.bones, skin.transform.GetChild(k));
				}
			}
			m_SkinBones.Add(skin);
		}
		//
		m_Mesh.m_vertices = renderArray.sharedMesh.vertices;
		m_Mesh.m_UV = renderArray.sharedMesh.uv;
		m_Mesh.m_Triangles = renderArray.sharedMesh.triangles;
		m_Mesh.m_BoneWeight = renderArray.sharedMesh.boneWeights;
		// 解析 ani mesh 数据
		for (int i = 0; i < m_AniMeshClip.Count; i++)
		{
			// Set the animation clip to export
			AnimationClip clip = m_AniMeshClip[i].clip;
			if (null == clip)
			{
				continue;
			}

			Ani.AddClip(clip, clip.name);
			Ani.clip = clip;
			AnimationState state = Ani[clip.name];
			state.enabled = true;
			state.weight = 1;

			float clipLength = clip.length;
			List<float> frameTimes = GetFrameTimes(clipLength, frameInterval);

			foreach (float time in frameTimes)
			{
				state.time = time;

				Ani.Play();
				Ani.Sample();
				Mesh bakeMesh = null;

				if (quaternionOffset != Quaternion.identity)
				{
					Matrix4x4 matrix = new Matrix4x4();
					matrix.SetTRS(Vector2.zero, quaternionOffset, Vector3.one);
				    bakeMesh = BakeFrameAfterMatrixTransform(renderArray, matrix);
				}
				else
				{
					bakeMesh = new Mesh();
				    renderArray.BakeMesh(bakeMesh);
				}
				m_Mesh.m_AniMesh.Add(bakeMesh.vertices);

				// 骨骼数据
				Matrix4x4[] boneFrame = new Matrix4x4[m_SkinBones.Count];
				for (int k = 0; k < m_SkinBones.Count; k++)
				{
					SkinBone CurBone= m_SkinBones [k];
					boneFrame [k] = CurBone.bindpose;
					do {
						Matrix4x4 mat = Matrix4x4.TRS(CurBone.transform.localPosition, CurBone.transform.localRotation, CurBone.transform.localScale);
						//Matrix4x4 mat = Matrix4x4.TRS(CurBone.transform.localPosition, CurBone.transform.localRotation, Vector3.one);
						boneFrame [k] = mat * boneFrame [k];
						if(CurBone.parentBoneIndex == -1)
						{
							break;
						}
						else 
						{
							CurBone = m_SkinBones[CurBone.parentBoneIndex];
						}
					} 
					while(true);
				}

				m_Mesh.m_AniBone.Add(boneFrame);


				bakeMesh.Clear();
				Object.DestroyImmediate(bakeMesh);
				Ani.Stop();
			}
		}
		GameObject.DestroyImmediate(go);
	}


	public void Clear ()
	{
		m_AniMeshClip.Clear();
		m_Mesh = null;
		m_SkinBones.Clear ();
		m_SkinBones = null;
	}
		

		
	/// <summary>
	/// Calculate the time periods when a frame snapshot should be taken
	/// </summary>
	/// <returns>The frame times.</returns>
	/// <param name="pLength">P length.</param>
	/// <param name="pInterval">P interval.</param>
	public List<float> GetFrameTimes(float pLength, float pInterval)
	{
		List<float> times = new List<float>();

		float time = 0;

		do
		{
			times.Add(time);
			time += pInterval;
		} while (time < pLength);

		times.Add(pLength);

		return times;
	}

	/// <summary>
	/// Take a snapshot of the frame as a mesh and transform the vertices to from local
	/// to world.
	/// </summary>
	/// <returns>Mesh snapshot</returns>
	/// <param name="pRenderer">SkinnedMeshRenderer used to render the current frame</param>
	private Mesh BakeFrame(SkinnedMeshRenderer pRenderer)
	{
		Mesh result = new Mesh();
		pRenderer.BakeMesh(result);
		result.vertices = TransformVertices(pRenderer.transform.localToWorldMatrix, result.vertices);
		return result;
	}

	private Mesh BakeFrameAfterMatrixTransform(SkinnedMeshRenderer pRenderer, Matrix4x4 matrix) {
		Mesh result = new Mesh();
		pRenderer.BakeMesh(result);
		result.vertices = TransformVertices(matrix, result.vertices);
		return result;
	}

	/// <summary>
	/// Convert a set of vertices using the given transform matrix.
	/// </summary>
	/// <returns>Transformed vertices</returns>
	/// <param name="pLocalToWorld">Transform Matrix</param>
	/// <param name="pVertices">Vertices to transform</param>
	private Vector3[] TransformVertices(Matrix4x4 pLocalToWorld, Vector3[] pVertices)
	{
		Vector3[] result = new Vector3[pVertices.Length];
		for (int i = 0; i < pVertices.Length; i++)
		{
			result[i] = pLocalToWorld * pVertices[i];
		}
		return result;
	}
}


public class SubMesh {
	public string Name;
	public int bones;
	public Vector3[] m_vertices = null;

	public Vector2[] m_UV = null;
	public int[] m_Triangles = null;
	public BoneWeight[] m_BoneWeight = null;


	public List<Vector3[]> m_AniMesh = new List<Vector3[]>();
	//
	public List<Matrix4x4[]> m_AniBone = new List<Matrix4x4[]>();

	// 顶点数
	public int GetVerticeCount()
	{
		if (m_vertices == null) {
			return 0;
		} 
		else 
		{
			return m_vertices.Length;
		}
	}

	// 三角面数
	public int GetTriangleCount()
	{
		if (m_Triangles == null) {
			return 0;
		} 
		else 
		{
			return m_Triangles.Length;
		}
	}
}

public class AniMeshClip {
	public AnimationClip clip;
	public string clipName;
	public int FrameCount;           // 动画总帧树
	public int StartFrame;           // 动画开始帧
	public int EndFrame;             // 动画结束帧
}



public class SkinBone
{
	public Transform transform = null;

	public Matrix4x4 bindpose;

	public int parentBoneIndex = -1;

	public int[] childrenBonesIndices = null;
}