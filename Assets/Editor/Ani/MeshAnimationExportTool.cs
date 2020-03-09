using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;




public class MeshAnimationExportTool : EditorWindow
{
	MeshAnimation exportSettings;
	private GameObject fbx;
	private int guiNumberOfClips;
	private int guiNumberOfBones;
	private Vector2 mainScrollPosition = Vector2.zero;
	private string outputFolderPath ;
	private string outputFilePath = "";
	private GUILayoutOption labelWidth;
	private float windowWidth;
	private Quaternion quaternion;

	public static string DEFAULT_OUTPUT_FOLDER = "Assets/AniAssets/";
	public static Vector3 eulerAngle = Vector3.zero;

	[MenuItem ("IGG/Export Animation")]
	static void CreateWizard()
	{
		eulerAngle = new Vector3(-90.0f, 0.0f, 0.0f);
		MeshAnimationExportTool window = (MeshAnimationExportTool)EditorWindow.GetWindow(typeof(MeshAnimationExportTool));
		window.ResetSettings();
		window.ShowUtility();
	}

	MeshAnimationExportTool()
	{
		ResetSettings();
	}

	private void ResetSettings()
	{
		exportSettings = new MeshAnimation();
	}


	private  string GetProjectPath()
	{
		return Application.dataPath.Substring (0, Application.dataPath.LastIndexOf ("/"));
	}
	/// <summary>
	/// Get the relative project file path from an absolute path
	/// </summary>
	/// <returns>The absolute file path</returns>
	/// <param name="pAbsolutePath">Absolute file path</param>
	public  string GetAssetPath (string pAbsolutePath)
	{
		string projectPath = GetProjectPath();
		if(pAbsolutePath.StartsWith(projectPath))
		{
			string relativePath = pAbsolutePath.Substring(projectPath.Length, pAbsolutePath.Length - projectPath.Length);

			if(relativePath.StartsWith("/") || relativePath.StartsWith("\\"))
			{
				relativePath = relativePath.Substring(1, relativePath.Length - 1);
			}

			return relativePath;
		}

		return null;
	}



