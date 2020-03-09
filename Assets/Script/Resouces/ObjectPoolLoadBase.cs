using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ObjectPoolLoadBase {

    protected string g_SceneName;
    protected bool g_AutoCreatePrefab;

    public string SceneName {
        get {
            return g_SceneName;
        }
    }


    public virtual void Init() {
        
    }

    public virtual void InitSceneCache(string sceneName, bool autoCreatePrefab) {
        this.g_SceneName = sceneName;
        this.g_AutoCreatePrefab = autoCreatePrefab;
    }

    public virtual void ClearSceneCache() {

    }

    public virtual void PreparePrefab(string dataName) {

    }

    public virtual GameObject SpawnGameObjectFromPool(string prefabName, string type) {
        return null;
    }

    public virtual void DespawnGameObjectToPool(GameObject target, string type) {

    }

    public virtual bool HasPoolCached(string prefabName, string type) {
        return false;
    }

    public virtual void Destory() {

    }
}
