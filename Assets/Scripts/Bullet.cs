using UnityEngine;
using System.Collections;
using Stone.Core;
using Stone.Pool;

public class Bullet : Pooled_BehaviourUnit 
{
	[SerializeField][Tooltip("移动速度")]
	private float m_moveVelocity=10;
	[SerializeField][Tooltip("移动时长")]
	private float m_moveTime=3;
	[System.NonSerialized][Tooltip("移动计数")]
	private float m_moveTimeTick;
	protected override void OnInitFirst()
	{
	}

	protected override void OnInitSecond()
	{
	}

	protected override void OnUpdate()
	{
		float deltaTime = Time.deltaTime;
		m_moveTimeTick += deltaTime;
		if (m_moveTimeTick >= m_moveTime)
		{
			m_moveTimeTick = 0;
			this.restore();
		}
		else
		{
			var pos = m_transform.localPosition;
			pos.z += m_moveVelocity * deltaTime;
			m_transform.localPosition = pos;
		}
	}
}