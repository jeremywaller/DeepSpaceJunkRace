using UnityEngine;
using UnityEngine.Serialization;

public class UbhDestroyArea : UbhMonoBehaviour
{
    [SerializeField, FormerlySerializedAs("_UseCenterCollider")]
    private bool m_useCenterCollider;
    [SerializeField, FormerlySerializedAs("_ColCenter")]
    private BoxCollider2D m_colCenter;
    [SerializeField, FormerlySerializedAs("_ColTop")]
    private BoxCollider2D m_colTop;
    [SerializeField, FormerlySerializedAs("_ColBottom")]
    private BoxCollider2D m_colBottom;
    [SerializeField, FormerlySerializedAs("_ColRight")]
    private BoxCollider2D m_colRight;
    [SerializeField, FormerlySerializedAs("_ColLeft")]
    private BoxCollider2D m_colLeft;

    private void Start()
    {
        if (m_colCenter == null || m_colTop == null || m_colBottom == null || m_colRight == null || m_colLeft == null)
        {
            return;
        }

        UbhManager manager = FindObjectOfType<UbhManager>();
        if (manager != null && manager.m_scaleToFit)
        {
            Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1f, 1f));
            Vector2 size = max * 2f;
            size.x += 0.5f;
            size.y += 0.5f;
            Vector2 center = Vector2.zero;

            m_colCenter.size = size;

            m_colTop.size = size;
            center.x = m_colTop.offset.x;
            center.y = size.y;
            m_colTop.offset = center;

            m_colBottom.size = size;
            center.x = m_colBottom.offset.x;
            center.y = -size.y;
            m_colBottom.offset = center;

            Vector2 horizontalSize = Vector2.zero;
            horizontalSize.x = size.y;
            horizontalSize.y = size.x;

            m_colRight.size = horizontalSize;
            center.x = (size.x / 2f) + (horizontalSize.x / 2f);
            center.y = m_colRight.offset.y;
            m_colRight.offset = center;

            m_colLeft.size = horizontalSize;
            center.x = -(size.x / 2f) - (horizontalSize.x / 2f);
            center.y = m_colLeft.offset.y;
            m_colLeft.offset = center;
        }

        m_colCenter.enabled = m_useCenterCollider;
        m_colTop.enabled = !m_useCenterCollider;
        m_colBottom.enabled = !m_useCenterCollider;
        m_colRight.enabled = !m_useCenterCollider;
        m_colLeft.enabled = !m_useCenterCollider;
    }

    private void OnTriggerEnter2D(Collider2D c)
    {
        if (m_useCenterCollider)
        {
            return;
        }
        HitCheck(c.transform);
    }

    private void OnTriggerExit2D(Collider2D c)
    {
        if (m_useCenterCollider == false)
        {
            return;
        }
        HitCheck(c.transform);
    }

    private void OnTriggerEnter(Collider c)
    {
        if (m_useCenterCollider)
        {
            return;
        }
        HitCheck(c.transform);
    }

    private void OnTriggerExit(Collider c)
    {
        if (m_useCenterCollider == false)
        {
            return;
        }
        HitCheck(c.transform);
    }

    private void HitCheck(Transform colTrans)
    {
        // *It is compared with name in order to separate as Asset from project settings.
        //  However, it is recommended to use Layer or Tag.
        string goName = colTrans.name;
        if (goName.Contains(UbhPlayer.NAME_ENEMY_BULLET) ||
            goName.Contains(UbhEnemy.NAME_PLAYER_BULLET))
        {
            var bullet = colTrans.parent.GetComponentInParent<UbhBullet>();
            if (bullet != null)
            {
                UbhObjectPool.instance.ReleaseBullet(bullet);
            }
        }
        else if (goName.Contains(UbhEnemy.NAME_PLAYER) == false)
        {
            Destroy(colTrans.gameObject);
        }
    }
}