using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 技术来源
///  https://docs.unity3d.com/ScriptReference/ParticleSystemRenderer.SetActiveVertexStreams.html
/// </summary>

public class ActorTeamBase : MonoBehaviour
{
    /// <summary>
    /// 单个小兵模型，默认使用quad
    /// </summary>
    public Mesh _targetMesh;
    /// <summary>
    /// 单个小兵材质
    /// </summary>
    public Material _targetMaterial;
    /// <summary>
    /// 小兵数量
    /// </summary>
    protected int ActorNum = 50;
    private int _aliveCount = -1;

    private ParticleSystem m_ParticleSys = null;
    private ParticleSystemRenderer m_ParticleSysRender = null;
    protected ParticleSystem.Particle[] m_ObjectParticles = null;
    private List<Vector4> customData = new List<Vector4>();
    private List<ParticleSystemVertexStream> _activeStreams = new List<ParticleSystemVertexStream>();



    protected void Init(ParticleSystem ps, int count, Mesh targetMesh, Material targetMaterial)
    {
        m_ParticleSys = ps;
        var Main = m_ParticleSys.main;
        Main.maxParticles = count;
        Main.startLifetime = 10000000.0f;
        Main.simulationSpeed = 0.0f;
        Main.startSpeed = 0.0f;
        Main.simulationSpace = ParticleSystemSimulationSpace.Local;

        m_ParticleSysRender = m_ParticleSys.GetComponent<ParticleSystemRenderer>();
        Main.startSizeMultiplier = 1.0f;
        m_ParticleSysRender.sortMode = ParticleSystemSortMode.None;
        m_ParticleSysRender.renderMode = ParticleSystemRenderMode.Mesh;
        m_ParticleSysRender.material = targetMaterial;
        m_ParticleSysRender.mesh = targetMesh;
        m_ParticleSysRender.alignment = ParticleSystemRenderSpace.Local;
        m_ParticleSysRender.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        m_ParticleSysRender.receiveShadows = false;

        // send custom data to the shader
        m_ParticleSysRender.SetActiveVertexStreams(new List<ParticleSystemVertexStream>(
            new ParticleSystemVertexStream[] { ParticleSystemVertexStream.Position,
                ParticleSystemVertexStream.UV,
                ParticleSystemVertexStream.Custom1X }));

        Main.startSizeMultiplier = 0.5f;
        // Get the emission module.
        var EmissionModule = m_ParticleSys.emission;
        EmissionModule.rateOverTime = 0;
        m_ParticleSys.Emit(ActorNum);
        m_ParticleSys.Stop();
        m_ParticleSys.Pause();
    }

    /// <summary>
    /// 更新小兵的数据
    /// </summary>
    /// <param name="index"></param>
    /// <param name="frameIndex"></param>
    /// <param name="position"></param>
    /// <param name="dir"></param>
    /// <returns></returns>
    protected bool SetParticleData(int index, Vector3 position, int framedir)
    {
        if (index >= ActorNum)
            return false;

        if (m_ObjectParticles == null || m_ObjectParticles.Length < ActorNum)
            m_ObjectParticles = new ParticleSystem.Particle[ActorNum];

        //每個frame只能call一次…
        if (_aliveCount < 0)
        {
            _aliveCount = m_ParticleSys.GetParticles(m_ObjectParticles);
            m_ParticleSys.GetCustomParticleData(customData, ParticleSystemCustomData.Custom1);
        }

        m_ObjectParticles[index].position = position;

        customData[index] = new Vector4(framedir, 0.0f, 0.0f, 0.0f);

        return true;
    }

    protected void ApplyTransforms()
    {
        if ((m_ParticleSys != null) && (m_ObjectParticles != null))
        {
            m_ParticleSys.SetParticles(m_ObjectParticles, ActorNum);
            m_ParticleSys.SetCustomParticleData(customData, ParticleSystemCustomData.Custom1);
        }
        _aliveCount = -1;
    }

    protected virtual void OnDestroy()
    {
    }


}
