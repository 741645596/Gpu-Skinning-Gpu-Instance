using System;
using UnityEngine;
using System.Text;

public class FPSCounter : MonoBehaviour {
	const float fpsMeasurePeriod = 0.5f;
	private int m_FpsAccumulator = 0;
	private float m_FpsNextPeriod = 0;
	private int m_CurrentFps;
	const string display = "{0} FPS";
	private string m_text;
	private GUIStyle m_style;
	private GUIStyle m_style1;

	private bool m_bShowInfo = false;

	//存储临时字符串
	private StringBuilder info = new StringBuilder(string.Empty);


	private void Start() {

		m_style = new GUIStyle ();
		m_style.normal.background = null;
		m_style.normal.textColor = Color.red;
		m_style.fontSize = 30;


		m_style1 = new GUIStyle ();
		m_style1.normal.textColor = Color.blue;
		m_style1.fontSize = 30;
		m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
		//CollectHardDriveInfo();
	}

	private Rect m_FpsRect = new Rect(Screen.width - 300, 0, 300, 100);
    private Rect m_VerRect = new Rect(350, 0, 200, 50);

    void OnGUI(){
        GUI.Label(new Rect(Screen.width - 100, 0, 100, 30), "supportsInstancing:" + SystemInfo.supportsInstancing, m_style);
        GUI.Label(new Rect(Screen.width - 100, 100, 100, 30), "graphicsMultiThreaded:" + SystemInfo.graphicsMultiThreaded, m_style);
        /*if (m_bShowInfo == true)*/
        {
			GUI.Label(m_FpsRect, m_text ,m_style);
		}

	//	m_bShowInfo = GUI.Toggle (new Rect (200, 0, 150, 60), m_bShowInfo,m_bShowInfo ? "隐藏信息": "显示信息", m_style1);


		/*string[] names = QualitySettings.names;
		GUILayout.BeginVertical();
		int i = 0;
		while (i < names.Length) {
			if (GUILayout.Button (names [i])) {
				QualitySettings.SetQualityLevel (i, true);
			}
			i++;
		}
		GUILayout.EndVertical();*/

		//ConstantData.ABServerPath
    }


	void CollectHardDriveInfo() {
		//将输出文本框置空

		info.AppendLine("设备与系统信息:");

		//设备的模型

		//GetMessage("设备模型", SystemInfo.deviceModel);

		//设备的名称

		//GetMessage("设备名称", SystemInfo.deviceName);


		//设备的类型

		//GetMessage("设备类型（PC电脑，掌上型）", SystemInfo.deviceType.ToString());

		//系统内存大小

		GetMessage("系统内存大小MB", SystemInfo.systemMemorySize.ToString());

		//操作系统

		GetMessage("操作系统", SystemInfo.operatingSystem);

		//设备的唯一标识符

		GetMessage("设备唯一标识符", SystemInfo.deviceUniqueIdentifier);

		//显卡设备标识ID

		GetMessage("显卡ID", SystemInfo.graphicsDeviceID.ToString());

		//显卡名称

		GetMessage("显卡名称", SystemInfo.graphicsDeviceName);

		//显卡类型

		GetMessage("显卡类型", SystemInfo.graphicsDeviceType.ToString());

		//显卡供应商

		GetMessage("显卡供应商", SystemInfo.graphicsDeviceVendor);

		//显卡供应唯一ID

		GetMessage("显卡供应唯一ID", SystemInfo.graphicsDeviceVendorID.ToString());

		//显卡版本号

		GetMessage("显卡版本号", SystemInfo.graphicsDeviceVersion);

		//显卡内存大小

		GetMessage("显存大小MB", SystemInfo.graphicsMemorySize.ToString());

		//显卡是否支持多线程渲染

		GetMessage("显卡是否支持多线程渲染", SystemInfo.graphicsMultiThreaded.ToString());

		//支持的渲染目标数量

		GetMessage("支持的渲染目标数量", SystemInfo.supportedRenderTargetCount.ToString());

		GetMessage("显卡内存大小", SystemInfo.graphicsMemorySize.ToString());

		GetMessage("处理器类型", SystemInfo.processorType);

		GetMessage("屏幕宽：", Screen.width.ToString());

		GetMessage("屏幕高：", Screen.height.ToString());
	}

	void GetMessage(params string[] str) {
		if (str.Length == 2) {
			info.AppendLine(str[0] + ":" + str[1]);
		}
	}

	private void Update() {
		//measure average frames per second
		m_FpsAccumulator++;
		if (Time.realtimeSinceStartup > m_FpsNextPeriod) {
			m_CurrentFps = (int)(m_FpsAccumulator / fpsMeasurePeriod);
			m_FpsAccumulator = 0;
			m_FpsNextPeriod += fpsMeasurePeriod;
			m_text = "FPS:" + m_CurrentFps;
		}
    }
}
