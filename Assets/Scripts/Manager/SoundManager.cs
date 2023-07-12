using EnumDef;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    static public SoundManager Instance { get; private set; }

    public enum Type {
        Effect,     // 출력 타입 효과음
        BGM         // 출력 타입 배경음
    }

    public enum Repeat
    {
        Once,       // 한번 출력
        Loop        // 반복 출력
    }

    public enum BgmList
    {
        Order,      //목록 중 순서대로 재생
        Random      //목록 중 임의대로 재생
    }

    BgmList mNextBgmPlay = BgmList.Order;
    //글로벌 볼륨 : 조정시 효과, 배경 동시 조절(0 ~ 1)
    float mGlobalVolumn = 1;

    bool mIsAutoNextPlay = false;
    string mCurBgmSoundPath = "";
    string mCurEffectSoundPath = "";

    //배경 전용 AudioSource
    AudioSource mBgmAudioSource;
    //재생목록
    List<string> mBgmPlayList = new List<string>();
    //재생 목록 중 재생 중인 배경음의 인덱스
    int mBgmPlayIndex = 0;

    //효과 전용 AudioSource(일시 출력 전용)
    AudioSource mOneshotEffectAudioSource;

    //효과 전용 AudioSource(반복 출력 전용)
    List<AudioSource> mLoopEffectAudioSourceList = new List<AudioSource>();
    //Loop Effect AudioSource 생성시 발급되는 ID 생성기
    int mLoopEffectIdGenerator = 0;
    //재생목록
    List<string> mEffectPlayList = new List<string>();

    //출력 했던 오디오 리소스들의 목록
    Dictionary<string, AudioClip> mAudioClipPoolList = new Dictionary<string, AudioClip>();

    //컨트롤 패널 생성 여부
    bool mIsGuiControlPanel = true;

    public AudioClip kBGM;
    public AudioClip kHitEffect;
    public AudioClip kBombEffect;

    private void Awake()
    {
        Instance = this;

        normalStyle.fontSize = 12;
        normalStyle.normal.textColor = Color.white;

        boldStyle.fontSize = 12;
        boldStyle.normal.textColor = Color.yellow;
        boldStyle.richText = true;

        Init(false);
    }

	private void OnDestroy()
	{
		Instance = null;
	}

	//////////////////////////////////////////////////////////////////////////////////
	//*공용
	#region Common
	/// <summary>초기화(**선언 필수!**) / _isControlPanel : GUI 컨트롤 패널 등장 유무</summary>
	public void Init(bool _isControlPanel = true)
    {
        mIsGuiControlPanel = _isControlPanel;

        GameObject leftBgmGo = new GameObject();
        leftBgmGo.transform.parent = transform;
        leftBgmGo.transform.localPosition = Vector3.zero;
        leftBgmGo.name = "BGM_AudioSource";
        mBgmAudioSource = leftBgmGo.AddComponent<AudioSource>();

        GameObject effectGo = new GameObject();
        effectGo.transform.parent = transform;
        effectGo.transform.localPosition = Vector3.zero;
        effectGo.name = "Effect_OneShot_AudioSource";
        mOneshotEffectAudioSource = effectGo.AddComponent<AudioSource>();
        mOneshotEffectAudioSource.loop = false;
    }

    /// <summary>오디오클립 선행 로드</summary>
    bool Load(string _path)
    {
        if (mAudioClipPoolList.ContainsKey(_path) == true) {
            Debug.Log("읽으려는 '" + _path + "'오디오클립 형태의 리소스가 이미 존재합니다.");
            return false;
        }

        AudioClip clip = Resources.Load(_path) as AudioClip;
        if (clip == null) {
            Debug.Log("읽으려는 '" + _path + "'오디오클립 형태의 리소스가 없습니다.");
            return false;
        }

        mAudioClipPoolList[_path] = clip;

        return true;
    }

    /// <summary>배경 오디오소스 검색</summary>
    AudioSource GetBgmAudioSource()
    {
        return mBgmAudioSource;
    }

    /// <summary>효과 오디오소스 검색</summary>
    AudioSource GetOncshotEffectAudioSource()
    {
        return mOneshotEffectAudioSource;
    }

    /// <summary>현재까지 읽어들이 모든 오디오클립 중 획득</summary>
    public AudioClip GetAudioClip(string _path)
    {
        if (mAudioClipPoolList.ContainsKey(_path) == false) {
            if (Load(_path) == false) {
                Debug.Log("'" + _path + "' 가져오는데 실패 했습니다.");
                return null;
            }
        }

        return mAudioClipPoolList[_path];
    }

    /// <summary>모든 볼륨 통합 조정</summary>
    public void SetGlobalVolumn(float _value)
    {
        mGlobalVolumn = _value;
        SetBgmVolume(_value);
        SetEffectVolume(_value);
    }

    public float GetGlobalVolumn()
    {
        return mGlobalVolumn;
    }

    /// <summary>모든 소리 일시 멈춤</summary>
    public void Pause(bool _isPause)
    {
        if (_isPause == true)
        {
            mBgmAudioSource.Pause();
            mOneshotEffectAudioSource.Pause();

            foreach (var audio in mLoopEffectAudioSourceList)
                audio.Pause();

            mIsAutoNextPlay = false;
        }
        else
        {
            mBgmAudioSource.UnPause();
            mOneshotEffectAudioSource.UnPause();

            foreach (var audio in mLoopEffectAudioSourceList)
                audio.UnPause();

            mIsAutoNextPlay = true;
        }
    }

    /// <summary>모든 소리 멈춤</summary>
    public void AllStop()
    {
        StopBGM();
        AllStopEffect();
    }

    public void Clear()
    {
        AllStop();
        ClearBgmPlayList();
        ClearEffectPlayList();
        mAudioClipPoolList.Clear();
    }
    #endregion

    //////////////////////////////////////////////////////////////////////////////////
    //*BGM 관련
    #region Bgm
    /// <summary>재생 목록에 추가</summary>
    public void AddBgmPlayList(string _path)
    {
        if (mBgmPlayList.Contains(_path) == true) {
            Debug.Log("추가하려는 " + _path + "이(가) 이미 재생 목록에 있습니다.");
            return;
        }

        mBgmPlayList.Add(_path);
    }
    /// <summary>재생 목록에서 선택적 삭제</summary>
    public void RemoveBgmPlayList(string _path)
    {
        if (mBgmPlayList.Contains(_path) == false) {
            Debug.Log("삭제하려는 " + _path + "이(가) 이미 재생 목록에 없습니다.");
            return;
        }

        mBgmPlayList.Remove(_path);
    }

    /// <summary>재생 목록 모두 삭제</summary>
    public void RemoveBgmList()
    {
        mBgmPlayList.Clear();
    }

    /// <summary>배경 소리 크기 조절</summary>
    public void SetBgmVolume(float _value)
    {
        mBgmAudioSource.volume = _value;
    }

    public float GetBgmVolume()
    {
        return mBgmAudioSource.volume;
    }

    /// <summary>가장 최근 출력된 배경음 경로</summary>
    public string GetCurrentPlayBgm()
    {
        return mCurBgmSoundPath;
    }

    /// <summary>배경 재생</summary>
    public void PlayBgm(string _path)
    {
        AudioClip clip = GetAudioClip(_path);
        if (clip == null)
            return;

        int index = mBgmPlayList.IndexOf(_path);
        //기존의 없는 마지막 목록으로 추가
        if (index < 0) {
            mBgmPlayList.Add(_path);
            mBgmPlayIndex = mBgmPlayList.Count - 1;
        }

        mBgmAudioSource.clip = clip;
        mBgmAudioSource.loop = false;
        mBgmAudioSource.Play();
        mIsAutoNextPlay = true;

        mCurBgmSoundPath = _path;
    }

    /// <summary>목록에서 무작위 재생</summary>
    public void PlayRandomBgmList()
    {
        mNextBgmPlay = BgmList.Random;
        int selectIndex = Random.Range(0, mBgmPlayList.Count);

        mBgmPlayIndex = selectIndex;
        string _path = mBgmPlayList[mBgmPlayIndex];

        PlayBgm(_path);
    }

    /// <summary>목록 위에서 아래로 순서대로 재생</summary>
    public void PlayOrderBgmList(int _index)
    {
        mNextBgmPlay = BgmList.Order;
        if (_index >= mBgmPlayList.Count)
            _index = 0;
        if(_index < 0)
            _index = mBgmPlayList.Count - 1;

        mBgmPlayIndex = _index;
        string _path = mBgmPlayList[mBgmPlayIndex];

        PlayBgm(_path);
    }

    public void StopBGM()
    {
        mBgmAudioSource.Stop();
        mIsAutoNextPlay = false;
    }

    /// <summary>배경 재생 목록 해제</summary>
    void ClearBgmPlayList()
    {
        mBgmPlayList.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        AutoNextPlayUpdate();
    }

    public bool IsPlayBgm()
    {
        return mBgmAudioSource.isPlaying;
    }

    void AutoNextPlayUpdate()
    {
        if (mIsAutoNextPlay == false)
            return;

        if (mBgmAudioSource.isPlaying == true)
            return;
        if (mBgmPlayList.Count == 0)
            return;        

        switch (mNextBgmPlay)
        {
            case BgmList.Order:
                {
                    mBgmPlayIndex++;
                    if (mBgmPlayIndex >= mBgmPlayList.Count)
                        mBgmPlayIndex = 0;

                    PlayOrderBgmList(mBgmPlayIndex);
                }
                break;
            case BgmList.Random:
                PlayRandomBgmList();
                break;
        }
    }
    #endregion

    //////////////////////////////////////////////////////////////////////////////////
    //*Effect 관련
    #region Effect

    public void AddEffectList(string _path)
    {
        if (Load(_path) == true)
            mEffectPlayList.Add(_path);
    }

    public void RemoveEffectList(string _path)
    {
        mEffectPlayList.Remove(_path);
    }

    public void ClearEffectPlayList()
    {
        mEffectPlayList.Clear();
    }

    AudioSource CreateLoopEffectAudioSource()
    {
        //검색시 0은 mOneshotEffectAudiosource이며 mLoopEffectAudiosourceList의 key값 삽입에서 제외        
        GameObject effectGo = new GameObject();
        effectGo.transform.parent = transform;
        effectGo.transform.localPosition = Vector3.zero;
        effectGo.name = "Effect_Loop_AudioSource_ID_" + mLoopEffectIdGenerator.ToString();

        AudioSource audio = effectGo.AddComponent<AudioSource>();
        audio.volume = mOneshotEffectAudioSource.volume;
        mLoopEffectAudioSourceList.Add(audio);
        return audio;
    }

    public void SetEffectVolume(float _value)
    {        
        mOneshotEffectAudioSource.volume = _value;

        foreach (var audio in mLoopEffectAudioSourceList)
            audio.volume = _value;
    }

    public float GetEffectVolume()
    {
        return mOneshotEffectAudioSource.volume;
    }

    /// <summary>가장 최근 출력된 효과음 경로</summary>
    public string GetCurrentPlayEffect()
    {
        return mCurEffectSoundPath;
    }

    public AudioSource PlayEffect(string _path, Repeat repeat = Repeat.Once)
    {
        AudioClip clip = GetAudioClip(_path);
        if (clip == null)
            return null;

        AudioSource audio = null;

        switch (repeat)
        {
            case Repeat.Once:                
                mOneshotEffectAudioSource.PlayOneShot(clip);
                audio = mOneshotEffectAudioSource;
                break;
            case Repeat.Loop:                
                AudioSource loopAudio = CreateLoopEffectAudioSource();
                loopAudio.volume = mOneshotEffectAudioSource.volume;
                loopAudio.loop = true;
                loopAudio.clip = clip;
                loopAudio.Play();
                mLoopEffectAudioSourceList.Add(loopAudio);
                audio = loopAudio;
                break;
        }

        mCurEffectSoundPath = _path;

        if (mEffectPlayList.Contains(_path) == false)
            mEffectPlayList.Add(_path);

        //-1 : 재생 실패, 0 : 메인(OneShot), 1~ : 나머지는 생성(Loop)
        return audio;
    }
    public void StopLoopEffect(int _index)
    {        
        if (_index >= mLoopEffectAudioSourceList.Count || _index < 0 )
        {
            Debug.Log("멈춤에 실패 했습니다.");
            return;
        }

        AudioSource audio = mLoopEffectAudioSourceList[_index];
        audio.Stop();
        mLoopEffectAudioSourceList.RemoveAt(_index);
        Destroy(audio.gameObject);
    }

    public void StopOneshotEffect()
    {
        mOneshotEffectAudioSource.Stop();
    }
    
    /// <summary>모든 효과음 중지</summary>
    public void AllStopEffect()
    {
        mOneshotEffectAudioSource.Stop();

        foreach (var audio in mLoopEffectAudioSourceList){
            audio.Stop();
            Destroy(audio.gameObject);
        }

        mLoopEffectAudioSourceList.Clear();
    }
    #endregion

    GUIStyle normalStyle = new GUIStyle();
    GUIStyle boldStyle = new GUIStyle();

    [Header("배경음 로그창 설정")]
    public Rect windowRectBGM = new Rect(10, 10, 220, 400);
    [Header("효과음 로그창 설정")]
    public Rect windowRectEffect = new Rect(10 + 220, 10, 220, 400);
    private void OnGUI()
    {
        if (mIsGuiControlPanel == false)
            return;

        windowRectBGM = GUI.Window(0, windowRectBGM, SoundManagerBgmLogWindow,  "SoundManager BGM");
        windowRectEffect = GUI.Window(1, windowRectEffect, SoundManagerEffectLogWindow,  "SoundManager Effect");
    }

    void SoundManagerBgmLogWindow(int windowID)
    {
        GUILayout.Space(5f);

        GUI.Label(GUILayoutUtility.GetRect(220f, 20f), "배경음 볼륨", normalStyle);
        mBgmAudioSource.volume = GUI.HorizontalSlider(GUILayoutUtility.GetRect(100f, 20f), mBgmAudioSource.volume, 0f, 1f);

        GUILayout.BeginHorizontal(GUILayout.Width(100));
        //재생
        if (GUI.Button(GUILayoutUtility.GetRect(100f, 20f), "Play"))
            PlayOrderBgmList(mBgmPlayIndex);
        //멈춤
        if (GUI.Button(GUILayoutUtility.GetRect(100f, 20f), "Stop"))
            StopBGM();
        GUILayout.EndVertical();

        GUILayout.BeginHorizontal(GUILayout.Width(100));
        //일시정지
        if (GUI.Button(GUILayoutUtility.GetRect(100f, 20f), "Pause"))
            Pause(true);
        //계속
        if (GUI.Button(GUILayoutUtility.GetRect(100f, 20f), "Resume"))
            Pause(false);
        GUILayout.EndVertical();

        GUILayout.BeginHorizontal(GUILayout.Width(100));
        //이전 재생
        if (GUI.Button(GUILayoutUtility.GetRect(100f, 20f), "Prev Play"))
            PlayOrderBgmList(mBgmPlayIndex - 1);
        //다음 재생
        if (GUI.Button(GUILayoutUtility.GetRect(100f, 20f), "Next Play"))
            PlayOrderBgmList(mBgmPlayIndex + 1);        
        GUILayout.EndVertical();

        //랜덤 재생
        if (GUI.Button(GUILayoutUtility.GetRect(100f, 20f), "Random Play"))
            PlayRandomBgmList();

        GUILayout.Space(5f);
        string playMethod = "";
        switch(mNextBgmPlay){
            case BgmList.Order:     playMethod = "순차적"; break;
            case BgmList.Random:    playMethod = "무작위"; break;
        }

        GUI.Label(GUILayoutUtility.GetRect(220f, 20f), "재생 방법 - " + playMethod, normalStyle);

        for (int i = 0; i < mBgmPlayList.Count; i++)
        {
            if (mBgmPlayIndex == i)
                GUI.Label(GUILayoutUtility.GetRect(220f, 20f), "<b>" + (i + 1).ToString() + ". " + mBgmPlayList[i] + "</b>" + (mIsAutoNextPlay == true ? " (재생중)" : " (멈춤)"), boldStyle);
            else
                GUI.Label(GUILayoutUtility.GetRect(220f, 20f), (i + 1).ToString() + ". " + mBgmPlayList[i], normalStyle);
        }

        GUI.DragWindow();
    }

    [Header("효과음 목록 스크롤바 위치")]
    public Vector2 scrollPosition =new Vector2(0,0);
    [Header("효과음 목록 뷰 영역")]
    public Rect viewRect = new Rect(10f, 110f, 100f, 100f);
    [Header("효과음 목록 스크롤 영역")]
    public Rect scrollRect = new Rect(10f, 110f, 200f, 280f);
    void SoundManagerEffectLogWindow(int windowID)
    {
        GUILayout.Space(5f);

        GUI.Label(GUILayoutUtility.GetRect(220f, 20f), "효과음 볼륨", normalStyle);
        mOneshotEffectAudioSource.volume = GUI.HorizontalSlider(GUILayoutUtility.GetRect(100f, 20f), mOneshotEffectAudioSource.volume, 0f, 1f);
        SetEffectVolume(mOneshotEffectAudioSource.volume);

        //모두 멈춤
        if (GUI.Button(GUILayoutUtility.GetRect(100f, 20f), "Stop"))
        {
            AllStopEffect();
        }

        GUILayout.Space(5f);

        GUI.Label(GUILayoutUtility.GetRect(220f, 20f), "<b>최근 재생 - " + GetCurrentPlayEffect() + "</b>", boldStyle);

        viewRect.height = (mEffectPlayList.Count * 30f) + 10f;
        scrollPosition = GUI.BeginScrollView(scrollRect, scrollPosition, viewRect);
        
        for (int i = 0; i < mEffectPlayList.Count; i++)
        {            
            GUI.Label(GUILayoutUtility.GetRect(220f, 15f), (i + 1).ToString() + ". " + mEffectPlayList[i], normalStyle);
            GUILayout.BeginHorizontal(GUILayout.Width(100));
            if (GUI.Button(GUILayoutUtility.GetRect(90f, 15f), "Once"))
            {
                PlayEffect(mEffectPlayList[i]);
            }
            if (GUI.Button(GUILayoutUtility.GetRect(90f, 15f), "Loop"))
            {
                PlayEffect(mEffectPlayList[i], Repeat.Loop);
            }

            GUILayout.EndHorizontal();
        }

        GUI.EndScrollView();

        GUI.DragWindow();
    }

    public void PlayBgm()
    {
        AudioClip clip = kBGM;
        if (clip == null)
            return;

        mBgmAudioSource.clip = clip;
        mBgmAudioSource.loop = true;
        mBgmAudioSource.Play();
        mIsAutoNextPlay = true;
    }

    public void PlayHit()
    {
        AudioClip clip = kHitEffect;
        if (clip == null)
            return;

        mOneshotEffectAudioSource.PlayOneShot(clip);        
    }

    public void PlayBomb()
    {
        AudioClip clip = kBombEffect;
        if (clip == null)
            return;

        mOneshotEffectAudioSource.PlayOneShot(clip);
    }
}
