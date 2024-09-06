using System;
using UnityEngine;

public class TimeManager : SingletonMonobehaviour<TimeManager>
{
    // タイマー
    [Header("Timer")]
    private Timer timer;

    public Timer Timer
    { 
        get
        {
            return timer;
        }
        private set
        {
            timer = value;
        }
    }

    // 初期制限時間 (Hours, Minites, Seconds)
    [SerializeField] HMSTimeStruct INIT_TIME_LIMIT = new HMSTimeStruct(1, 0, 0);

    [System.Serializable]
    public struct HMSTimeStruct
    {
        public int Hours;
        public int Minites;
        public int Seconds;

        public HMSTimeStruct(int hours, int minites, int seconds)
        {
            this.Hours = hours;
            this.Minites = minites;
            this.Seconds = seconds;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // タイマーの初期化
        var limit = INIT_TIME_LIMIT;
        timer = new Timer(new TimeSpan(limit.Hours, limit.Minites, limit.Seconds));
        // timer.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        // 残り時間の取得とUIの更新
        TimeSpan remainTime = timer.GetRemainTime();
        // UIManager.Instance.SetLimitTimeText(remainTime);
        Debug.Log(remainTime);
    }
}
