using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static LHS.CLHSDialogUI;

namespace LHS
{
	public class CLHSDialogUI: PopupBase
	{
		public TMP_Text m_Text;
		public Button m_Button_OK, m_Button_Yes, m_Button_No, m_Button_Cancel, m_Button_Custom1, m_Button_Custom2, m_Button_Custom3;
		public TMP_Text m_Button_Custom1_Text, m_Button_Custom2_Text, m_Button_Custom3_Text;
		public float m_2ButtonShiftRatio = 0.25f;

		public enum EButtonType { NONE=0, OK, YESNO, OKCANCEL, YESNOCANCEL, CUSTOM1, CUSTOM2, CUSTOM3 }
		private EButtonType m_ButtonType;

		public enum EDialogResult { NONE=0, OK, CANCEL, YES, NO, CUSTOM1, CUSTOM2, CUSTOM3 }

		// 초기 위치들
		private Vector2 m_ButtonPosition_OK, m_ButtonPosition_Yes, m_ButtonPosition_No, m_ButtonPosition_Cancel, m_ButtonPosition_Custom1, m_ButtonPosition_Custom2, m_ButtonPosition_Custom3;

		public delegate bool CallbackFunc(EDialogResult result);
		private CallbackFunc m_Callback;


		public void Init()
		{
			m_ButtonPosition_OK			= GetPositionOfButton(m_Button_OK);
			m_ButtonPosition_Yes		= GetPositionOfButton(m_Button_Yes);
			m_ButtonPosition_No			= GetPositionOfButton(m_Button_No);
			m_ButtonPosition_Cancel		= GetPositionOfButton(m_Button_Cancel);
			m_ButtonPosition_Custom1	= GetPositionOfButton(m_Button_Custom1);
			m_ButtonPosition_Custom2	= GetPositionOfButton(m_Button_Custom2);
			m_ButtonPosition_Custom3	= GetPositionOfButton(m_Button_Custom3);

			m_Button_OK.onClick.AddListener(() => OnButtonClicked(EDialogResult.OK));
			m_Button_Yes.onClick.AddListener(() => OnButtonClicked(EDialogResult.YES));
			m_Button_No.onClick.AddListener(() => OnButtonClicked(EDialogResult.NO));
			m_Button_Cancel.onClick.AddListener(() => OnButtonClicked(EDialogResult.CANCEL));
			m_Button_Custom1.onClick.AddListener(() => OnButtonClicked(EDialogResult.CUSTOM1));
			m_Button_Custom2.onClick.AddListener(() => OnButtonClicked(EDialogResult.CUSTOM2));
			m_Button_Custom3.onClick.AddListener(() => OnButtonClicked(EDialogResult.CUSTOM3));
		}
		private Vector2 GetPositionOfButton(Button button)
		{
			if (button == null)
				return Vector2.zero;

			return button.GetComponent<RectTransform>().anchoredPosition;
		}

