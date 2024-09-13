// NFCカードの識別子
public enum CardID
{
    // 問題カード
    Question01_RedBlueArrow,
    Question02_Siritori,
    Question03_Snow,
    Question04_Loop,
    Question05_Bug,
    NoUse_Question06_TrafficJamAfterSiren,
    Question06_TrafficJam,
    Question07_FinalQuestion,

    // ヒントカード
    Hint01_Snow,
    Hint02_Car,
    Hint03_Fire,
    Hint04_Arrow,
    Hint05_KillBugSpray,
    Hint06_Speaker,
    Hint07_FinalHint,

    // 解答カード
    Answer01_Nuts,
    Answer02_JapaneseAlcohol,
    Answer03_BatteryCharger,
    // 交通系ICを読み取るのは別の処理となるので、ここでは使用しない
    // Answer04_SuicaICCard, 
    Answer05_Kokeshi,
    Answer06_Dog,
    Answer07_FinalAnswer,

    // ダミーのヒントカード
    HintDummy_SewingMachine,
    HintDummy_Grass,
    HintDummy_Pumpkin,
    HintDummy_SandClock,

    // 渋滞クイズのために追加
    // スピーカー設置後にサイレンを鳴らしたことを検知した場合に、特殊なヒントカードを読み込んだこととする
    Hint06_Siren,

    HintDummy_Maid,
}