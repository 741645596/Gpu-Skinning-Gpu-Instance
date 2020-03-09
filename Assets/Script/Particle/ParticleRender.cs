using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 技术来源
///  https://docs.unity3d.com/ScriptReference/ParticleSystemRenderer.SetActiveVertexStreams.html
/// </summary>
public class ParticleRender
{
    private List<ParticleSystemVertexStream> _activeStreams = new List<ParticleSystemVertexStream>();
    public bool Init(ParticleSystem ps, int count, Mesh targetMesh, Material targetMaterial)
    {
        m_ParticleSys = ps;
        _count = count;

        var Main = m_ParticleSys.main;
        Main.maxParticles = _count;
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
        m_ParticleSys.Emit(_count);
        m_ParticleSys.Stop();
        m_ParticleSys.Pause();
        return true;
    }    

    public bool SetParticleData(int index, int frameIndex, Vector3 position,
         Vector3 rotation)
    {
        if (index >= _count)
            return false;

        if (m_ObjectParticles == null || m_ObjectParticles.Length < _count)
            m_ObjectParticles = new ParticleSystem.Particle[_count];

        //每個frame只能call一次…
        if (_aliveCount < 0)
        {
            _aliveCount = m_ParticleSys.GetParticles(m_ObjectParticles);
            m_ParticleSys.GetCustomParticleData(customData, ParticleSystemCustomData.Custom1);
        }

        m_ObjectParticles[index].position = position;
        m_ObjectParticles[index].rotation3D = rotation;

        customData[index] = new Vector4(frameIndex, 0.0f, 0.0f, 0.0f);

        return true;
    }

    public void ApplyTransforms()
    {
        if ((m_ParticleSys != null) && (m_ObjectParticles != null))
        {
            m_ParticleSys.SetParticles(m_ObjectParticles, _count);
            //m_ParticleSys.SetCustomParticleData(customData, ParticleSystemCustomData.Custom1);
            m_ParticleSys.SetCustomParticleData(customData, ParticleSystemCustomData.Custom1);
        }
        _aliveCount = -1;
    }

    public ParticleSystem GetParticleSystem() { return m_ParticleSys; }

    private int _count = 0;
    private int _aliveCount = -1;
    private ParticleSystem m_ParticleSys = null;
    private ParticleSystemRenderer m_ParticleSysRender = null;
    protected ParticleSystem.Particle[] m_ObjectParticles = null;

    private List<Vector4> customData = new List<Vector4>();
}