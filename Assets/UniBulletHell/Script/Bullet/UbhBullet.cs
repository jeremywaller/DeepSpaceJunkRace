using UnityEngine;

/// <summary>
/// Ubh bullet.
/// </summary>
public class UbhBullet : UbhMonoBehaviour
{
    private float m_speed;
    private float m_angle;
    private float m_accelSpeed;
    private float m_accelTurn;
    private bool m_homing;
    private Transform m_homingTarget;
    private float m_homingAngleSpeed;
    private bool m_wave;
    private float m_waveSpeed;
    private float m_waveRangeSize;
    private bool m_pauseAndResume;
    private float m_pauseTime;
    private float m_resumeTime;
    private UbhUtil.AXIS m_axisMove;

    private float m_selfFrameCnt;
    private float m_selfTimeCount;

    private UbhTentacleBullet m_tentacleBullet;

    public bool shooting
    {
        get;
        private set;
    }

    private void Awake()
    {
        m_tentacleBullet = GetComponent<UbhTentacleBullet>();
    }

    private void OnDisable()
    {
        transform.ResetPosition();
        transform.ResetRotation();
        shooting = false;
    }

    /// <summary>
    /// Bullet Shot
    /// </summary>
    public void Shot(float speed, float angle, float accelSpeed, float accelTurn,
                      bool homing, Transform homingTarget, float homingAngleSpeed,
                      bool wave, float waveSpeed, float waveRangeSize,
                      bool pauseAndResume, float pauseTime, float resumeTime, UbhUtil.AXIS axisMove)
    {
        if (shooting)
        {
            return;
        }
        shooting = true;

        m_speed = speed;
        m_angle = angle;
        m_accelSpeed = accelSpeed;
        m_accelTurn = accelTurn;
        m_homing = homing;
        m_homingTarget = homingTarget;
        m_homingAngleSpeed = homingAngleSpeed;
        m_wave = wave;
        m_waveSpeed = waveSpeed;
        m_waveRangeSize = waveRangeSize;
        m_pauseAndResume = pauseAndResume;
        m_pauseTime = pauseTime;
        m_resumeTime = resumeTime;
        m_axisMove = axisMove;

        if (axisMove == UbhUtil.AXIS.X_AND_Z)
        {
            // X and Z axis
            transform.SetEulerAnglesY(-angle);
        }
        else
        {
            // X and Y axis
            transform.SetEulerAnglesZ(angle);
        }

        m_selfFrameCnt = 0f;
        m_selfTimeCount = 0f;
    }

    /// <summary>
    /// Update Move
    /// </summary>
    public void UpdateMove()
    {
        if (shooting == false)
        {
            return;
        }

        m_selfTimeCount += UbhTimer.instance.deltaTime;

        // pause and resume.
        if (m_pauseAndResume && m_pauseTime >= 0f && m_resumeTime > m_pauseTime)
        {
            if (m_pauseTime <= m_selfTimeCount && m_selfTimeCount < m_resumeTime)
            {
                return;
            }
        }

        if (m_homing)
        {
            // homing target.
            if (m_homingTarget != null && 0f < m_homingAngleSpeed)
            {
                float rotAngle = UbhUtil.GetAngleFromTwoPosition(transform, m_homingTarget, m_axisMove);
                float myAngle = 0f;
                if (m_axisMove == UbhUtil.AXIS.X_AND_Z)
                {
                    // X and Z axis
                    myAngle = -transform.eulerAngles.y;
                }
                else
                {
                    // X and Y axis
                    myAngle = transform.eulerAngles.z;
                }

                float toAngle = Mathf.MoveTowardsAngle(myAngle, rotAngle, UbhTimer.instance.deltaTime * m_homingAngleSpeed);

                if (m_axisMove == UbhUtil.AXIS.X_AND_Z)
                {
                    // X and Z axis
                    transform.SetEulerAnglesY(-toAngle);
                }
                else
                {
                    // X and Y axis
                    transform.SetEulerAnglesZ(toAngle);
                }
            }

        }
        else if (m_wave)
        {
            // acceleration turning.
            m_angle += (m_accelTurn * UbhTimer.instance.deltaTime);
            // wave.
            if (0f < m_waveSpeed && 0f < m_waveRangeSize)
            {
                float waveAngle = m_angle + (m_waveRangeSize / 2f * Mathf.Sin(m_selfFrameCnt * m_waveSpeed / 100f));
                if (m_axisMove == UbhUtil.AXIS.X_AND_Z)
                {
                    // X and Z axis
                    transform.SetEulerAnglesY(-waveAngle);
                }
                else
                {
                    // X and Y axis
                    transform.SetEulerAnglesZ(waveAngle);
                }
            }
            m_selfFrameCnt += UbhTimer.instance.deltaFrameCount;
        }
        else
        {
            // acceleration turning.
            float addAngle = m_accelTurn * UbhTimer.instance.deltaTime;
            if (m_axisMove == UbhUtil.AXIS.X_AND_Z)
            {
                // X and Z axis
                transform.AddEulerAnglesY(-addAngle);
            }
            else
            {
                // X and Y axis
                transform.AddEulerAnglesZ(addAngle);
            }
        }

        // acceleration speed.
        m_speed += (m_accelSpeed * UbhTimer.instance.deltaTime);

        // move.
        if (m_axisMove == UbhUtil.AXIS.X_AND_Z)
        {
            // X and Z axis
            transform.localPosition += transform.forward.normalized * m_speed * UbhTimer.instance.deltaTime;
        }
        else
        {
            // X and Y axis
            transform.localPosition += transform.up.normalized * m_speed * UbhTimer.instance.deltaTime;
        }

        if (m_tentacleBullet != null)
        {
            // Update tentacles
            m_tentacleBullet.UpdateRotate();
        }
    }
}