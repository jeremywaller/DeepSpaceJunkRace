using UnityEngine;

/// <summary>
/// Ubh circle shot.
/// </summary>
[AddComponentMenu("UniBulletHell/Shot Pattern/Circle Shot")]
public class UbhCircleShot : UbhBaseShot
{
    public override void Shot()
    {
        if (m_bulletNum <= 0 || m_bulletSpeed <= 0f)
        {
            Debug.LogWarning("Cannot shot because BulletNum or BulletSpeed is not set.");
            return;
        }

        float shiftAngle = 360f / (float)m_bulletNum;

        for (int i = 0; i < m_bulletNum; i++)
        {
            var bullet = GetBullet(transform.position, transform.rotation);
            if (bullet == null)
            {
                break;
            }

            float angle = shiftAngle * i;

            ShotBullet(bullet, m_bulletSpeed, angle);

            AutoReleaseBullet(bullet);
        }

        FiredShot();

        FinishedShot();
    }
}