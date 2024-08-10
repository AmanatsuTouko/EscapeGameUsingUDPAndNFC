using UnityEngine;
using System;
using PCSC;
using PCSC.Iso7816;
using System.Linq;

public class NFCReader : MonoBehaviour
{
    private ISCardContext context;
    private ISCardReader reader;

    void Start()
    {
        context = ContextFactory.Instance.Establish(SCardScope.System);
        string[] readers = context.GetReaders();

        if (readers.Length <= 0)
        {
            Debug.LogError("No NFC readers found.");
            return;
        }

        foreach (var readerName in readers) {
            try {
                // スコープの外に出るとIDisposableを継承しているreaderは自動的に破棄される．
                // https://learn.microsoft.com/ja-jp/dotnet/csharp/language-reference/statements/using
                using (var reader = context.ConnectReader(readerName, SCardShareMode.Shared, SCardProtocol.Any)) {
                    PrintReaderStatus(reader);
                }
            } catch (Exception exception) {
                Debug.Log($"No card inserted or reader '{readerName}' is reserved exclusively by another application.");
                Debug.Log($"Error message: {exception.Message} ({exception.GetType()})");
            }
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

    void OnDestroy()
    {
        reader.Dispose();
        context.Dispose();
    }
}
