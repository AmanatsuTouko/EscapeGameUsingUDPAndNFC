using System;

public class Timer
{
    // コンストラクタ
    public Timer(TimeSpan limitTime)
    {
        this._startTime = DateTime.Now;
        this._limitTime = limitTime;
    }

    // タイマーを開始した時間
    private DateTime _startTime;

    // タイマーが開始時刻から hh:mm:ss 後に止まるか
    // (タイマーが同期できるようにする)
    private TimeSpan _limitTime;
    
    private bool _isStopipng = false;
    
    private DateTime _stopedTime;
    
    // Getter, Setter
    public DateTime StartTime {
        get
        {
            return _startTime;
        }
        set
        {
            _startTime = value;
        }
    }
    
    public TimeSpan LimitTime {
        get
        {
            return _limitTime;
        }
        set
        {
            _limitTime = value;
        }
    }
    
    // タイマーの残り時間を取得
    public TimeSpan GetRemainTime()
    {
        // タイマーストップ中はストップしたタイミングでの残り時間を計算する
        if(_isStopipng)
        {
            return (_startTime + LimitTime) - _stopedTime;
        }
        // タイマーカウント中はそのまま計算する
        else
        {
            return (_startTime + LimitTime) - DateTime.Now;
        }        
    }

    // タイマーの経過時間を取得
    public TimeSpan GetElapsedTime()
    {
        if(_isStopipng)
        {
            return _stopedTime - _startTime;
        }
        else
        {
            return DateTime.Now - _startTime;
        }
    }

    // 制限時間を増やす
    public void AddLimitTime(TimeSpan timeSpan)
    {
        // 開始時刻を前倒しにする
        _startTime += timeSpan;
    }

    // 制限時間を減らす
    public void SubLimitTime(TimeSpan timeSpan)
    {
        // 開始時刻を後ろ倒しにする
        this.AddLimitTime(-timeSpan);
    }

    // タイマーを停止する
    public void Stop()
    {
        _isStopipng = true;
        _stopedTime = DateTime.Now;
    }

    // タイマーを再開する
    public void ReStart()
    {   
        // どのくらい止めていたかのを計算する
        TimeSpan stopTimeDistance = DateTime.Now - _stopedTime;
        // 再開時に開始時刻を加算する
        this.AddLimitTime(stopTimeDistance);
        _isStopipng = false;
    }
}
