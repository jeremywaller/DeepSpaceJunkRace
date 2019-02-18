using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

/// <summary>
/// Ubh shot ctrl.
/// </summary>
[AddComponentMenu("UniBulletHell/Controller/Shot Controller")]
public class UbhShotCtrl : UbhMonoBehaviour
{
    [Serializable]
    public class ShotInfo
    {
        // "Set a shot pattern component (inherits UbhBaseShot)."
        [FormerlySerializedAs("_ShotObj")]
        public UbhBaseShot m_shotObj;
        // "Set a delay time to starting next shot pattern. (sec)"
        [FormerlySerializedAs("_AfterDelay")]
        public float m_afterDelay;
    }

    // "Axis on bullet move."
    [FormerlySerializedAs("_AxisMove")]
    public UbhUtil.AXIS m_axisMove = UbhUtil.AXIS.X_AND_Y;
    // "This flag starts a shot routine at same time as instantiate."
    [FormerlySerializedAs("_StartOnAwake")]
    public bool m_startOnAwake = true;
    // "Set a delay time at using Start On Awake. (sec)"
    [FormerlySerializedAs("_StartOnAwakeDelay")]
    public float m_startOnAwakeDelay = 1f;
    // "This flag repeats a shot routine."
    [FormerlySerializedAs("_Loop")]
    public bool m_loop = true;
    // "This flag makes a shot routine randomly."
    [FormerlySerializedAs("_AtRandom")]
    public bool m_atRandom = false;
    // "List of shot information. this size is necessary at least 1 or more."
    [FormerlySerializedAs("_ShotList")]
    public List<ShotInfo> m_shotList = new List<ShotInfo>();

    [Space(10)]

    // "Set a callback method after shot routine."
    [SerializeField]
    private UnityEvent m_shotRoutineFinishedCallbackEvents = new UnityEvent();

    private bool m_shooting;
    private List<ShotInfo> m_workShotInfoList = new List<ShotInfo>(32);

    /// <summary>
    /// is shooting flag.
    /// </summary>
    public bool shooting { get { return m_shooting; } }

    private IEnumerator Start()
    {
        if (m_startOnAwake)
        {
            if (0f < m_startOnAwakeDelay)
            {
                yield return UbhUtil.WaitForSeconds(m_startOnAwakeDelay);
            }
            StartShotRoutine();
        }
    }

    private void OnDisable()
    {
        m_shooting = false;
    }

    /// <summary>
    /// Start the shot routine.
    /// </summary>
    public void StartShotRoutine()
    {
        StartCoroutine(ShotCoroutine());
    }

    private IEnumerator ShotCoroutine()
    {
        if (m_shotList == null || m_shotList.Count <= 0)
        {
            Debug.LogWarning("Cannot shot because ShotList is not set.");
            yield break;
        }

        bool enableShot = false;
        for (int i = 0; i < m_shotList.Count; i++)
        {
            if (m_shotList[i].m_shotObj != null)
            {
                enableShot = true;
                break;
            }
        }
        if (enableShot == false)
        {
            Debug.LogWarning("Cannot shot because all ShotObj of ShotList is not set.");
            yield break;
        }

        bool enableDelay = false;
        for (int i = 0; i < m_shotList.Count; i++)
        {
            if (0f < m_shotList[i].m_afterDelay)
            {
                enableDelay = true;
                break;
            }
        }
        if (m_loop && enableDelay == false)
        {
            Debug.LogWarning("Cannot shot because loop is true and all AfterDelay of ShotList is zero.");
            yield break;
        }

        if (m_shooting)
        {
            Debug.LogWarning("Already shooting.");
            yield break;
        }
        m_shooting = true;

        m_workShotInfoList.AddRange(m_shotList);

        int nowIndex = 0;

        while (true)
        {
            if (m_atRandom)
            {
                nowIndex = UnityEngine.Random.Range(0, m_workShotInfoList.Count);
            }

            if (m_workShotInfoList[nowIndex].m_shotObj != null)
            {
                m_workShotInfoList[nowIndex].m_shotObj.SetShotCtrl(this);
                m_workShotInfoList[nowIndex].m_shotObj.Shot();
            }

            if (0f < m_workShotInfoList[nowIndex].m_afterDelay)
            {
                yield return UbhUtil.WaitForSeconds(m_workShotInfoList[nowIndex].m_afterDelay);
            }

            if (m_atRandom)
            {
                m_workShotInfoList.RemoveAt(nowIndex);

                if (m_workShotInfoList.Count <= 0)
                {
                    if (m_loop)
                    {
                        m_workShotInfoList.AddRange(m_shotList);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                if (m_loop == false && m_workShotInfoList.Count - 1 <= nowIndex)
                {
                    break;
                }

                nowIndex = (int)Mathf.Repeat(nowIndex + 1f, m_workShotInfoList.Count);
            }

            if (m_shooting == false)
            {
                break;
            }
        }

        m_workShotInfoList.Clear();

        m_shotRoutineFinishedCallbackEvents.Invoke();

        m_shooting = false;

        yield break;
    }

    /// <summary>
    /// Stop the shot routine.
    /// </summary>
    public void StopShotRoutine()
    {
        StopAllCoroutines();
        m_shooting = false;
    }
}