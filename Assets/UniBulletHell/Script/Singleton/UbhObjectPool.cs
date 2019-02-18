// ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// *Please enable this define if you want to use the DarkTonic's CoreGameKit pooling system.
// ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

// #define USING_CORE_GAME_KIT

using System;
using System.Collections.Generic;
using UnityEngine;

#if USING_CORE_GAME_KIT
using DarkTonic.CoreGameKit;
#endif

/// <summary>
/// Ubh object pool.
/// </summary>
[AddComponentMenu("UniBulletHell/Manager/Object Pool")]
public class UbhObjectPool : UbhSingletonMonoBehavior<UbhObjectPool>
{
    [Serializable]
    private class InitializePool
    {
        public GameObject m_bulletPrefab = null;
        public int m_initialPoolNum = 0;
    }

    [SerializeField]
    private List<InitializePool> m_initializePoolList;

#if USING_CORE_GAME_KIT
    // +++++ Replace PoolingSystem with DarkTonic's CoreGameKit. +++++
    private PoolBoss m_poolBoss = null;

    /// <summary>
    /// Initialize CoreGameKit's PoolBoss
    /// </summary>
    private void InitializePoolBoss()
    {
        if (m_poolBoss == null)
        {
            // PoolBoss Initialize
            m_poolBoss = FindObjectOfType<PoolBoss>();
            if (m_poolBoss == null)
            {
                m_poolBoss = new GameObject(typeof(PoolBoss).Name).AddComponent<PoolBoss>();
            }
            m_poolBoss.autoAddMissingPoolItems = true;
        }
    }
#else
    private List<int> m_pooledKeyList = new List<int>(256);
    private Dictionary<int, List<UbhBullet>> m_pooledBulletDic = new Dictionary<int, List<UbhBullet>>(256);
#endif

    protected override void Awake()
    {
        base.Awake();

#if USING_CORE_GAME_KIT
#else
        transform.hierarchyCapacity = 3000;
#endif

        if (m_initializePoolList != null && m_initializePoolList.Count > 0)
        {
            for (int i = 0; i < m_initializePoolList.Count; i++)
            {
                CreatePool(m_initializePoolList[i].m_bulletPrefab, m_initializePoolList[i].m_initialPoolNum);
            }
        }
    }

    /// <summary>
    /// Create object pool from prefab.
    /// </summary>
    public void CreatePool(GameObject goPrefab, int createNum)
    {
        for (int i = 0; i < createNum; i++)
        {
            UbhBullet bullet = GetBullet(goPrefab, Vector3.zero, Quaternion.identity, true);
            if (bullet == null)
            {
                break;
            }
            ReleaseBullet(bullet);
        }
    }

    /// <summary>
    /// Get Bullet from object pool or instantiate.
    /// </summary>
    public UbhBullet GetBullet(GameObject goPrefab, Vector3 position, Quaternion rotation, bool forceInstantiate = false)
    {
        if (goPrefab == null)
        {
            return null;
        }

        UbhBullet bullet = null;

#if USING_CORE_GAME_KIT
        // +++++ Replace PoolingSystem with DarkTonic's CoreGameKit. +++++
        InitializePoolBoss();
        Transform trans = PoolBoss.Spawn(goPrefab.transform, position, rotation, transform);
        if (trans == null)
        {
            return null;
        }

        bullet = trans.gameObject.GetComponent<UbhBullet>();
        if (bullet == null)
        {
            bullet = trans.gameObject.AddComponent<UbhBullet>();
        }
#else
        int key = goPrefab.GetInstanceID();

        if (m_pooledKeyList.Contains(key) == false && m_pooledBulletDic.ContainsKey(key) == false)
        {
            m_pooledKeyList.Add(key);
            m_pooledBulletDic.Add(key, new List<UbhBullet>(1024));
        }

        List<UbhBullet> bulletList = m_pooledBulletDic[key];

        if (forceInstantiate == false)
        {
            for (int i = bulletList.Count - 1; i >= 0; i--)
            {
                if (bulletList[i] == null || bulletList[i].gameObject == null)
                {
                    bulletList.RemoveAt(i);
                    continue;
                }

                if (bulletList[i].gameObject.activeSelf == false)
                {
                    // Found free GameObject in object pool.
                    bullet = bulletList[i];
                    bullet.transform.localPosition = position;
                    bullet.transform.localRotation = rotation;
                    bullet.gameObject.SetActive(true);
                    break;
                }
            }
        }

        if (bullet == null)
        {
            // Instantiate because there is no free GameObject in object pool.
            GameObject go = (GameObject)Instantiate(goPrefab, transform);
            bullet = go.GetComponent<UbhBullet>();
            if (bullet == null)
            {
                bullet = go.AddComponent<UbhBullet>();
            }
            bullet.transform.localPosition = position;
            bullet.transform.localRotation = rotation;
            bulletList.Add(bullet);
        }
#endif

        UbhBulletManager.instance.AddBullet(bullet);

        return bullet;
    }

