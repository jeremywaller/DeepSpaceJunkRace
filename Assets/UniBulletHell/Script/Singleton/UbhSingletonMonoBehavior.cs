using UnityEngine;

/// <summary>
/// Ubh singleton mono behavior.
/// </summary>
public class UbhSingletonMonoBehavior<T> : UbhMonoBehaviour where T : UbhMonoBehaviour
{
    private static T m_instance;

    /// <summary>
    /// Get singleton instance.
    /// </summary>
    public static T instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<T>();

                if (m_instance == null)
                {
                    Debug.Log("Created " + typeof(T).Name);
                    m_instance = new GameObject(typeof(T).Name).AddComponent<T>();
                }
            }
            return m_instance;
        }
    }

    /// <summary>
    /// Call from override Awake method in inheriting classes.
    /// Example : protected override void Awake () { base.Awake (); }
    /// </summary>
    protected virtual void Awake()
    {
        if (this != instance)
        {
            GameObject obj = gameObject;
            Destroy(this);
            Destroy(obj);
            return;
        }
    }
}
