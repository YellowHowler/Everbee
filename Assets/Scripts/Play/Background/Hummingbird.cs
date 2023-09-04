using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EnumDef;
using StructDef;
using ClassDef;
using TMPro;

public class Hummingbird : MonoBehaviour
{
    [HideInInspector] public BirdState mCurState;

    [SerializeField] public SpriteRenderer kSprite;
    [SerializeField] public SpriteRenderer kPresentSprite;
    public SpriteOutline kOutline;
    public SpriteOutline kPresentOutline;

    [SerializeField] public Sprite kBaseImage;
    [SerializeField] public Sprite kHappyImage;

    [SerializeField] public GameObject kChat;
    [SerializeField] public Sprite kChatNeedImage;
    [SerializeField] public Sprite kChatHappyImage;
    [SerializeField] public Sprite kChatPresentImage;

    [SerializeField] public TMP_Text kReturnText;
    [SerializeField] public TMP_Text kNectarText;

    [HideInInspector] public GameResAmount mCurNectar = new GameResAmount(0f, GameResUnit.Microgram);
    [HideInInspector]public GameResAmount mNeedNectar = new GameResAmount(100f, GameResUnit.Microgram);

    [HideInInspector]public GameResType mPresentType;
    [HideInInspector]public int mPresent;
    [HideInInspector]public int mTotPresentCount = 4;

    [HideInInspector]public bool mIsHovered = false;
    [HideInInspector] public int mReturnTime = 0;

    void Start()
    {
        kOutline = GetComponent<SpriteOutline>();
        kPresentOutline = kPresentSprite.gameObject.GetComponent<SpriteOutline>();

        kOutline.DisableOutline();
        kPresentOutline.DisableOutline();

        UpdateState(BirdState.Need);
    }

    void Update()
    {
    
    }

    void UpdateState(BirdState _state)
    {
        mCurState = _state;

        switch(_state)
        {
            case BirdState.Need:
                kChat.SetActive(true);
                kSprite.sprite = kBaseImage;
                kPresentSprite.gameObject.SetActive(false);
                kChat.GetComponent<SpriteRenderer>().sprite = kChatNeedImage;

                for (int i = 0; i < kChat.transform.childCount; i++)
                {
                    kChat.transform.GetChild(i).gameObject.SetActive(true);
                }

                kReturnText.gameObject.SetActive(false);
                kSprite.enabled = true;

                break;
            case BirdState.Happy:
                kSprite.sprite = kHappyImage;
                kPresentSprite.gameObject.SetActive(false);
                kChat.GetComponent<SpriteRenderer>().sprite = kChatHappyImage;

                for (int i = 0; i < kChat.transform.childCount; i++)
                {
                    kChat.transform.GetChild(i).gameObject.SetActive(false);
                }

                break;
            case BirdState.Present:
                kChat.SetActive(true);
                kSprite.sprite = kBaseImage;
                kPresentSprite.gameObject.SetActive(true);
                kChat.GetComponent<SpriteRenderer>().sprite = kChatPresentImage;

                for (int i = 0; i < kChat.transform.childCount; i++)
                {
                    kChat.transform.GetChild(i).gameObject.SetActive(false);
                }

                kReturnText.gameObject.SetActive(false);
                kSprite.enabled = true;

                break;
            case BirdState.Absent:
                kSprite.enabled = false;
                kReturnText.gameObject.SetActive(true);
                kPresentSprite.gameObject.SetActive(false);
                kChat.SetActive(false);
                kReturnText.text = Mng.canvas.GetSecondsText(mReturnTime);

                for (int i = 0; i < kChat.transform.childCount; i++)
                {
                    kChat.transform.GetChild(i).gameObject.SetActive(false);
                }

                break;
            default:
                break;
        }
    }

    public void UpdatePresent(int _num)
    {
        int flowerCount = System.Enum.GetValues(typeof(FlowerType)).Length;

        if(_num < flowerCount)
        {
            mPresentType = GameResType.Seed;
            mPresent = _num;
            kPresentSprite.sprite = Mng.canvas.kSeedSprites[mPresent];
        }
        else
        {

        }
    }

    public void UpdatePresent(GameResType _presentType, int _num)
    {
        mPresentType = _presentType;
        mPresent = _num;
        
        switch(mPresentType)
        {
            case GameResType.Seed:
                kPresentSprite.sprite = Mng.canvas.kSeedSprites[mPresent];
                break;
            default:
                break;
        }
    }

    void OnMouseOver()
    {
        if(mCurState == BirdState.Absent)
        {
            return;
        }
        
        if(Mng.play.kHive.mIsPlacingItem && mCurState == BirdState.Need)
        {
            kOutline.EnableOutline();
        }
        else if (Mng.play.kHive.mIsPlacingItem == false && mCurState == BirdState.Present)
        {
            kPresentOutline.EnableOutline();
        }

        mIsHovered = true;
        Mng.play.kHive.mHoveredHummingbird = this;

        UpdateText();
        kChat.SetActive(true);
    }

