using System;
using System.Collections.Generic;

public class EventFuncDispatcher
{
	private List<Func<bool> > Funcs = new List<Func<bool>>();
	public bool Consumed { get; private set; }

	public void AddFunc(Func<bool> func)
	{
		if (!Funcs.Contains(func))
			Funcs.Add(func);
	}

	public void DelFunc(Func<bool> func)
	{
		Funcs.Remove(func);
	}

	public void Dispatch(Func<Func<bool>, bool> func)
	{
		Consumed = false;

		for(int i=0; i<Funcs.Count; ++i)
		{
			// func 가 false 를 반환하면 중도에 멈춘다.

			if(!func(Funcs[i]))
			{
				Consumed = true;
				break;
			}
		}
	}
}
