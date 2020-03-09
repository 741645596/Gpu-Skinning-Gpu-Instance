using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleInstance : MonoBehaviour
{
    public ParticleSystem m_ParticleSystem;
    public ParticleSystemRenderer m_ParticleSystemRender;
    public Material targetMaterial;
    public Mesh targetMesh;
    public ParticleSystem.Particle []m_ObjParticles ;
    [SerializeField]
    private float _stepSize = 1.5f;
    [SerializeField]
    private int _rowCount = 50;
    [SerializeField]
    private int _columnCount = 50;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        ChangeTransform();
    }

    public void Init()
    {
        ParticleSystem.MainModule main = m_ParticleSystem.main;
        main.maxParticles = 10000;
        main.startLifetime = 10000000.0f;
        main.simulationSpeed = 0.0f;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
        //
        if (m_ParticleSystemRender == null)
        {
            m_ParticleSystemRender = m_ParticleSystem.GetComponent<ParticleSystemRenderer>();
        }
        main.startSizeMultiplier = 1.0f;
        m_ParticleSystemRender.sortMode = ParticleSystemSortMode.None;
        m_ParticleSystemRender.renderMode = ParticleSystemRenderMode.Mesh;
        m_ParticleSystemRender.material = targetMaterial;
        m_ParticleSystemRender.mesh = targetMesh;
        m_ParticleSystemRender.alignment = ParticleSystemRenderSpace.Local;
        m_ParticleSystemRender.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        m_ParticleSystemRender.receiveShadows = false;
        ParticleSystem.EmissionModule emissionModule = m_ParticleSystem.emission;
        emissionModule.rateOverTime = 0;
        m_ParticleSystem.Emit(main.maxParticles);
        m_ParticleSystem.Stop();
        m_ParticleSystem.Pause();
    }

    public void ChangeTransform()
    {
        if (m_ObjParticles == null || m_ObjParticles.Length < _rowCount * _columnCount)
        {
            m_ObjParticles = new ParticleSystem.Particle[_rowCount * _columnCount];
        }
        int numParticleAlive = m_ParticleSystem.GetParticles(m_ObjParticles);
        for (int i = 0; i < numParticleAlive; i++)
        {
            Vector3 v = new Vector3((i/ _rowCount - _rowCount * 0.5f) * _stepSize, 0, (i % _columnCount - _columnCount * 0.5f) * _stepSize);
            m_ObjParticles[i].position = v;
            //m_ObjParticles[i].rotation3D =new Vector3(-90.0f, 0.0f, 0.0f);
        }
        m_ParticleSystem.SetParticles(m_ObjParticles, _rowCount * _columnCount);
    }
}
