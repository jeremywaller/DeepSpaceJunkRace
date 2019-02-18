using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Ubh homing shot.
/// </summary>
[AddComponentMenu("UniBulletHell/Shot Pattern/Homing Shot")]
public class UbhHomingShot : UbhBaseShot
{
    [Header("===== HomingShot Settings =====")]
    // "Set a delay time between bullet and next bullet. (sec)"
    [FormerlySerializedAs("_BetweenDelay")]
    public float m_betweenDelay = 0.1f;
    // "Set a speed of homing angle."
    [FormerlySerializedAs("_HomingAngleSpeed")]
    public float m_homingAngleSpeed = 20f;
    // "Set a target with tag name."
    [FormerlySerializedAs("_SetTargetFromTag")]
    public bool m_setTargetFromTag = true;
    // "Set a unique tag name of target at using SetTargetFromTag."
    [FormerlySerializedAs("_TargetTagName")]
    public string m_targetTagName = "Player";
    // "Transform of lock on target."
    // "It is not necessary if you want to specify target in tag."
    [FormerlySerializedAs("_TargetTransform")]
    public Transform m_targetTransform;

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

            if (m_targetTransform == null && m_setTargetFromTag)
            {
                m_targetTransform = UbhUtil.GetTransformFromTagName(m_targetTagName);
            }

            float angle = UbhUtil.GetAngleFromTwoPosition(transform, m_targetTransform, shotCtrl.m_axisMove);

            ShotBullet(bullet, m_bulletSpeed, angle, true, m_targetTransform, m_homingAngleSpeed);

            AutoReleaseBullet(bullet);
        }

        FiredShot();

        FinishedShot();

        yield break;
    }
}