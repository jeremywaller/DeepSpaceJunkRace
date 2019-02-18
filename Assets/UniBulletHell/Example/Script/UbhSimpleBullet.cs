using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
public class UbhSimpleBullet : UbhBullet
{
    [FormerlySerializedAs("_Power")]
    public int m_power = 1;
}