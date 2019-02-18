using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Ubh spiral multi nway shot.
/// </summary>
[AddComponentMenu("UniBulletHell/Shot Pattern/Spiral Multi nWay Shot")]
public class UbhSpiralMultiNwayShot : UbhBaseShot
{
    [Header("===== SpiralMultiNwayShot Settings =====")]
    // "Set a number of shot spiral way."
    [FormerlySerializedAs("_SpiralWayNum")]
    public int m_spiralWayNum = 4;
    // "Set a number of shot way."
    [FormerlySerializedAs("_WayNum")]
    public int m_wayNum = 5;
    // "Set a starting angle of shot. (0 to 360)"
    [Range(0f, 360f), FormerlySerializedAs("_StartAngle")]
    public float m_startAngle = 180f;
    // "Set a shift angle of spiral. (-360 to 360)"
    [Range(-360f, 360f), FormerlySerializedAs("_ShiftAngle")]
    public float m_shiftAngle = 5f;
    // "Set a angle between bullet and next bullet. (0 to 360)"
    [Range(0f, 360f), FormerlySerializedAs("_BetweenAngle")]
    public float m_betweenAngle = 5f;
    // "Set a delay time between shot and next line shot. (sec)"
    [FormerlySerializedAs("_NextLineDelay")]
    public float m_nextLineDelay = 0.1f;

    public override void Shot()
    {
        StartCoroutine(ShotCoroutine());
    }

    private IEnumerator ShotCoroutine()
    {
        if (m_bulletNum <= 0 || m_bulletSpeed <= 0f || m_wayNum <= 0 || m_spiralWayNum <= 0)
        {
            Debug.LogWarning("Cannot shot because BulletNum or BulletSpeed or WayNum or SpiralWayNum is not set.");
            yield break;
        }
        if (m_shooting)
        {
            yield break;
        }
        m_shooting = true;

        float spiralWayShiftAngle = 360f / m_spiralWayNum;

        int wayIndex = 0;
        int spiralWayIndex = 0;

        for (int i = 0; i < m_bulletNum; i++)
        {
            if (m_wayNum <= wayIndex)
            {
                wayIndex = 0;

                spiralWayIndex++;
                if (m_spiralWayNum <= spiralWayIndex)
                {
                    spiralWayIndex = 0;

                    if (0f < m_nextLineDelay)
                    {
                        FiredShot();
                        yield return UbhUtil.WaitForSeconds(m_nextLineDelay);
                    }
                }
            }

            var bullet = GetBullet(transform.position, transform.rotation);
            if (bullet == null)
            {
                break;
            }

            float centerAngle = m_startAngle + (spiralWayShiftAngle * spiralWayIndex) + (m_shiftAngle * Mathf.Floor(i / m_wayNum));

            float baseAngle = m_wayNum % 2 == 0 ? centerAngle - (m_betweenAngle / 2f) : centerAngle;

            float angle = UbhUtil.GetShiftedAngle(wayIndex, baseAngle, m_betweenAngle);

            ShotBullet(bullet, m_bulletSpeed, angle);

            AutoReleaseBullet(bullet);

            wayIndex++;
        }

        FiredShot();

        FinishedShot();

        yield break;
    }
}