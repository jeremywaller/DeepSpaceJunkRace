using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

public class UbhEmitter : UbhMonoBehaviour
{
    [SerializeField, FormerlySerializedAs("_Waves")]
    private GameObject[] m_waves;

    private int m_currentWave;
    private UbhManager m_manager;

    private IEnumerator Start()
    {
        if (m_waves.Length == 0)
        {
            yield break;
        }

        m_manager = FindObjectOfType<UbhManager>();

        while (true)
        {
            while (m_manager.IsPlaying() == false)
            {
                yield return 0;
            }

            GameObject wave = (GameObject)Instantiate(m_waves[m_currentWave], transform);
            Transform waveTrans = wave.transform;
            waveTrans.position = transform.position;

            while (0 < waveTrans.childCount)
            {
                yield return 0;
            }

            Destroy(wave);

            m_currentWave = (int)Mathf.Repeat(m_currentWave + 1f, m_waves.Length);
        }
    }
}