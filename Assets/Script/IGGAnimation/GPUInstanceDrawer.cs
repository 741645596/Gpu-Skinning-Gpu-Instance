using UnityEngine;
using UnityEngine.Rendering;

public class GPUInstanceDrawer
{
    //native array is a reference type
    public Matrix4x4[] GetMatrixArray() { return _matrices; }
    public float[] GetFlameArray() { return _flames; }
    public Material GetDestMaterial() { return _destMaterial; }
    public int GetAvailableCount() { return _availableObjectCount; }

    public virtual bool Init(Mesh targetMesh, Material targetMaterial, bool enablePropertyBlock,
        ShadowCastingMode shadowCastMode = ShadowCastingMode.Off,
        bool isReceiveShadow = false)
    {
        if ((!targetMesh) || (!targetMaterial))
            return false;

        _shadowCastMode = shadowCastMode;
        _isReceiveShadow = isReceiveShadow;

        _targetMesh = targetMesh;
        _destMaterial = new Material(targetMaterial);
        _destMaterial.enableInstancing = true;
        if (enablePropertyBlock)
            _propBlock = new MaterialPropertyBlock();

        return true;
    }

    public virtual bool Release()
    {
        GameObject.DestroyImmediate(_destMaterial);
        _destMaterial = null;
        return true;
    }

    public void ClearAll()
    {
        _availableObjectCount = 0;
    }

    public void RenderInstnaces()
    {
        if ((!_targetMesh) || (!_destMaterial) || (_availableObjectCount <= 0))
            return;        

        int availableCount = 0;
        Vector3 prePoistion = Vector3.zero;
        for (int i = 0; i < _availableObjectCount; i+=1023)
        {
            availableCount = Mathf.Min(1023, _availableObjectCount - i);
            if (_propBlock != null)
            {
                System.Array.ConstrainedCopy(_flames, i, _flames1023, 0, Mathf.Min(1023, availableCount));
                _propBlock.SetFloatArray("_frame", _flames1023);
                ProcessExtendPropertyBlock(i, availableCount);
            }
            System.Array.ConstrainedCopy(_matrices, i, _matrices1023, 0, Mathf.Min(1023, availableCount));
            if (_propBlock != null)
                Graphics.DrawMeshInstanced(_targetMesh, 0, _destMaterial, _matrices1023, availableCount, _propBlock,
                    _shadowCastMode, _isReceiveShadow);
            else
                Graphics.DrawMeshInstanced(_targetMesh, 0, _destMaterial, _matrices1023, availableCount, null,
                    _shadowCastMode, _isReceiveShadow);
        }
    }
    protected virtual void ProcessExtendPropertyBlock(int index, int availableCount) { }


    public int AddObject(Vector3 position,  Quaternion rotation,  Vector3 scaleFactor,  int flame)
    {
        if (_availableObjectCount >= MAX_COUNT)
            return -1;
        ++_availableObjectCount;
        int destIndex = _availableObjectCount - 1;
        _matrices[destIndex].SetTRS(position, rotation, scaleFactor);
        _flames[destIndex] = flame;
        return destIndex;
    }


    public bool RemoveObject(int removeIndex)
    {
        if ((removeIndex < 0) || (removeIndex >= _availableObjectCount))
            return false;

        --_availableObjectCount;
        int destIndex = removeIndex + 1;
        System.Array.ConstrainedCopy(_matrices, destIndex, _matrices, removeIndex, _availableObjectCount);
        System.Array.ConstrainedCopy(_flames, destIndex, _flames, removeIndex, _availableObjectCount);        

        return true;
    }

    public bool ChangePerObjectFlame(int index,  int flame)
    {
        if ((index >= _availableObjectCount) || (index >= _flames.Length))
            return false;
        _flames[index] = flame;
        return true;
    }



    public bool ChangePerObjectData(int index,  Vector3 position,  Quaternion rotation,
         Vector3 scaleFactor,  int flame)
    {
        if ((index >= _availableObjectCount) || (index >= _flames.Length))
            return false;
        _matrices[index].SetTRS(position, rotation, scaleFactor);
        _flames[index] = flame;
        return true;
    }

    protected const int MAX_COUNT = 50000;
    private Mesh _targetMesh = null;
    private Matrix4x4[] _matrices = new Matrix4x4[MAX_COUNT];
    private Matrix4x4[] _matrices1023 = new Matrix4x4[1023];
    protected int _availableObjectCount = 0;
    private Material _destMaterial = null;

    protected MaterialPropertyBlock _propBlock = null;
    private float[] _flames = new float[MAX_COUNT];
    private float[] _flames1023 = new float[1023];

    private ShadowCastingMode _shadowCastMode = ShadowCastingMode.Off;
    private bool _isReceiveShadow = false;
}
