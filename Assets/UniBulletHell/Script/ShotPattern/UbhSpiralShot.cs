using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Ubh spiral shot.
/// </summary>
[AddComponentMenu("UniBulletHell/Shot Pattern/Spiral Shot")]
public class UbhSpiralShot : UbhBaseShot
{
    [Header("===== SpiralShot Settings =====")]
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
        if (m_bulletNum <= 0 || m_bulletSpeed <= 0f)
        {
            Debug.LogWarning("Cannot shot because BulletNum or BulletSpeed is not set.");
            yield break;
        }
        if (m_shooting)
        {
            yield break;
        }
        m_shooting = true;

        for (int i = 0; i < m_bulletNum; i++)
        {
            if (0 < i && 0f < m_betweenDelay)
            {
                FiredShot();
                yield return UbhUtil.WaitForSeconds(m_betweenDelay);
            }

            var bullet = GetBullet(transform.position, transform.rotation);
            if (bullet == null)
            {
                break;
            }

            float angle = m_startAngle + (m_shiftAngle * i);

            ShotBullet(bullet, m_bulletSpeed, angle);

            AutoReleaseBullet(bullet);
        }

        FiredShot();

        FinishedShot();

        yield break;
    }
}