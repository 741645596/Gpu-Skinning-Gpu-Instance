using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 加载队列管理
/// </summary>
/// <author>zhulin</author>
public class LoadQueue
{
    //最大支持的加载数量
    private static int s_MaxTask = 8;
    // 加载任务列表
    private static List<LoadTask> s_LoadTasks = new List<LoadTask>();

    public static void AddLoadTask(string ABRelativePath,
        string ObjName,
        System.Type type,
        bool IsCacheAsset,
        bool IsCacheAB,
        bool IsFreeUnUseABRes,
        AssetLoadHook pfun)
    {
        LoadTask task = new LoadTask(ABRelativePath, ObjName, type, IsCacheAsset, IsCacheAB, IsFreeUnUseABRes, pfun);
        AddLoadQueue(task);
    }

    private static void AddLoadQueue(LoadTask task)
    {
        s_LoadTasks.Add(task);
        StartRunTask();
    }

    // 开始加载任务
    public static void StartRunTask()
    {

        List<LoadTask> runTasks = GetRunTask();
        int runTaskCount = runTasks.Count;
        if (s_LoadTasks.Count < 1 || runTaskCount >= s_MaxTask)
        {
            return;
        }

        // 添加加载任务
        List<LoadTask> unRunTasks = GetUnRunTask();
        for (int i = 0; i < unRunTasks.Count; ++i)
        {
            LoadTask task = unRunTasks[i];
            if (task == null || task.IsRun == true)
            {
                continue;
            }

            if (runTaskCount < s_MaxTask)
            {
                if (CheckCanRun(runTasks, task) == true)
                {
                    task.StartTask();
                    runTasks.Add(task);
                    ++runTaskCount;
                }
            }
            else
            {
                break;
            }
        }
    }

    // 确定该任务能否现在执行，主要是担心2个相同任务加载同一个ab 包，或依赖ab 包的问题。
    private static bool CheckCanRun(List<LoadTask> tasks, LoadTask t)
    {
        if (t == null)
        {
            return false;
        }

        for (int i = 0; i < tasks.Count; ++i)
        {
            LoadTask task = tasks[i];
            if (task != null && task.IsRun == true)
            {
                if (task.EqualRunTask(t))
                    return false;
            }
        }

        return true;
    }

    // 获取未运行的任务
    private static List<LoadTask> GetUnRunTask()
    {
        return GetTasks(false);
    }

    //获取已运行的任务
    private static List<LoadTask> GetRunTask()
    {
        return GetTasks(true);
    }

    private static List<LoadTask> GetTasks(bool run)
    {
        List<LoadTask> l = new List<LoadTask>();

        for (int i = 0; i < s_LoadTasks.Count; ++i)
        {
            LoadTask task = s_LoadTasks[i];
            if (task != null && task.IsRun == run)
            {
                l.Add(task);
            }
        }

        return l;
    }

    public static void NotifyTaskFinish(LoadTask t)
    {
        if (t != null)
        {
            s_LoadTasks.Remove(t);
        }
        StartRunTask();
    }

}

// 加载任务
public class LoadTask
{
    public string m_ABRelativePath;
    public string m_ObjName;
    public System.Type m_type;
    public bool m_IsCacheAsset;
    public bool m_IsCacheAB;
    public bool m_IsFreeUnUseABRes;
    public AssetLoadHook m_fun;
    private bool m_IsRun = false;
    public bool IsRun
    {
        get { return m_IsRun; }
    }

    public LoadTask(string ABRelativePath,
        string ObjName,
        System.Type type,
        bool IsCacheAsset,
        bool IsCacheAB,
        bool IsFreeUnUseABRes,
        AssetLoadHook pfun)
    {

        m_ABRelativePath = ABRelativePath;
        m_ObjName = ObjName;
        m_type = type;
        m_IsCacheAsset = IsCacheAsset;
        m_IsCacheAB = IsCacheAB;
        m_IsFreeUnUseABRes = IsFreeUnUseABRes;
        m_fun = pfun;
    }


    /// <summary>
    /// 开始任务
    /// </summary>
    public void StartTask()
    {
        m_IsRun = true;
        ResourceManger.AsyncGo.StartCoroutine(ABLoad.LoadABasync(m_ABRelativePath, m_IsCacheAB,
            (ab) =>
            {
                AssetLoadRun run = ResourceManger.gAssetLoad as AssetLoadRun;
                ResourceManger.AsyncGo.StartCoroutine(run.LoadObjAsync(ab, m_ObjName, m_type, m_IsCacheAB, m_IsFreeUnUseABRes,
                    (g) =>
                    {
                        run.LoadAssetCallBack(g, m_ABRelativePath + m_ObjName, m_IsCacheAsset, m_fun);
                        FinishTask();
                    }));
            }));
    }


    private void FinishTask()
    {
        //Debug.Log("加载完成！");
        LoadQueue.NotifyTaskFinish(this);
    }

    /// <summary>
    ///  正在运行的任务不能为相同的ab包。
    /// </summary>
    public bool EqualRunTask(LoadTask task)
    {
        if (task == null)
            return false;
        if (string.Compare(this.m_ABRelativePath, task.m_ABRelativePath) == 0)
            return true;

        return false;
    }
}

