using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Ubh paint shot.
/// </summary>
[AddComponentMenu("UniBulletHell/Shot Pattern/Paint Shot")]
public class UbhPaintShot : UbhBaseShot
{
    private static readonly string[] SPLIT_VAL = { "\n", "\r", "\r\n" };

    [Header("===== PaintShot Settings =====")]
    // "Set a paint data text file. (ex.[UniBulletHell] > [Example] > [PaintShotData] in Project view)"
    // "BulletNum is ignored."
    [FormerlySerializedAs("_PaintDataText")]
    public TextAsset m_paintDataText;
    // "Set a center angle of shot. (0 to 360) (center of first line)"
    [Range(0f, 360f), FormerlySerializedAs("_PaintCenterAngle")]
    public float m_paintCenterAngle = 180f;
    // "Set a angle between bullet and next bullet. (0 to 360)"
    [Range(0f, 360f), FormerlySerializedAs("_BetweenAngle")]
    public float m_betweenAngle = 3f;
    // "Set a delay time between shot and next line shot. (sec)"
    [FormerlySerializedAs("_NextLineDelay")]
    public float m_nextLineDelay = 0.1f;

    public override void Shot()
    {
        StartCoroutine(ShotCoroutine());
    }

    private IEnumerator ShotCoroutine()
    {
        if (m_bulletSpeed <= 0f || m_paintDataText == null)
        {
            Debug.LogWarning("Cannot shot because BulletSpeed or PaintDataText is not set.");
            yield break;
        }
        if (m_shooting)
        {
            yield break;
        }
        m_shooting = true;

        List<List<int>> paintData = LoadPaintData();

        if (paintData != null)
        {
            float paintStartAngle = m_paintCenterAngle;
            if (0 < paintData.Count)
            {
                paintStartAngle -= paintData[0].Count % 2 == 0 ?
                    (m_betweenAngle * paintData[0].Count / 2f) + (m_betweenAngle / 2f) :
                     m_betweenAngle * Mathf.Floor(paintData[0].Count / 2f);
            }

            for (int lineCnt = 0; lineCnt < paintData.Count; lineCnt++)
            {
                var line = paintData[lineCnt];
                if (0 < lineCnt && 0 < m_nextLineDelay)
                {
                    FiredShot();
                    yield return UbhUtil.WaitForSeconds(m_nextLineDelay);
                }
                for (int i = 0; i < line.Count; i++)
                {
                    if (line[i] == 1)
                    {
                        var bullet = GetBullet(transform.position, transform.rotation);
                        if (bullet == null)
                        {
                            break;
                        }

                        float angle = paintStartAngle + (m_betweenAngle * i);

                        ShotBullet(bullet, m_bulletSpeed, angle);

                        AutoReleaseBullet(bullet);
                    }
                }
            }
        }

        FiredShot();

        FinishedShot();

        yield break;
    }

    private List<List<int>> LoadPaintData()
    {
        if (string.IsNullOrEmpty(m_paintDataText.text))
        {
            Debug.LogWarning("Cannot load paint data because PaintDataText file is empty.");
            return null;
        }

        string[] lines = m_paintDataText.text.Split(SPLIT_VAL, System.StringSplitOptions.RemoveEmptyEntries);

        var paintData = new List<List<int>>(lines.Length);

        for (int i = 0; i < lines.Length; i++)
        {
            // lines beginning with "#" are ignored as comments.
            if (lines[i].StartsWith("#"))
            {
                continue;
            }
            // add line
            paintData.Add(new List<int>(lines[i].Length));

            for (int j = 0; j < lines[i].Length; j++)
            {
                // bullet is fired into position of "*".
                paintData[paintData.Count - 1].Add(lines[i][j] == '*' ? 1 : 0);
            }
        }

        // reverse because fire from bottom left.
        paintData.Reverse();

        return paintData;
    }
}