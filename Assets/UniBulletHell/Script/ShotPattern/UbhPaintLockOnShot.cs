using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Ubh paint lock on shot.
/// </summary>
[AddComponentMenu("UniBulletHell/Shot Pattern/Paint Shot (Lock On)")]
public class UbhPaintLockOnShot : UbhPaintShot
{
    [Header("===== PaintLockOnShot Settings =====")]
    // "Set a target with tag name."
    [FormerlySerializedAs("_SetTargetFromTag")]
    public bool m_setTargetFromTag = true;
    // "Set a unique tag name of target at using SetTargetFromTag."
    [FormerlySerializedAs("_TargetTagName")]
    public string m_targetTagName = "Player";
    // "Transform of lock on target."
    // "It is not necessary if you want to specify target in tag."
    // "Overwrite PaintCenterAngle in direction of target to Transform.position."
    [FormerlySerializedAs("_TargetTransform")]
    public Transform m_targetTransform;

    public override void Shot()
    {
        if (m_shooting)
        {
            return;
        }
        if (m_targetTransform == null && m_setTargetFromTag)
        {
            m_targetTransform = UbhUtil.GetTransformFromTagName(m_targetTagName);
        }
        if (m_targetTransform == null)
        {
            Debug.LogWarning("Cannot shot because TargetTransform is not set.");
            return;
        }

        m_paintCenterAngle = UbhUtil.GetAngleFromTwoPosition(transform, m_targetTransform, shotCtrl.m_axisMove);

        base.Shot();
    }
}