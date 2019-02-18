using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Ubh random spiral shot.
/// </summary>
[AddComponentMenu("UniBulletHell/Shot Pattern/Random Spiral Shot")]
public class UbhRandomSpiralShot : UbhBaseShot
{
    [Header("===== RandomSpiralShot Settings =====")]
    // "Set a starting angle of shot. (0 to 360)"
    [Range(0f, 360f), FormerlySerializedAs("_StartAngle")]
    public float m_startAngle = 180f;
    // "Set a shift angle of spiral. (-360 to 360)"
    [Range(-360f, 360f), FormerlySerializedAs("_ShiftAngle")]
    public float m_shiftAngle = 5f;
    // "Set a angle size of random range. (0 to 360)"
    [Range(0f, 360f), FormerlySerializedAs("_RandomRangeSize")]
    public float m_randomRangeSize = 30f;
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

        for (int i = 0; i < m_bulletNum; i++)
        {
            if (0 < i && 0f <= m_randomDelayMin && 0f < m_randomDelayMax)
            {
                FiredShot();
                float waitTime = Random.Range(m_randomDelayMin, m_randomDelayMax);
                yield return UbhUtil.WaitForSeconds(waitTime);
            }

            var bullet = GetBullet(transform.position, transform.rotation);
            if (bullet == null)
            {
                break;
            }

            float bulletSpeed = Random.Range(m_randomSpeedMin, m_randomSpeedMax);

            float centerAngle = m_startAngle + (m_shiftAngle * i);
            float minAngle = centerAngle - (m_randomRangeSize / 2f);
            float maxAngle = centerAngle + (m_randomRangeSize / 2f);
            float angle = Random.Range(minAngle, maxAngle);

            ShotBullet(bullet, bulletSpeed, angle);

            AutoReleaseBullet(bullet);
        }

        FiredShot();

        FinishedShot();

        yield break;
    }
}