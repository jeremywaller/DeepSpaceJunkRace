using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Ubh over take nway shot.
/// </summary>
[AddComponentMenu("UniBulletHell/Shot Pattern/Over Take nWay Shot")]
public class UbhOverTakeNwayShot : UbhBaseShot
{
    [Header("===== OverTakeNwayShot Settings =====")]
    // "Set a number of shot way."
    [FormerlySerializedAs("_WayNum")]
    public int m_wayNum = 8;
    // "Set a center angle of shot. (0 to 360)"
    [Range(0f, 360f), FormerlySerializedAs("_CenterAngle")]
    public float m_centerAngle = 180f;
    // "Set a angle between bullet and next bullet. (0 to 360)"
    [Range(0f, 360f), FormerlySerializedAs("_BetweenAngle")]
    public float m_betweenAngle = 10f;
    // "Set a difference speed between shot and next line shot."
    [FormerlySerializedAs("_DiffSpeed")]
    public float m_diffSpeed = 0.5f;
    // "Set a shift angle between shot and next line shot. (-360 to 360)"
    [Range(-360f, 360f), FormerlySerializedAs("_ShiftAngle")]
    public float m_shiftAngle = 0f;
    // "Set a delay time between shot and next line shot. (sec)"
    [FormerlySerializedAs("_NextLineDelay")]
    public float m_nextLineDelay = 0.1f;

    public override void Shot()
    {
        StartCoroutine(ShotCoroutine());
    }

    private IEnumerator ShotCoroutine()
    {
        if (m_bulletNum <= 0 || m_bulletSpeed <= 0f || m_wayNum <= 0)
        {
            Debug.LogWarning("Cannot shot because BulletNum or BulletSpeed or WayNum is not set.");
            yield break;
        }
        if (m_shooting)
        {
            yield break;
        }
        m_shooting = true;

        int wayIndex = 0;

        float bulletSpeed = m_bulletSpeed;
        float shiftAngle = 0f;

        for (int i = 0; i < m_bulletNum; i++)
        {
            if (m_wayNum <= wayIndex)
            {
                wayIndex = 0;

                if (0f < m_nextLineDelay)
                {
                    FiredShot();
                    yield return UbhUtil.WaitForSeconds(m_nextLineDelay);
                }

                bulletSpeed += m_diffSpeed;
                shiftAngle += m_shiftAngle;
            }

            var bullet = GetBullet(transform.position, transform.rotation);
            if (bullet == null)
            {
                break;
            }

            float baseAngle = m_wayNum % 2 == 0 ? m_centerAngle - (m_betweenAngle / 2f) : m_centerAngle;

            float angle = UbhUtil.GetShiftedAngle(wayIndex, baseAngle, m_betweenAngle) + shiftAngle;

            ShotBullet(bullet, bulletSpeed, angle);

            AutoReleaseBullet(bullet);

            wayIndex++;
        }

        FiredShot();

        FinishedShot();

        yield break;
    }
}