    /// <summary>
    /// Releases bullet (back to pool or destroy).
    /// </summary>
    public void ReleaseBullet(UbhBullet bullet, bool destroy = false)
    {
        if (bullet == null || bullet.gameObject == null)
        {
            return;
        }

        UbhBulletManager.instance.RemoveBullet(bullet);

#if USING_CORE_GAME_KIT
        // +++++ Replace PoolingSystem with DarkTonic's CoreGameKit. +++++
        InitializePoolBoss();
        PoolBoss.Despawn(bullet.transform);
#else
        if (destroy)
        {
            Destroy(bullet.gameObject);
            Destroy(bullet);
            bullet = null;
            return;
        }
        bullet.gameObject.SetActive(false);
#endif
    }

    /// <summary>
    /// Get active bullets list.
    /// </summary>
    public List<UbhBullet> GetActiveBulletsList(GameObject goPrefab)
    {
        int poolKey = goPrefab.GetInstanceID();
        List<UbhBullet> findList = null;

#if USING_CORE_GAME_KIT
        // +++++ Replace PoolingSystem with DarkTonic's CoreGameKit. +++++
        InitializePoolBoss();
        if (PoolBoss.Instance.poolItems != null && PoolBoss.Instance.poolItems.Count > 0)
        {
            for (int i = 0; i < PoolBoss.Instance.poolItems.Count; i++)
            {
                if (PoolBoss.Instance.poolItems[i].prefabTransform.gameObject.GetInstanceID() == poolKey)
                {
                    var poolIteminfo = PoolBoss.PoolItemInfoByName(PoolBoss.Instance.poolItems[i].prefabTransform.gameObject.name);
                    if (poolIteminfo != null && poolIteminfo.SpawnedClones != null && poolIteminfo.SpawnedClones.Count > 0)
                    {
                        for (int k = 0; k < poolIteminfo.SpawnedClones.Count; k++)
                        {
                            if (poolIteminfo.SpawnedClones[k] == null)
                            {
                                continue;
                            }
                            var bullet = poolIteminfo.SpawnedClones[k].GetComponent<UbhBullet>();
                            if (bullet == null)
                            {
                                continue;
                            }
                            if (findList == null)
                            {
                                findList = new List<UbhBullet>(1024);
                            }
                            findList.Add(bullet);
                        }
                    }
                }
            }
        }
#else
        if (m_pooledBulletDic.ContainsKey(poolKey))
        {
            List<UbhBullet> bulletList = m_pooledBulletDic[poolKey];
            for (int i = 0; i < bulletList.Count; i++)
            {
                UbhBullet bullet = bulletList[i];
                if (bullet != null && bullet.gameObject != null && bullet.gameObject.activeSelf)
                {
                    if (findList == null)
                    {
                        findList = new List<UbhBullet>(1024);
                    }
                    findList.Add(bulletList[i]);
                }
            }
        }
#endif
        return findList;
    }

    /// <summary>
    /// Get active bullets count.
    /// </summary>
    public int GetActiveBulletsCount()
    {
#if USING_CORE_GAME_KIT
        // +++++ Replace PoolingSystem with DarkTonic's CoreGameKit. +++++
        InitializePoolBoss();
        var bullets = GetComponentsInChildren<UbhBullet>();
        return bullets == null ? 0 : bullets.Length;
#else
        int cnt = 0;
        for (int i = 0; i < m_pooledKeyList.Count; i++)
        {
            int key = m_pooledKeyList[i];
            List<UbhBullet> bulletList = m_pooledBulletDic[key];
            for (int k = 0; k < bulletList.Count; k++)
            {
                UbhBullet bullet = bulletList[k];
                if (bullet != null && bullet.gameObject != null && bullet.gameObject.activeSelf)
                {
                    cnt++;
                }
            }
        }
        return cnt;
#endif
    }
}