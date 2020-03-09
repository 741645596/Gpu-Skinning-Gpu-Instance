using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Jobs;
public class CombineMeshAniAction : MonoBehaviour
{
    // Start is called before the first frame update
    public MeshFilter mf;
    private Mesh mesh;
    public float SpeedParam = 0.1f;
    public int Count = 0;
    private NativeArray<MeshQuad> Listpt ;
    private Vector3[] Listvv;
    MeshJob m_Job;
    JobHandle m_JobHandle;
    void Start()
    {
        mesh = mf.mesh;
        Count = mesh.vertices.Length / 4;
        Listvv = new Vector3[Count * 4];
        Listpt = new NativeArray<MeshQuad>(Count, Allocator.Persistent);
        for (int i = 0; i < Count; i++)
        {
            MeshQuad s = new MeshQuad();
            s.SetA(mesh.vertices[i * 4]);
            s.SetB(mesh.vertices[i * 4 + 1]);
            s.SetC(mesh.vertices[i * 4 + 2]);
            s.SetD(mesh.vertices[i * 4 + 3]);
            Listpt[i] = s;
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_Job = new MeshJob()
        {
            deltaTime = Time.deltaTime,
            moveSpeed = Vector3.right * SpeedParam,
            ListV = Listpt,
            Length = Count
        };
        m_JobHandle = m_Job.Schedule(Count, 64);
    }

    public void LateUpdate()
    {
        m_JobHandle.Complete();
        for (int i = 0; i < Count; i++)
        {
            Listvv[i * 4] = Listpt[i].a;
            Listvv[i * 4 + 1] = Listpt[i].b;
            Listvv[i * 4 + 2] = Listpt[i].c;
            Listvv[i * 4 + 3] = Listpt[i].d;
        }
        mesh.vertices = Listvv;
        mf.mesh = mesh;
    }


    struct MeshJob : IJobParallelFor
    {
        public NativeArray<MeshQuad> ListV;
        [ReadOnly]
        public Vector3 moveSpeed;
        [ReadOnly]
        public float deltaTime;
        [ReadOnly]
        public int Length;

        public void Execute(int i)
        {
            Vector3 ms = moveSpeed * i * deltaTime;
            MeshQuad s = ListV[i];
            s.Move(ms);
            ListV[i] = s ;
        }
    }

    void OnDestroy()
    {
        Listpt.Dispose();
    }
}



struct MeshQuad
{
    public Vector3 a;
    public Vector3 b;
    public Vector3 c;
    public Vector3 d;

    public void SetA(Vector3 s)
    {
        this.a = s;
    }
    public void SetB(Vector3 s)
    {
        this.b = s;
    }
    public void SetC(Vector3 s)
    {
        this.c = s;
    }
    public void SetD(Vector3 s)
    {
        this.d = s;
    }

    public void Move(Vector3 ms)
    {
        a += ms;
        b += ms;
        c += ms;
        d += ms;
    }
}
