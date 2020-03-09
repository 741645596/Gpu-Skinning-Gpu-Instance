using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Jobs;
struct TransformJob : IJobParallelFor
{
    public NativeArray<Vector3> position;
    public NativeArray<Quaternion> rotate ;
    //private NativeArray<Vector3> scale;
    [ReadOnly]
    public Vector3 MoveSpeed;
    [ReadOnly]
    public Quaternion RotateSpeed;
    [ReadOnly]
    public int frame;
    //[ReadOnly]
    //public Vector3 scale;

    // The code actually running on the job
    public void Execute(int i)
    {
        position[i] += MoveSpeed;
        rotate[i] *= RotateSpeed ;
        TestInstance.instance.ChangePerObjectData(i, position[i], rotate[i].normalized, Vector3.one, frame);
        //TestInstance.instance.ChangePerObjectFlame(i,frame);
    }
}
public class TestInstance : IAnimation
{
    /// <summary>
    /// 移动速度
    /// </summary>
    public Vector3 moveSpeed;
    /// <summary>
    /// 旋转速度
    /// </summary>
    public Vector3 rotateSpeed;

    public Mesh targetMesh;
    public Material targetMaterial;
    public float m_w = 100;
    public float m_h = 100;
    public static GPUInstanceDrawer instance = null;
    private int frame = 0;
    private int m_count = 0;

    public NativeArray<Vector3> _pos;
    private NativeArray<Quaternion> _Rotate;
    // Start is called before the first frame update
    void Start()
    {
        instance = new GPUInstanceDrawer();
        instance.Init(targetMesh, targetMaterial, true);
        LoadPrefab();
        Play(ListAniClip[0].clipName);
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (instance != null)
        {
            instance.RenderInstnaces();
        }
    }

    public override void DoAnim(int Frame)
    {
        if (instance == null || m_count == 0)
            return;
        // Initialize the job data
        var job = new TransformJob()
        {
            frame = Frame,
            rotate = _Rotate,
            position = _pos,
            MoveSpeed = moveSpeed * Time.deltaTime,
            RotateSpeed = Quaternion.Euler(rotateSpeed * Time.deltaTime)
        };
        JobHandle jobHandle = job.Schedule(m_count, 64);
        jobHandle.Complete();
    }

    private void LoadPrefab()
    {
        m_count = 0;
        if (_pos != null&& _pos.Length > 0)
        {
            _pos.Dispose();
        }
        if (_Rotate != null && _Rotate.Length > 0)
        {
            _Rotate.Dispose();
        }
        instance.ClearAll();
        float dx = 6.0f;
        float dy = 6.0f;
        int w = (int)m_w;
        int h = (int)m_h;
        int k = 0;
        m_count = w * h;
        _pos = new NativeArray<Vector3>(m_count, Allocator.Persistent);
        _Rotate = new NativeArray<Quaternion>(m_count, Allocator.Persistent);
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                _pos[k] = new Vector3((i - w / 2) * dx, 0.0f, (j - h / 2) * dy);
                _Rotate[k] = Quaternion.AngleAxis(-90, Vector3.right);
                instance.AddObject(_pos[k],  _Rotate[k], Vector3.one, 0);
                k++;
            }
        }
        
    }

    void OnDestroy()
    {
        if (_pos != null && _pos.Length > 0)
        {
            _pos.Dispose();
        }
        if (_Rotate != null && _Rotate.Length > 0)
        {
            _Rotate.Dispose();
        }
    }
}