		public void Show(string msg, EButtonType buttontype, CallbackFunc callback, string custom1=null, string custom2=null, string custom3=null)
		{
			m_Text.text = msg;
			m_Callback = callback;

			m_Button_OK.gameObject.SetActive(false);
			m_Button_Yes.gameObject.SetActive(false);
			m_Button_No.gameObject.SetActive(false);
			m_Button_Cancel.gameObject.SetActive(false);
			m_Button_Custom1.gameObject.SetActive(false);
			m_Button_Custom2.gameObject.SetActive(false);
			m_Button_Custom3.gameObject.SetActive(false);

			switch (buttontype)
			{
				case EButtonType.OK:	// OK, CANCEL 버튼 중간에 자리잡는다.
					m_Button_OK.GetComponent<RectTransform>().anchoredPosition = (m_ButtonPosition_OK + m_ButtonPosition_Cancel) / 2;

					m_Button_OK.gameObject.SetActive(true);
					break;

				case EButtonType.YESNO:	// Yes, Cancel 버튼(No 가 아님) 중간에 자리잡는다.
					m_Button_Yes.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(m_ButtonPosition_Yes, m_ButtonPosition_Cancel, m_2ButtonShiftRatio);
					m_Button_No.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(m_ButtonPosition_Yes, m_ButtonPosition_Cancel, (1 - m_2ButtonShiftRatio));

					m_Button_Yes.gameObject.SetActive(true);
					m_Button_No.gameObject.SetActive(true);
					break;

				case EButtonType.OKCANCEL: // OK, Cancel 버튼 중간에 자리잡는다.
					m_Button_OK.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(m_ButtonPosition_OK, m_ButtonPosition_Cancel, m_2ButtonShiftRatio);
					m_Button_Cancel.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(m_ButtonPosition_OK, m_ButtonPosition_Cancel, (1 - m_2ButtonShiftRatio));

					m_Button_OK.gameObject.SetActive(true);
					m_Button_Cancel.gameObject.SetActive(true);
					break;

				case EButtonType.YESNOCANCEL:
					m_Button_Yes.GetComponent<RectTransform>().anchoredPosition = m_ButtonPosition_Yes;
					m_Button_No.GetComponent<RectTransform>().anchoredPosition = m_ButtonPosition_No;
					m_Button_Cancel.GetComponent<RectTransform>().anchoredPosition = m_ButtonPosition_Cancel;
					m_Button_Yes.gameObject.SetActive(true);
					m_Button_No.gameObject.SetActive(true);
					m_Button_Cancel.gameObject.SetActive(true);
					break;

				case EButtonType.CUSTOM1:	// Custom1, Custom3 버튼 중간에 자리잡는다.(Custom3 가 없다면 Custom2)
					if (m_Button_Custom2 == null)
						m_Button_Custom1.GetComponent<RectTransform>().anchoredPosition = m_ButtonPosition_Custom1;
					else if (m_Button_Custom3 == null)
						m_Button_Custom1.GetComponent<RectTransform>().anchoredPosition = (m_ButtonPosition_Custom1 + m_ButtonPosition_Custom2) / 2;
					else
						m_Button_Custom1.GetComponent<RectTransform>().anchoredPosition = (m_ButtonPosition_Custom1 + m_ButtonPosition_Custom3) / 2;

					m_Button_Custom1_Text.text = custom1;

					m_Button_Custom1.gameObject.SetActive(true);
					break;

				case EButtonType.CUSTOM2:
					if (m_Button_Custom3 == null)
					{
						m_Button_Custom1.GetComponent<RectTransform>().anchoredPosition = m_ButtonPosition_Custom1;
						m_Button_Custom2.GetComponent<RectTransform>().anchoredPosition = m_ButtonPosition_Custom2;
					}
					else
					{
						m_Button_Custom1.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(m_ButtonPosition_Custom1, m_ButtonPosition_Custom3, m_2ButtonShiftRatio);
						m_Button_Custom2.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(m_ButtonPosition_Custom1, m_ButtonPosition_Custom3, (1 - m_2ButtonShiftRatio));
					}

					m_Button_Custom1_Text.text = custom1;
					m_Button_Custom2_Text.text = custom2;

					m_Button_Custom1.gameObject.SetActive(true);
					m_Button_Custom2.gameObject.SetActive(true);
					break;

				case EButtonType.CUSTOM3:
					m_Button_Custom1.GetComponent<RectTransform>().anchoredPosition = m_ButtonPosition_Custom1;
					m_Button_Custom2.GetComponent<RectTransform>().anchoredPosition = m_ButtonPosition_Custom2;
					m_Button_Custom3.GetComponent<RectTransform>().anchoredPosition = m_ButtonPosition_Custom3;

					m_Button_Custom1_Text.text = custom1;
					m_Button_Custom2_Text.text = custom2;
					m_Button_Custom3_Text.text = custom3;

					m_Button_Custom1.gameObject.SetActive(true);
					m_Button_Custom2.gameObject.SetActive(true);
					m_Button_Custom3.gameObject.SetActive(true);
					break;
			}

			m_ButtonType = buttontype;

			transform.SetAsLastSibling();	// 가장 앞으로 옮긴다.
			Show();
		}

		private void OnButtonClicked(EDialogResult result)
		{
			if (m_Callback != null)
			{
				if (!m_Callback(result))
					return;
			}

			m_ButtonType = EButtonType.NONE;
			m_Callback = null;

			base.Hide();
		}

		override public bool Hide()
		{
			switch(m_ButtonType)
			{
				case EButtonType.OKCANCEL:
				case EButtonType.YESNOCANCEL:
					OnButtonClicked(EDialogResult.CANCEL);
					break;

				case EButtonType.OK:
					OnButtonClicked(EDialogResult.OK);
					break;

				case EButtonType.YESNO:
					OnButtonClicked(EDialogResult.NO);
					break;
			}

			return true;
		}
	}
}
