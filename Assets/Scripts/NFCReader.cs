using UnityEngine;
using System;
using System.Linq;
using PCSC;
using PCSC.Iso7816;
using PCSC.Monitoring;
using System.Threading;

public class NFCReader : MonoBehaviour
{
    private ISCardContext context;
    private ISCardMonitor monitor;
    string mainReaderName;
    
    // カードの読み取り時に外部から関数を実行できるようにする

    // NFCカードを読み込んだときに実行する関数(UUIDを引数に持つ)
    public Action<string> ActionOnReadCard;
    // 交通系ICを読み込んだ時に実行する関数
    public Action ActionOnReadTranspotationICCard;
    public Action ActionOnReleaseCard;

    // カード読み込み時にメインスレッドに処理を戻して実行できるようにする
    private SynchronizationContext mainThreadContext;    

    void Start()
    {
        // カードリーダーを取得
        context = ContextFactory.Instance.Establish(SCardScope.System);
        string[] readers = context.GetReaders();

        // カードリーダーの接続がない場合は終了
        if (readers.Length <= 0)
        {
            Debug.LogError("No NFC readers found.");
            return;
        }

        // カードリーダーの情報を表示する
        foreach (var readerName in readers) {
            try {
                // IDisposableインスタンスはusing句終了時に破棄される
                // https://learn.microsoft.com/ja-jp/dotnet/csharp/language-reference/statements/using
                using (var reader = context.ConnectReader(readerName, SCardShareMode.Shared, SCardProtocol.Any)){
                    PrintReaderStatus(reader);
                }
            } catch (Exception exception) {
                Debug.Log($"No card inserted or reader '{readerName}' is reserved exclusively by another application.");
                Debug.Log($"Error message: {exception.Message} ({exception.GetType()})");
            }
        }

        mainReaderName = readers[0];

        // カードの読み込みを監視するインスタンスを作成
        var monitorFactory = MonitorFactory.Instance;
        monitor = monitorFactory.Create(SCardScope.System);

        // カードを読み込んだ時に実行する関数を追加
        monitor.StatusChanged += StatusChanged;
        monitor.Start(mainReaderName);

        // メインスレッドのSynchronizationContextを取得
        mainThreadContext = SynchronizationContext.Current;
    }

    // カードリーダーの状態が変化したときに実行する関数
    private void StatusChanged(object sender, StatusChangeEventArgs args)
    {
        // メインスレッドに処理を戻して実行する
        // (UnityのUI操作などの関数はメインスレッドからしか実行できないため)
        mainThreadContext.Post(_ =>
        {
            switch(args.NewState)
            {
                // カードを認識 or 処理中（データの読み取りや書き込みを行っている可能性がある）
                case SCRState.Present:
                case SCRState.Present | SCRState.InUse:
                    Debug.Log($"カードを認識 or 処理中. カードリーダーの状態:{args.NewState}");
                    DoMethodOnScan();
                    break;

                // カードが物理的にリーダから離れた状態
                case SCRState.Empty:
                    Debug.Log($"カードを認識しなくなったことを検出．カードリーダーの状態:{args.NewState}");
                    // カードリリース時の関数を実行する
                    if(ActionOnReleaseCard != null)
                    {
                        ActionOnReleaseCard.Invoke();
                    }
                    break;
                
                // カードの認識はされているが，通信が行われていない状態
                // リーダーがカードと通信を開始するためのリスクエストを送っていない もしくは カードがリクエストに応じていない
                case SCRState.Present | SCRState.Mute:
                    Debug.Log($"カードを認識＋通信の失敗を検出．カードリーダーの状態:{args.NewState}");
                    break;

                default:
                    Debug.LogError($"カードリーダーが設定されていない状態:{args.NewState}を検出.");
                    break;
            }
        }, null);
    }

