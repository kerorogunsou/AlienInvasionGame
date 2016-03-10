using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// counts from 0 to duration
/// </summary>
/// 
class CustomizeableTimer {
    int tick;
    int duration;
    /// <summary>
    /// (if dur < 0 count() will always return true)
    /// </summary>
    public CustomizeableTimer(int duration) {
        this.duration = duration;
    }
    /// <summary>
    /// does 1 tick and returns true if finished
    /// </summary>
    public bool Count() {
        if (tick > duration) {
            return true;
        }
        tick++;
        return false;
    }
    public void Reset() {
        tick = 0;
    }
}

public class ObjectPool {
    protected List<GameObject> pool;
    protected string aliveTag;
    protected string deadTag;
    protected string freeTag;
    public delegate void DeathSignalingMethod();
    public event DeathSignalingMethod signalDeath;
    public ObjectPool(int count, GameObject prefab, Vector3 position, string freeTag, string aliveTag, string deadTag) {

        pool = new List<GameObject>();
        this.aliveTag = aliveTag;
        this.deadTag = deadTag;
        this.freeTag = freeTag;
        for (int i = 0; i < count; i++) {
            pool.Add(GameObject.Instantiate(prefab, position, Quaternion.identity) as GameObject);
            InitPoolObject(pool[i]);
        }
    }
    protected virtual void InitPoolObject(GameObject obj) {
        obj.SetActive(false);
        obj.tag = this.freeTag;
    }
    public int IstantiateIfFree(Vector3 position) {
        for (int i = 0; i < pool.Count; i++) {
            if (pool[i].tag == freeTag) {
                pool[i].transform.position = position;
                pool[i].SetActive(true);
                pool[i].tag = aliveTag;
                return i;
            }
        }
        return -1;
    }
    public void CheckDeads() {
        for (int i = 0; i < pool.Count; i++) {
            if (pool[i].tag == deadTag) {
                if (pool[i].activeSelf) {
                    pool[i].SetActive(false);
                }
                pool[i].transform.rotation = Quaternion.identity;
                pool[i].tag = freeTag;
                if (signalDeath != null) {
                    signalDeath();
                }
            }
        }
    }
    public GameObject GetByIndex(int index) {
        if (pool != null && index < pool.Count) {
            return pool[index];
        }
        return null;
    }
}
public class TransportPool : ObjectPool {
    string killedTag;
    tank_script objectScript;
    tank_script.shoot_delegate shootFunction;
    public TransportPool(int count, GameObject prefab, Vector3 position, string freeTag, string aliveTag, string deadTag, string killedTag, tank_script.shoot_delegate shootFunction) :
    base(count, prefab, position, freeTag, aliveTag, deadTag) {
        this.killedTag = killedTag;
        this.shootFunction = shootFunction;
    }
    protected override void InitPoolObject(GameObject obj) {
        obj.SetActive(false);
        obj.tag = freeTag;
        objectScript = obj.GetComponent<tank_script>();
        objectScript.shoot = shootFunction;
    }
    
}


