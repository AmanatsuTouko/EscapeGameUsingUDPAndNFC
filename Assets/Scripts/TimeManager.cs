using System;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : SingletonMonobehaviour<TimeManager>
{
    // タイマー
    [Header("Timer")]
    private Timer timer;

    // 初期制限時間 (Hours, Minites, Seconds)
    [SerializeField] HMSTimeStruct INIT_TIME_LIMIT = new HMSTimeStruct(1, 0, 0);

    void Start()
    {
        // タイマーの初期化
        var limit = INIT_TIME_LIMIT;
        timer = new Timer(new TimeSpan(limit.Hours, limit.Minites, limit.Seconds));
        // timer.Stop();
    }

    void Update()
    {
        // 残り時間の取得とUIの更新
        TimeSpan remainTime = timer.GetRemainTime();
        // 時間の整形
        string remainText = $"{remainTime.Hours.ToString("00")}:{remainTime.Minutes.ToString("00")}:{remainTime.Seconds.ToString("00")}";
        // UIの更新
        UIManager.Instance.SetLimitTimeText(remainText);

        // 1秒経過ごとにクイズの表示を更新する
        if(CircleClockQuizManager.Instance.IsDisplayed() && IsPassedOneSecond(remainTime))
        {
            CircleClockQuizManager.Instance.DisplayCircleNumber(GetIncludedNumbers(remainTime));
        }
    }

    // 1秒経過したかどうか
    int preSecond = 0;
    private bool IsPassedOneSecond(TimeSpan now)
    {
        bool isPassed = false;
        if(now.Seconds != preSecond)
        {
            isPassed = true;
        }
        preSecond = now.Seconds;
        return isPassed;
    }

    // 現在の時刻に含まれる数字を取得する
    private List<int> GetIncludedNumbers(TimeSpan now)
    {
        HashSet<int> set = new HashSet<int>();

        string h = now.Hours.ToString("00");
        set.Add((int)(h[0] - '0'));
        set.Add((int)(h[1] - '0'));
        string m = now.Minutes.ToString("00");
        set.Add((int)(m[0] - '0'));
        set.Add((int)(m[1] - '0'));
        string s = now.Seconds.ToString("00");
        set.Add((int)(s[0] - '0'));
        set.Add((int)(s[1] - '0'));

        return new List<int>(set);
    }

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
}
