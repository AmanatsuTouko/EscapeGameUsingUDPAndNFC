using UnityEngine;
using System;
using System.Linq;
using PCSC;
using PCSC.Iso7816;
using PCSC.Monitoring;

public class NFCReader : MonoBehaviour
{
    private ISCardContext context;
    private ISCardMonitor monitor;
    string mainReaderName;
    
    // カードの読み取り時に外部から関数を実行できるようにする
    public Action ActionOnReadCard;
    public Action ActionOnReleaseCard;

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
    }

    // カードリーダーの状態が変化したときに実行する関数
    private void StatusChanged(object sender, StatusChangeEventArgs args)
    {
        switch(args.NewState)
        {
            // カードを認識した状態
            case SCRState.Present:
                Debug.Log($"カードを認識. カードリーダーの状態:{args.NewState}");
                // UUIDなどを読み込んで表示
                ReadData();
                // カード読み込み時の関数を実行する
                ActionOnReadCard.Invoke();
                break;

            // カードが処理中（データの読み取りや書き込みを行っている可能性がある）
            case SCRState.Present | SCRState.InUse:
                Debug.Log($"カードの処理中. カードリーダーの状態:{args.NewState}");
                break;

            // カードが物理的にリーダから離れた状態
            case SCRState.Empty:
                Debug.Log($"カードを認識しなくなったことを検出．カードリーダーの状態:{args.NewState}");
                // カードリリース時の関数を実行する
                ActionOnReleaseCard.Invoke();
                break;

            default:
                Debug.LogError($"カードリーダーが設定されていない状態:{args.NewState}を検出.");
                break;
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
        Console.WriteLine("Card ATR: {0}", BitConverter.ToString(atr));
    }

    // read data
    // Reference : https://github.com/danm-de/pcsc-sharp/blob/master/Examples/ISO7816-4/Transmit/Program.cs
    private void ReadData() {

        ICardReader reader = context.ConnectReader(mainReaderName, SCardShareMode.Shared, SCardProtocol.Any);

        var apdu = new CommandApdu(IsoCase.Case2Short, reader.Protocol) {
            CLA = 0xFF,
            Instruction = InstructionCode.GetData,
            P1 = 0x00,
            P2 = 0x00,
            Le = 0 // We don't know the ID tag size
        };

        using (reader.Transaction(SCardReaderDisposition.Leave)) {
            Console.WriteLine("Retrieving the UID .... ");

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
                responseApdu.HasData ? BitConverter.ToString(responseApdu.GetData()) : "No uid received"));
        }
    }

    void OnApplicationQuit()
    {
        // カードリーダーの接続を終了する
        context.Dispose();

        // NFCカードリーダーの状態のモニタリングを終了する
        monitor.Cancel();
        monitor.Dispose();
    }
}
