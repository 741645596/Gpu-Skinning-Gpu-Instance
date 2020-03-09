using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorTeam : ActorTeamBase
{
    private List<ActorCtrlInfo> m_ListActor = null;
    /// <summary>
    /// 是否需要更新
    /// </summary>
    private bool m_isNeedUpdate = false;

    /// <summary>
    /// 测试数据
    /// </summary>
    /*void Start()
    {
        List<Vector3> listPos = new List<Vector3>(new Vector3[10]);
        List< float > listAngle = new List<float>(new float[10]);
        List<int> listAni = new List<int>(new int[10]);
        for (int i = 0; i < 10; i++)
        {
            listPos[i] = new Vector3((i - 5) * 0.5f, 0, 0);
            listAngle[i] = i * 45.0f;
            listAni[i] = 0;
        }
        InitActorInfo(listPos, listAngle, listAni);
    }*/

    /// <summary>
    /// 初始化军团信息
    /// </summary>
    /// <param name="listPos"></param>
    /// <param name="listAngle"></param>
    /// <param name="listAni"></param>
    public void InitActorInfo(List<Vector3> listPos, List<float> listAngle, List<int> listAni)
    {
        this.ActorNum = listPos.Count;
        if (m_ListActor == null)
        {
            m_ListActor = new List<ActorCtrlInfo>(new ActorCtrlInfo[this.ActorNum]);
        }
        Debug.Log("count;" + m_ListActor.Count);
        for (int i = 0; i < this.ActorNum; i++)
        {
            ActorCtrlInfo v = new ActorCtrlInfo();
            v.Pos = listPos[i];
            v.Dir = Mathf.CeilToInt(listAngle[i] / 8);
            v.AniState = listAni[i];
            m_ListActor[i] = v;
        }
        Init(gameObject.GetComponent<ParticleSystem>(), ActorNum, _targetMesh, _targetMaterial);
        m_isNeedUpdate = true;
    }

    /// <summary>
    /// 更新军团信息
    /// </summary>
    /// <param name="listPos"></param>
    /// <param name="listAngle"></param>
    /// <param name="listAni"></param>
    public void UpdateActorInfo(List<Vector3> listPos, List<float> listAngle, List<int> listAni)
    {
        for (int i = 0; i < this.ActorNum; i++)
        {
            m_ListActor[i].Pos = listPos[i];
            m_ListActor[i].Dir = Mathf.CeilToInt(listAngle[i] / 8);
            m_ListActor[i].AniState = listAni[i];
        }
        m_isNeedUpdate = true;
    }
    
    // Update is called once per frame
    protected virtual void Update()
    {
        if (m_isNeedUpdate == true)
        {
            UpdateParticleData();
            m_isNeedUpdate = false;
        }
    }

    /// <summary>
    /// 更新数据
    /// </summary>
    private void UpdateParticleData()
    {
        if (m_ListActor == null)
            return;
        for (int i = 0; i < ActorNum && i < m_ListActor.Count; ++i)
        {
            SetParticleData(i, m_ListActor[i].Pos, m_ListActor[i].Dir);
        }
        ApplyTransforms();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (m_ListActor != null)
        {
            m_ListActor.Clear();
            m_ListActor = null;
        }
    }

}

/// <summary>
/// 军团小兵控制信息
/// </summary>
public class ActorCtrlInfo
{
    /// <summary>
    /// 位置
    /// </summary>
    public Vector3 Pos;
    /// <summary>
    /// 方向
    /// </summary>
    public int Dir;
    /// <summary>
    /// 动画状态，到时换成枚举
    /// </summary>
    public int AniState;
}
