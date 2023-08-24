using System.Collections.Generic;
using UnityEngine;

// 모든 팝업의 Base 클래스
abstract public class PopupBase: MonoBehaviour
{
	static private List<PopupBase> s_PopupStack = new List<PopupBase>();

	static public bool IsTherePopup() { return s_PopupStack.Count > 0; }

	static public bool PopLast()
	{
		if (s_PopupStack.Count <= 0)
			return false;

		return s_PopupStack[s_PopupStack.Count - 1].Hide();
	}

	static public PopupBase GetLastPopup()
	{
		if(s_PopupStack.Count <= 0)
			return null;

		return s_PopupStack[s_PopupStack.Count - 1];
	}


	private void Awake()
	{
	}

	private void OnDestroy()
	{
		Hide();
	}

	virtual public void Show()
	{
		gameObject.SetActive(true);

		if (!s_PopupStack.Contains(this))
			s_PopupStack.Add(this);
	}

	virtual public void Show(bool isPopup)
	{
		gameObject.SetActive(true);

		if (isPopup && !s_PopupStack.Contains(this))
			s_PopupStack.Add(this);
	}

	virtual public bool CanHide()
	{
		return true;	// 이것이 false 이면 Hide 못함
	}

	virtual public bool Hide()
	{
		if (!CanHide())
			return false;

		gameObject.SetActive(false);

		if (s_PopupStack.Contains(this))
			s_PopupStack.Remove(this);

		return true;
	}

	virtual public void ProcessEscapeKey()
	{
		Hide();
	}

	public void HideMeAndAfter()
	{
		// 내 이후 전부 가려줌
		for(int i=0; i<s_PopupStack.Count; ++i)
		{
			if (s_PopupStack[i] == this)
			{
				while(s_PopupStack.Count > i)
				{
					if (s_PopupStack[s_PopupStack.Count - 1].Hide())
						break;
				}

				break;
			}
		}
	}
}