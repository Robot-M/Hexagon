using UnityEngine;
using System.Collections;
using Stone.Core;
using Stone.Pool;

public class Gun_Simple : BaseBehaviour 
{

	[SerializeField][Tooltip("模板对象")]
	private GameObject m_bulletTemplate;
	[System.NonSerialized][Tooltip("组件对象池")]
	private Pool_Comp m_compPool;
	[SerializeField][Tooltip("产生间隔")]
	private float m_fireRate=0.5f;
	[System.NonSerialized][Tooltip("产生计数")]
	private float m_fireTick;

	protected override void OnInitFirst()
	{
		m_compPool = Singletons.Get<Pool_Comp>("pool_comps");
		m_compPool.getList<Bullet>().setTemplate(m_bulletTemplate);
	}

	protected override void OnInitSecond()
	{

	}

	protected override void OnUpdate()
	{
		m_fireTick -= Time.deltaTime;
		if (m_fireTick < 0)
		{
			m_fireTick += m_fireRate;
			fire();
		}
	}
	protected void fire()
	{
		Bullet bullet =  m_compPool.takeUnit<Bullet>();
		bullet.m_transform.position = m_transform.position;
		bullet.m_transform.rotation = m_transform.rotation;
	}
}