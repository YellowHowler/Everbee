using System;
using System.Collections.Generic;

public class CTargetLink<OwnerT, T>
{
	public OwnerT m_Owner { get; private set;}

	public CTargetLink<T, OwnerT> m_Target { get; private set; }

	public CTargetLink(OwnerT owner)
	{
		m_Owner = owner;
	}

	public T GetObject() { return m_Target != null ? m_Target.m_Owner : default(T); }
	public bool IsLinked() { return m_Target != null; }

	public void LinkTo(CTargetLink<T, OwnerT> target)
	{
		if (m_Target != null)
		{
			if (m_Target == target)
				return;

			Unlink();
		}

		if (target != null)
		{
			m_Target = target;
			target.LinkTo(this);
		}
	}

	public void Unlink()
	{
		if (m_Target == null)
			return;

		var target = m_Target;
		m_Target = null;

		target.Unlink();
	}
}
