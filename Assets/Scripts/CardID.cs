// NFCカードの識別子
// ScriptableObjectの番号がずれるので、カードIDを追加するときは最下に追加すること
public enum CardID
{
    // 問題カード
    Question01_RedBlueArrow,
    Question02_Siritori,
    Question03_Snow,
    Question04_Loop,
    Question05_Bug,
    Question06_TrafficJam,
    Question07_FinalQuestion,

    // ヒントカード
    Hint01_Snow,
    Hint02_Car,
    Hint03_Fire,
    Hint04_Arrow,
    Hint05_KillBugSpray,
    Hint06_Speaker,
    Hint06_Siren, // 渋滞クイズのために追加 スピーカー設置後にサイレンを鳴らしたことを検知した場合に、特殊なヒントカードを読み込んだこととする

    // 解答カード
    Answer01_Nuts,
    Answer02_JapaneseAlcohol,
    Answer03_BatteryCharger,
    Answer04_SuicaICCard, // 交通系ICを読み取るのは別の処理となるので、デバッグ用途でのみ使用
    Answer05_Kokeshi,
    Answer06_Dog,

    // ダミーのヒントカード
    HintDummy_SewingMachine,
    HintDummy_Grass,
    HintDummy_Pumpkin,
    HintDummy_SandClock,
    HintDummy_Maid,
}