    void DoMethodOnScan()
    {
        // UUIDを取得する
        string uuid = GetUUIDByReadData();
        // 交通系ICかどうか判定する
        bool isTransportationICCard = IsTransportationICCard();
        Debug.Log($"UUID: {uuid}, 交通系ICかどうか: {isTransportationICCard}");

        // どのカードIDかを出力する(デバッグ用)
        CardID? cardID = DataBase.Instance.GetCardIDFromUUID(uuid);
        if(cardID != null)
        {
            CardType? cardType = DataBase.Instance.GetCardTypeFromCardID((CardID)cardID);
            Debug.Log($"CardID:{cardID}, CardType:{cardType}");
        }

        // カード読み込み時の関数を実行する
        if (ActionOnReadCard != null)
        {
            ActionOnReadCard.Invoke(uuid);
        }
        if (ActionOnReadTranspotationICCard != null && isTransportationICCard)
        {
            ActionOnReadTranspotationICCard.Invoke();
        }
    }

    void OnApplicationQuit()
    {
        // カードリーダーの接続を終了する
        context.Dispose();

        // NFCカードリーダーの状態のモニタリングを終了する
        if(monitor != null)
        {
            monitor.Cancel();
            monitor.Dispose();
        }
        else
        {
            Debug.LogError("No NFC readers found. Monitoring cannot exit.");
        }
    }

    private static void PrintReaderStatus(ICardReader reader) {
        try {
            var status = reader.GetStatus();
            Debug.Log($"Reader {status.GetReaderNames().FirstOrDefault<string>()} connected with protocol {status.Protocol} in state {status.State}");
            PrintCardAtr(status.GetAtr());
        } catch (Exception exception) {
            Debug.LogError($"Unable to retrieve card status.\nError message: {exception} ({exception.GetType()}");
        }
    }

    private static void PrintCardAtr(byte[] atr) {
        if (atr == null || atr.Length <= 0) {
            return;
        }
        Debug.Log($"Card ATR: {BitConverter.ToString(atr)}");
    }

    // UUIDを取得する
    // Reference : https://github.com/danm-de/pcsc-sharp/blob/master/Examples/ISO7816-4/Transmit/Program.cs
    private string GetUUIDByReadData() {

        ICardReader reader = context.ConnectReader(mainReaderName, SCardShareMode.Shared, SCardProtocol.Any);

        // ISO7816のAPDUコマンドを使用してカードからデータを読み取る
        var apdu = new CommandApdu(IsoCase.Case2Short, reader.Protocol) {
            CLA = 0xFF,
            Instruction = InstructionCode.GetData,
            P1 = 0x00,
            P2 = 0x00,
            Le = 0 // We don't know the ID tag size
        };

        using (reader.Transaction(SCardReaderDisposition.Leave)) {
            Debug.Log("Retrieving the UID .... ");

            var sendPci = SCardPCI.GetPci(reader.Protocol);
            var receivePci = new SCardPCI(); // IO returned protocol control information.

            var receiveBuffer = new byte[256];
            var command = apdu.ToArray();

            var bytesReceived = reader.Transmit(
                sendPci,             // Protocol Control Information (T0, T1 or Raw)
                command,             // command APDU
                command.Length,
                receivePci,          // returning Protocol Control Information
                receiveBuffer,
                receiveBuffer.Length // data buffer
            );

            var responseApdu = new ResponseApdu(receiveBuffer, bytesReceived, IsoCase.Case2Short, reader.Protocol);
            Debug.Log(
                string.Format("SW1: {0:X2}, SW2: {1:X2}\nUid: {2}",
                responseApdu.SW1,
                responseApdu.SW2,
                responseApdu.HasData ? BitConverter.ToString(responseApdu.GetData()) : "No uid received")
            );
            
            // UUIDを返却する
            // 操作が正常に完了した かつ データを持っているとき
            if(responseApdu.HasData && responseApdu.SW1 == 0x90 && responseApdu.SW2 == 0x00)
            {
                return BitConverter.ToString(responseApdu.GetData());
            }
            else
            {
                Debug.LogError("NFCカードのデータの読み取り操作が正常に完了しませんでした．");
                return "";
            }
        }
    }


