using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Ubh random shot.
/// </summary>
[AddComponentMenu("UniBulletHell/Shot Pattern/Random Shot")]
public class UbhRandomShot : UbhBaseShot
{
    [Header("===== RandomShot Settings =====")]
    // "Center angle of random range."
    [Range(0f, 360f), FormerlySerializedAs("_RandomCenterAngle")]
    public float m_randomCenterAngle = 180f;
    // "Set a angle size of random range. (0 to 360)"
    [Range(0f, 360f), FormerlySerializedAs("_RandomRangeSize")]
    public float m_randomRangeSize = 360f;
    // "Set a minimum bullet speed of shot."
    // "BulletSpeed is ignored."
    [FormerlySerializedAs("_RandomSpeedMin")]
    public float m_randomSpeedMin = 1f;
    // "Set a maximum bullet speed of shot."
    // "BulletSpeed is ignored."
    [FormerlySerializedAs("_RandomSpeedMax")]
    public float m_randomSpeedMax = 3f;
    // "Set a minimum delay time between bullet and next bullet. (sec)"
    [FormerlySerializedAs("_RandomDelayMin")]
    public float m_randomDelayMin = 0.01f;
    // "Set a maximum delay time between bullet and next bullet. (sec)"
    [FormerlySerializedAs("_RandomDelayMax")]
    public float m_randomDelayMax = 0.1f;
    // "Evenly distribute of all bullet angle."
    [FormerlySerializedAs("_EvenlyDistribute")]
    public bool m_evenlyDistribute = true;

    public override void Shot()
    {
        StartCoroutine(ShotCoroutine());
    }

    private IEnumerator ShotCoroutine()
    {
        if (m_bulletNum <= 0 || m_randomSpeedMin <= 0f || m_randomSpeedMax <= 0)
        {
            Debug.LogWarning("Cannot shot because BulletNum or RandomSpeedMin or RandomSpeedMax is not set.");
            yield break;
        }
        if (m_shooting)
        {
            yield break;
        }
        m_shooting = true;

        var numList = new List<int>(m_bulletNum);

        for (int i = 0; i < m_bulletNum; i++)
        {
            numList.Add(i);
        }

        while (0 < numList.Count)
        {
            int index = Random.Range(0, numList.Count);
            var bullet = GetBullet(transform.position, transform.rotation);
            if (bullet == null)
            {
                break;
            }

            float bulletSpeed = Random.Range(m_randomSpeedMin, m_randomSpeedMax);

            float minAngle = m_randomCenterAngle - (m_randomRangeSize / 2f);
            float maxAngle = m_randomCenterAngle + (m_randomRangeSize / 2f);
            float angle = 0f;

            if (m_evenlyDistribute)
            {
                float oneDirectionNum = Mathf.Floor((float)m_bulletNum / 4f);
                float quarterIndex = Mathf.Floor((float)numList[index] / oneDirectionNum);
                float quarterAngle = Mathf.Abs(maxAngle - minAngle) / 4f;
                angle = Random.Range(minAngle + (quarterAngle * quarterIndex), minAngle + (quarterAngle * (quarterIndex + 1f)));
            }
            else
            {
                angle = Random.Range(minAngle, maxAngle);
            }

            ShotBullet(bullet, bulletSpeed, angle);

            AutoReleaseBullet(bullet);

            numList.RemoveAt(index);

            if (0 < numList.Count && 0f <= m_randomDelayMin && 0f < m_randomDelayMax)
            {
                FiredShot();
                float waitTime = Random.Range(m_randomDelayMin, m_randomDelayMax);
                yield return UbhUtil.WaitForSeconds(waitTime);
            }
        }

        FiredShot();

        FinishedShot();

        yield break;
    }
}