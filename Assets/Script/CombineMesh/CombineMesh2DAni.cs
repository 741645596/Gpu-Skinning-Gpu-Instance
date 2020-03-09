using UnityEngine;
using Unity.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine.Jobs;



public class CombineMesh2DAni : MonoBehaviour
{
    private List<GameObject> m_Listgo = new List<GameObject>();
    public float m_w = 10;
    public float m_h = 10;
    public GameObject frameAni;


    public Vector3 m_Acceleration = new Vector3(0.0002f, 0.0001f, 0.0002f);
    public Vector3 m_AccelerationMod = new Vector3(.0001f, 0.0001f, 0.0001f);

    NativeArray<Vector3> m_Velocities;
    TransformAccessArray m_TransformsAccessArray;

    PositionUpdateJob m_Job;
    AccelerationJob m_AccelJob;

    JobHandle m_PositionJobHandle;
    JobHandle m_AccelJobHandle;

    void Start()
    {
        LoadPrefab(frameAni);
        List<Transform> lt = new List<Transform>();
        for (int i = 0; i < m_Listgo.Count; i++)
        {
            lt.Add(m_Listgo[i].transform);
        }
        m_Velocities = new NativeArray<Vector3>(m_Listgo.Count, Allocator.Persistent);
        m_TransformsAccessArray = new TransformAccessArray(lt.ToArray());
    }

    private void LoadPrefab(GameObject prefab)
    {
        if (m_Listgo != null && m_Listgo.Count > 0)
        {
            foreach (GameObject g in m_Listgo)
            {
                GameObject.Destroy(g);
            }
        }
        m_Listgo.Clear();
        //

        if (prefab != null)
        {
            float dx = 10.0f;
            float dy = 10.0f;
            int w = (int)m_w;
            int h = (int)m_h;
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    GameObject go = Instantiate(prefab);
                    //go.transform.parent = GameObject.Find("ActorNode").transform;
                    Vector3 v = new Vector3((i - w / 2) * dx, 0, (j - h / 2) * dy) + go.transform.localPosition;
                    go.transform.localPosition = v;
                    m_Listgo.Add(go);
                }
            }

        }
    }

    void OnDestroy()
    {
        if (m_Listgo != null && m_Listgo.Count > 0)
        {
            foreach (GameObject g in m_Listgo)
            {
                GameObject.Destroy(g);
            }
        }
        m_Listgo.Clear();
        m_Velocities.Dispose();
        m_TransformsAccessArray.Dispose();
    }


    public void Update()
    {
        m_AccelJob = new AccelerationJob()
        {
            deltaTime = Time.deltaTime,
            velocity = m_Velocities,
            acceleration = m_Acceleration,
            accelerationMod = m_AccelerationMod
        };

        m_Job = new PositionUpdateJob()
        {
            deltaTime = Time.deltaTime,
            velocity = m_Velocities,
        };

        m_AccelJobHandle = m_AccelJob.Schedule(m_Listgo.Count, 64);
        m_PositionJobHandle = m_Job.Schedule(m_TransformsAccessArray, m_AccelJobHandle);
    }

    public void LateUpdate()
    {
        m_PositionJobHandle.Complete();
    }


    struct AccelerationJob : IJobParallelFor
    {
        public NativeArray<Vector3> velocity;

        public Vector3 acceleration;
        public Vector3 accelerationMod;

        public float deltaTime;

        public void Execute(int i)
        {
            velocity[i] += (acceleration + i * accelerationMod) * deltaTime;
        }
    }
    struct PositionUpdateJob : IJobParallelForTransform
    {
        [ReadOnly]
        public NativeArray<Vector3> velocity;  // the velocities from AccelerationJob

        public float deltaTime;

        public void Execute(int i, TransformAccess transform)
        {
            transform.position += velocity[i] * deltaTime;
        }
    }
}