    // 交通系ICかどうかの判別を，残高があるかどうかで判別する
    // https://office-fun.com/techmemo-csharp-nfcreading-practice01/
    // https://marunouchi-tech.i-studio.co.jp/5133/
    // https://tomosoft.jp/design/?p=5543
    private bool IsTransportationICCard()
    {
        var contextFactory = ContextFactory.Instance;
 
        using (var context = contextFactory.Establish(SCardScope.System))
        {
            // using句を使うことで，using句を出るときに自動的にreaderインスタンスを破棄する
            using (var rfidReader = context.ConnectReader(mainReaderName, SCardShareMode.Shared, SCardProtocol.Any))
            {
                using (rfidReader.Transaction(SCardReaderDisposition.Leave))
                {
                    // 交通系ICかどうかを判別する

                    // Send Select File Command
                    byte[] commnadSelectFile = { 0xff, 0xA4, 0x00, 0x01, 0x02, 0x0f, 0x09 };
                    byte[] commnadReadBinary = { 0xff, 0xb0, 0x00, 0x00, 0x00 };
        
                    var sendPci = SCardPCI.GetPci(rfidReader.Protocol);
                    var receivePci = new SCardPCI(); // IO returned protocol control information.
                    var receiveBuffer = new byte[256];
        
                    var bytesReceived = rfidReader.Transmit(
                        sendPci,                     // Protocol Control Information (T0, T1 or Raw)
                        commnadSelectFile,           // command APDU
                        commnadSelectFile.Length,
                        receivePci,                  // returning Protocol Control Information
                        receiveBuffer,
                        receiveBuffer.Length         // data buffer
                    );

                    var responseApdu = new ResponseApdu(receiveBuffer, bytesReceived, IsoCase.Case2Short, rfidReader.Protocol);
                    Debug.Log("SW1: " + responseApdu.SW1.ToString() + ", SW2: " + responseApdu.SW2.ToString());

                    // コマンドが正常に実行されなかった場合は交通系ICではない
                    // https://tex2e.github.io/blog/protocol/apdu-return-status-msg
                    if(responseApdu.SW1 != 144 && responseApdu.SW2 != 0)
                    {
                        return false;
                    }

                    if (responseApdu.HasData)
                    {
                        Debug.Log("Uid: " + BitConverter.ToString(responseApdu.GetData()));
                    }
                    else
                    {
                        Debug.Log("Uid: No uid received");
                    }

                    // 残高を読み出す
                    bytesReceived = rfidReader.Transmit(
                        sendPci,                     // Protocol Control Information (T0, T1 or Raw)
                        commnadReadBinary,           // command APDU
                        commnadReadBinary.Length,
                        receivePci,                  // returning Protocol Control Information
                        receiveBuffer,
                        receiveBuffer.Length         // data buffer
                    );

                    responseApdu = new ResponseApdu(receiveBuffer, bytesReceived, IsoCase.Case2Short, rfidReader.Protocol);
                    Debug.Log("SW1: " + responseApdu.SW1.ToString() + ", SW2: " + responseApdu.SW2.ToString());

                    // コマンドが正常に実行されなかった場合は交通系ICではない
                    if(responseApdu.SW1 != 144 && responseApdu.SW2 != 0)
                    {
                        return false;
                    }

                    if (responseApdu.HasData)
                    {
                        GetBalanceParsingRecievedByte(responseApdu.GetData());
                    }
                    else
                    {
                        Debug.Log("No data received");
                    }

                    // コマンドが正常に実行されれば交通系IC
                    return true;
                }                
            }
        }
    }

    private int GetBalanceParsingRecievedByte(byte[] data)
    {
        // 残高情報が10バイト目と11バイト目にあるので整数値にパースする
        byte[] balance = new byte[] { data[10], data[11] };
        Debug.Log("残高：" + BitConverter.ToInt16(balance, 0) + "円");
        return BitConverter.ToInt16(balance, 0);
    }    
}
