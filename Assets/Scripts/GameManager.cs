using System;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TMPro;

public class GameManager : SingletonMonobehaviour<GameManager>
{
    // UDP通信
    [Header("UDP Connection")]
    [SerializeField] UdpSender udpSender;
    [SerializeField] UdpReceiver udpReceiver;
    [SerializeField] DisplayLocalIP displayLocalIP;

    // NFCカードの読み込み
    [Header("Read NFC Card")]
    [SerializeField] NFCReader nfcReader;

    // 1階もしくは2階のデータ
    [Header("Quiz Image Corresponding to Card ID")]
    [SerializeField] ClientScriptableObject clientScriptableObject;

    // NECカードのUUIDとCardIDの対応付けを定義したScriptableObject
    [SerializeField] UUIDToCardIDScriptableObject uuidToCardIdDictScriptableObject;

    // クイズ画像表示用UI
    [Header("UI")]
    [SerializeField] Image QuizDisplayImage;
    [SerializeField] Slider ProgressBarSlider;

    void Start()
    {
        // Imageの反映先を自身の持つImageクラスにする
        clientScriptableObject.image = QuizDisplayImage;

        // カード読み取り時に実行する関数を登録する
        nfcReader.ActionOnReadCard += DisplayImageOnRemoteClientFromUUID;

        // 交通系IC読み取り時に実行する関数を登録する
        nfcReader.ActionOnReadTranspotationICCard += ActionOnReadTransportationICCard;

        // UDP受信時に実行する関数を登録する
        udpReceiver.ActionRecieveData += OnRecieveMessage;
    }

    // 別クライアントでカード読み取り時にNFCカードから取得したUUIDを引数にして画像を表示する関数を起動するメッセージをUDPで送信する
    public void DisplayImageOnRemoteClientFromUUID(string uuid)
    {
        // UUIDを引数に，画像を表示する関数を別クライアントで実行する
        string jsonMethod = RPCManager.GetJsonFromMethodArgs(nameof(RPCStaticMethods), nameof(RPCStaticMethods.DisplayQuestionImage), new string[]{uuid});
        udpSender.SendMessage(jsonMethod);
    }

    // 交通系IC読み取り時に実行する関数
    void ActionOnReadTransportationICCard()
    {
        // 特殊なクイズ画像の表示
    }

    // 受信した文字列に応じて関数を実行する
    void OnRecieveMessage(string receivedJson)
    {
        try
        {
            RPCManager.InvokeFromJson(receivedJson);
        }
        catch
        {
            Debug.LogError("関数として無効な文字列を受信したため，動作を終了しました．");
        }
    }

    // NFCカードの識別番号と対応するクイズ画像のデータを更新する
    void UpdateClientScriptableObject(ClientScriptableObject clientScriptableObject)
    {
        this.clientScriptableObject = clientScriptableObject;
        clientScriptableObject.image = QuizDisplayImage;
    }

    // ==== UI操作 ====

    // 受信した時に実行する関数
    public void DisplayQuestionImage(string uuidString)
    {
        DisplayQuestionImageWithProgressBarUniTask(uuidString).Forget();
    }

    private async UniTask DisplayQuestionImageWithProgressBarUniTask(string uuidString)
    {
        // 文字列のUUIDからCardIDに変換する
        CardID? cardID = uuidToCardIdDictScriptableObject.GetCardIDFromUUID(uuidString);
        if (cardID == null)
        {
            Debug.LogError($"{uuidString}をCardIDに変換できないため，処理を停止します．");
            return;
        }
        // cardIDに応じた処理を行う
        clientScriptableObject.DisplayQuestionImage((CardID)cardID);

        // イージングを繋ぐタイミングをややランダムにする
        float addSeconds_01 = UnityEngine.Random.Range(-0.1f, 0.1f);
        float addSeconds_02 = UnityEngine.Random.Range(-0.1f, 0.1f);

        // イージングの種類をランダムにする(弾性や振動を除く)
        int enumLength = Enum.GetNames(typeof(Easing.Ease)).Length - 9;
        int easeRandIdx_01 = UnityEngine.Random.Range(0, enumLength);
        int easeRandIdx_02 = UnityEngine.Random.Range(0, enumLength);
        int easeRandIdx_03 = UnityEngine.Random.Range(0, enumLength);

        // プログレスバーを3段階に分けて上昇させる
        UpdateProgressText("組成を分析中……");
        await EasingSecondsFromTo(1.5f, 0.0f,                  0.25f + addSeconds_01, (Easing.Ease)easeRandIdx_01);

        UpdateProgressText("物質を構成中……");
        await EasingSecondsFromTo(1.0f, 0.25f + addSeconds_01, 0.55f + addSeconds_02, (Easing.Ease)easeRandIdx_02);
        await UniTask.Delay(500);

        UpdateProgressText("生成物を検証中……");
        await EasingSecondsFromTo(1.5f, 0.55f + addSeconds_02, 1.0f,                  (Easing.Ease)easeRandIdx_03);

        // ImageをONにする
        DisplayImageSetActive(true);

        // リセット処理
        ProgressBarSlider.value = 0;
    }

    // N秒でaからbまでイージングを行う関数
    private async UniTask EasingSecondsFromTo(float seconds, float fromValue, float toValue, Easing.Ease easing)
    {
        Func<float, float> easingMethod = Easing.GetEasingMethod(easing);
        float rate = 0;
        float sub = toValue - fromValue;
        while (rate < 1.0f)
        {
            await UniTask.Yield();
            rate += Time.deltaTime / seconds;
            if (rate >= 1.0f) rate = 1.0f;
            ProgressBarSlider.value = fromValue + easingMethod(rate) * sub;
        }
    }

    private void UpdateProgressText(string text)
    {
        // プログレスバーに記載されている文字を変更する
    }

    // Imageのオンオフを行う
    public void DisplayImageSetActive(bool active)
    {
        clientScriptableObject.image.enabled = active;
    }

    // === UI操作 ===

    public UUIDToCardIDScriptableObject GetUuidToCardIdDictScriptableObject()
    {
        return uuidToCardIdDictScriptableObject;
    }
}