    void OnGUI()
	{
        if(outputFilePath == null)
            outputFolderPath = Application.streamingAssetsPath;

        if (exportSettings == null)
		{
			exportSettings = new MeshAnimation();
		}
		windowWidth = position.width;	

		mainScrollPosition = EditorGUILayout.BeginScrollView(mainScrollPosition);

        //烘培定义角度转向
        eulerAngle = EditorGUILayout.Vector3Field("Bake model after apply Rotation Offset XYZ", eulerAngle);

        EditorGUILayout.Space();

        //默认路径
        string defultPath = DEFAULT_OUTPUT_FOLDER;

        outputFilePath = defultPath;
        outputFolderPath = Path.GetDirectoryName(defultPath);

		EditorGUILayout.LabelField("Default Folder:" + outputFolderPath);

        EditorGUILayout.Space();

		#region Base FBX File Setting
		EditorGUILayout.BeginHorizontal();
		{
			EditorGUILayout.LabelField("Base FBX:", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
			GameObject newFbx = EditorGUILayout.ObjectField(fbx, typeof(GameObject), true) as GameObject;

			if (newFbx != null && newFbx != fbx)
			{
				// error if they drag the prefab itself, since it won't have any transform data
				if (PrefabUtility.GetPrefabParent(newFbx) != null)
				{
					fbx = newFbx;
					exportSettings.AnalysisFbx(newFbx, Quaternion.identity);
					guiNumberOfClips = exportSettings.GetClipCount();
					guiNumberOfBones = exportSettings.GetBones();

				}
			}
		}

		EditorGUILayout.EndHorizontal();
		#endregion

		EditorGUILayout.Space();

		#region Framerate Setting
		EditorGUILayout.FloatField("Capture Framerate:", IGGUtil.framerate);
		#endregion

		EditorGUILayout.Space();

		#region Clip Count Setting
		guiNumberOfClips = EditorGUILayout.IntField("Number of Clips:", guiNumberOfClips);
		#endregion

		EditorGUILayout.Space();

		labelWidth = GUILayout.Width(windowWidth * 0.35f);

		#region Animation Clip Setting
		EditorGUILayout.BeginHorizontal();
		{
			EditorGUILayout.LabelField("Animation Name:", labelWidth);
			EditorGUILayout.LabelField("Animation File:", labelWidth);
			EditorGUILayout.LabelField("Frames:", GUILayout.Width(windowWidth * 0.2f));
		}
		EditorGUILayout.EndHorizontal();

		DrawAnimationArrayGui();
		#endregion

		EditorGUILayout.Space();

		#region Bone Count Setting
		guiNumberOfBones = EditorGUILayout.IntField("Number of Bones:", guiNumberOfBones);
		#endregion

        #region Clear and Save Buttons
        EditorGUILayout.BeginHorizontal();
		{

			if (GUILayout.Button("Export_CPU_RGBA"))
			{
                //Save 
                if(eulerAngle != Vector3.zero) {
					quaternion = Quaternion.Euler(eulerAngle);
                }

				CpuAniMesh_RGBAHalfExport  exp = new CpuAniMesh_RGBAHalfExport();
				exp.SetAniMeshData(exportSettings);
				exp.Export(outputFolderPath, quaternion, fbx.name);
            }

			if (GUILayout.Button("Export_GPU_RGBAHalf"))
			{
				//Save 
				if(eulerAngle != Vector3.zero) {
					quaternion = Quaternion.Euler(eulerAngle);
				}

				GpuAniMesh_RGBAHalfExport  exp = new GpuAniMesh_RGBAHalfExport();
				exp.SetAniMeshData(exportSettings);
				exp.Export(outputFolderPath, quaternion, fbx.name);
			}


			if (GUILayout.Button("Export_GPU_3RHalf"))
			{
				//Save 
				if(eulerAngle != Vector3.zero) {
					quaternion = Quaternion.Euler(eulerAngle);
				}

				GpuAniMesh_3RHalfExport  exp = new GpuAniMesh_3RHalfExport();
				exp.SetAniMeshData(exportSettings);
				exp.Export(outputFolderPath, quaternion, fbx.name);
			}


			if (GUILayout.Button("Export_GPU_RGBM"))
			{
				//Save 
				if(eulerAngle != Vector3.zero) {
					quaternion = Quaternion.Euler(eulerAngle);
				}

				GpuAniMesh_RGBMExport  exp = new GpuAniMesh_RGBMExport();
				exp.SetAniMeshData(exportSettings);
				exp.Export(outputFolderPath, quaternion, fbx.name);
			}
		}
		EditorGUILayout.EndHorizontal();




		EditorGUILayout.BeginHorizontal();
		{
			if (GUILayout.Button("Export_CPUSkin"))
			{
				//Save 
				if(eulerAngle != Vector3.zero) {
					quaternion = Quaternion.Euler(eulerAngle);
				}

				CpuSkin_RGBAExport  exp = new CpuSkin_RGBAExport();
				exp.SetAniMeshData(exportSettings);
				exp.Export(outputFolderPath, quaternion, fbx.name);
			}

			if (GUILayout.Button("Export_GPUSkin"))
			{
				//Save 
				if(eulerAngle != Vector3.zero) {
					quaternion = Quaternion.Euler(eulerAngle);
				}

				GpuSkin_RGBAExport  exp = new GpuSkin_RGBAExport();
				exp.SetAniMeshData(exportSettings);
				exp.Export(outputFolderPath, quaternion, fbx.name);
			}

            if (GUILayout.Button("Export_GPUSkinInstance"))
            {
                //Save 
                if (eulerAngle != Vector3.zero)
                {
                    quaternion = Quaternion.Euler(eulerAngle);
                }

                GpuSkinInstance_Export exp = new GpuSkinInstance_Export();
                exp.SetAniMeshData(exportSettings);
                exp.Export(outputFolderPath, quaternion, fbx.name);
            }

        }
		EditorGUILayout.EndHorizontal();
		#endregion

		EditorGUILayout.EndScrollView();
	}


	private void DrawAnimationArrayGui()
	{
		for (int i = 0; i < exportSettings.GetClipCount(); i++)
		{
			EditorGUILayout.BeginHorizontal();
			//
			EditorGUILayout.TextField(exportSettings.AniMeshClip[i].clipName, labelWidth);
			EditorGUILayout.ObjectField(exportSettings.AniMeshClip[i].clip, typeof(AnimationClip), true, labelWidth) ;
			EditorGUILayout.LabelField(exportSettings.AniMeshClip[i].FrameCount.ToString(), GUILayout.Width(windowWidth * 0.2f));
			//
			EditorGUILayout.EndHorizontal();
		}
	}

		
}
