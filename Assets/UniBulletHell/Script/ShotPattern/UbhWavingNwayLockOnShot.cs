using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Ubh waving nway lock on shot.
/// </summary>
[AddComponentMenu("UniBulletHell/Shot Pattern/Waving nWay Shot (Lock On)")]
public class UbhWavingNwayLockOnShot : UbhWavingNwayShot
{
    [Header("===== WavingNwayLockOnShot Settings =====")]
    // "Set a target with tag name."
    [FormerlySerializedAs("_SetTargetFromTag")]
    public bool m_setTargetFromTag = true;
    // "Set a unique tag name of target at using SetTargetFromTag."
    [FormerlySerializedAs("_TargetTagName")]
    public string m_targetTagName = "Player";
    // "Transform of lock on target."
    // "It is not necessary if you want to specify target in tag."
    // "Overwrite WaveCenterAngle in direction of target to Transform.position."
    [FormerlySerializedAs("_TargetTransform")]
    public Transform m_targetTransform;
    // "Always aim to target."
    [FormerlySerializedAs("_Aiming")]
    public bool m_aiming;

    public override void Shot()
    {
        if (m_shooting)
        {
            return;
        }

        AimTarget();

        if (m_targetTransform == null)
        {
            Debug.LogWarning("Cannot shot because TargetTransform is not set.");
            return;
        }

        base.Shot();

        if (m_aiming)
        {
            StartCoroutine(AimingCoroutine());
        }
    }

    private void AimTarget()
    {
        if (m_targetTransform == null && m_setTargetFromTag)
        {
            m_targetTransform = UbhUtil.GetTransformFromTagName(m_targetTagName);
        }
        if (m_targetTransform != null)
        {
            m_waveCenterAngle = UbhUtil.GetAngleFromTwoPosition(transform, m_targetTransform, shotCtrl.m_axisMove);
        }
    }

    private IEnumerator AimingCoroutine()
    {
        while (m_aiming)
        {
            if (m_shooting == false)
            {
                yield break;
            }

            AimTarget();

            yield return 0;
        }
    }
}