using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Ubh hole circle shot.
/// </summary>
[AddComponentMenu("UniBulletHell/Shot Pattern/Hole Circle Shot")]
public class UbhHoleCircleShot : UbhBaseShot
{
    [Header("===== HoleCircleShot Settings =====")]
    // "Set a center angle of hole. (0 to 360)"
    [Range(0f, 360f), FormerlySerializedAs("_HoleCenterAngle")]
    public float m_holeCenterAngle = 180f;
    // "Set a size of hole. (0 to 360)"
    [Range(0f, 360f), FormerlySerializedAs("_HoleSize")]
    public float m_holeSize = 20f;

    public override void Shot()
    {
        if (m_bulletNum <= 0 || m_bulletSpeed <= 0f)
        {
            Debug.LogWarning("Cannot shot because BulletNum or BulletSpeed is not set.");
            return;
        }

        m_holeCenterAngle = UbhUtil.Get360Angle(m_holeCenterAngle);
        float startAngle = m_holeCenterAngle - (m_holeSize / 2f);
        float endAngle = m_holeCenterAngle + (m_holeSize / 2f);

        float shiftAngle = 360f / (float)m_bulletNum;

        for (int i = 0; i < m_bulletNum; i++)
        {
            float angle = shiftAngle * i;
            if (startAngle <= angle && angle <= endAngle)
            {
                continue;
            }

            var bullet = GetBullet(transform.position, transform.rotation);
            if (bullet == null)
            {
                break;
            }

            ShotBullet(bullet, m_bulletSpeed, angle);

            AutoReleaseBullet(bullet);
        }

        FiredShot();

        FinishedShot();
    }
}