    void OnMouseExit()
    {
        if(mCurState == BirdState.Absent)
        {
            return;
        }

        kOutline.DisableOutline();
        kPresentOutline.DisableOutline();

        mIsHovered = false;
        Mng.play.kHive.mHoveredHummingbird = null;
    }

    void OnMouseDown()
    {
        if (Mng.play.kHive.mIsPlacingItem == false && mCurState == BirdState.Present)
        {
            if(mPresentType == GameResType.Empty) 
            {
                return;
            }

            Mng.canvas.SpawnItemAtMousePos(mPresentType, mPresent, new GameResAmount(10f, GameResUnit.Microgram), ItemLoc.Hummingbird, -1);
            RemovePresent();
        }
    }

    public void RemovePresent()
    {
        mPresentType = GameResType.Empty;
        kPresentSprite.gameObject.SetActive(false);
    }

    public void ShowPresent()
    {
        kPresentSprite.gameObject.SetActive(true);
    }

    public void TookPresent()
    {
        if(mPresentType != GameResType.Empty)
        {
            return;
        }

        mCurNectar = new GameResAmount(0f, GameResUnit.Microgram);
        UpdateState(BirdState.Need);
    }

    public GameResAmount GiveNectar(GameResAmount _amount)
    {
        StartCoroutine(ReceiveNectarCor());
        return UpdateNectar(Mng.play.AddResourceAmounts(mCurNectar, _amount));
    }

    public GameResAmount UpdateNectar(GameResAmount _amount)
    {
        mCurNectar = _amount;

		GameResAmount retAmount = new GameResAmount(0f, GameResUnit.Microgram);

		if(Mng.play.CompareResourceAmounts(mNeedNectar, mCurNectar))
		{
			retAmount = Mng.play.SubtractResourceAmounts(mCurNectar, mNeedNectar);
			mCurNectar = new GameResAmount(0f, GameResUnit.Microgram);

            mReturnTime = 10;
            StartCoroutine(FetchSeedCor(false));
		}
		else
		{
			mCurNectar = _amount;	
		}

		UpdateText();

		return retAmount;
    }

    private void UpdateText()
    {
        kNectarText.text = Mng.canvas.GetAmountText(mCurNectar) + "\n/\n" + Mng.canvas.GetAmountText(mNeedNectar);
    }

    private IEnumerator FetchSeedCor(bool _isSave)
    {
        if(_isSave == false)
        {
            UpdateState(BirdState.Happy);
            yield return new WaitForSeconds(4);
            UpdateState(BirdState.Absent);
        }

        WaitForSeconds sec = new WaitForSeconds(1);

        while(mReturnTime > 0)
        {
            yield return sec;
            mReturnTime--;
            kReturnText.text = Mng.canvas.GetSecondsText(mReturnTime);
        }

        UpdatePresent(UnityEngine.Random.Range(0, mTotPresentCount));
        UpdateState(BirdState.Present);
    }

    private IEnumerator ReceiveNectarCor()
    {
        kSprite.sprite = kHappyImage;
        yield return new WaitForSeconds(1f);
        if(mCurState == BirdState.Need)
        {
            kSprite.sprite = kBaseImage;
        }
    }

    [Serializable]
	public class CSaveData
	{
        public BirdState mCurState;

        public GameResAmount mCurNectar = new GameResAmount(0f, GameResUnit.Microgram);
        public GameResAmount mNeedNectar = new GameResAmount(100f, GameResUnit.Microgram);

        public GameResType mPresentType;
        public int mPresent;
        public int mTotPresentCount = 4;

        public int mReturnTime = 0;
	}

	public void ExportTo(CSaveData savedata)
	{
        savedata.mCurState = mCurState;

        savedata.mCurNectar = mCurNectar;
        savedata.mNeedNectar = mNeedNectar;

        savedata.mPresentType = mPresentType;
        savedata.mPresent = mPresent;
        savedata.mTotPresentCount = mTotPresentCount;

        savedata.mReturnTime = mReturnTime;
	}

	public void ImportFrom(CSaveData savedata)
	{
        UpdateState(savedata.mCurState);

        UpdateNectar(savedata.mCurNectar);
        mNeedNectar = savedata.mNeedNectar;
        
        UpdatePresent(savedata.mPresentType, savedata.mPresent);
        mTotPresentCount = savedata.mTotPresentCount;

        mReturnTime = savedata.mReturnTime;
        StartCoroutine(FetchSeedCor(true));
	}
}
