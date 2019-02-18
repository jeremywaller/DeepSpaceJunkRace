using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Ubh spiral multi shot.
/// </summary>
[AddComponentMenu("UniBulletHell/Shot Pattern/Spiral Multi Shot")]
public class UbhSpiralMultiShot : UbhBaseShot
{
    [Header("===== SpiralMultiShot Settings =====")]
    // "Set a number of shot spiral way."
    [FormerlySerializedAs("_SpiralWayNum")]
    public int m_spiralWayNum = 4;
    // "Set a starting angle of shot. (0 to 360)"
    [Range(0f, 360f), FormerlySerializedAs("_StartAngle")]
    public float m_startAngle = 180f;
    // "Set a shift angle of spiral. (-360 to 360)"
    [Range(-360f, 360f), FormerlySerializedAs("_ShiftAngle")]
    public float m_shiftAngle = 5f;
    // "Set a delay time between bullet and next bullet. (sec)"
    [FormerlySerializedAs("_BetweenDelay")]
    public float m_betweenDelay = 0.2f;

    public override void Shot()
    {
        StartCoroutine(ShotCoroutine());
    }

    private IEnumerator ShotCoroutine()
    {
        if (m_bulletNum <= 0 || m_bulletSpeed <= 0f || m_spiralWayNum <= 0)
        {
            Debug.LogWarning("Cannot shot because BulletNum or BulletSpeed or SpiralWayNum is not set.");
            yield break;
        }
        if (m_shooting)
        {
            yield break;
        }
        m_shooting = true;

        float spiralWayShiftAngle = 360f / m_spiralWayNum;

        int spiralWayIndex = 0;

        for (int i = 0; i < m_bulletNum; i++)
        {
            if (m_spiralWayNum <= spiralWayIndex)
            {
                spiralWayIndex = 0;
                if (0f < m_betweenDelay)
                {
                    FiredShot();
                    yield return UbhUtil.WaitForSeconds(m_betweenDelay);
                }
            }

            var bullet = GetBullet(transform.position, transform.rotation);
            if (bullet == null)
            {
                break;
            }

            float angle = m_startAngle + (spiralWayShiftAngle * spiralWayIndex) + (m_shiftAngle * Mathf.Floor(i / m_spiralWayNum));

            ShotBullet(bullet, m_bulletSpeed, angle);

            AutoReleaseBullet(bullet);

            spiralWayIndex++;
        }

        FiredShot();

        FinishedShot();

        yield break;
    }
}