# EscapeGameUsingUDPAndNFC

## Concept
- 2つのPCと2つのNECカードリーダーを用いて、様々な"もの"を読み込んで謎解きをする脱出ゲーム
- NFCカードやNFCコインタグを読み込むことで、"もの"を読み込む

## How To Play

### What You Need
- Unity が実行できるPC2台
- NFC card reader (実行環境では、SONY FeliCa Port/PaSoRi 4.0 を使用)
- NFC Card 7枚
- NFC Coin Tag 15枚程度

### Preparation
- 以下の必要なものを用意して、NFCコインタグを張り付ける
  <details>
  <summary>必要なものリスト</summary>
  
  - 雪
  - ミニカー
  - キャンドルなど火を連想させるもの
  - 矢印
  - 虫除けスプレー
  - スピーカー
  - ナッツ
  - 日本酒
  - 充電器
  - Suica
  - こけし
  - 犬の置き物

  </details>

- NFCコインタグのUUIDを登録する
  - ScriptableObject/UuidToCaradID に登録する
- NFCカードのUUIDを登録する
  - 同様に、ScriptableObject/UuidToCaradID に登録する
  - Question01 ～ Question07 が対象

### Steps To Play
- Unity側の設定
  - USBでPCにNFCカードリーダーを接続する
  - 2つのPCで同じWi-Fiに接続しておく
  - ConnectionClient GameObject の UdpSenderクラスの Remote Ip Adress 欄に送信先のプライベートIPアドレスを記入しておく
    - プライベートIPについては、Unityでゲームを実行するとログに出てくるようにしてある
  - DataBase GameObject の Databaseクラスの Client 欄の片方を FirstFloor, もう片方を Second Floor にする
- 脱出ゲームの会場の設営
  - NFCコインタグを張り付けた"もの"を、配置する
  - NFCカードを会場のどこかに隠す

- ※特殊な問題のクリア方法について
  - 以下、2つの問題は、カードを読み取る方法以外で正解を判定する
  - 渋滞のクイズについては、マウスホイールのクリックでクリア判定をゲームマスターが行う
    - 脱出ゲーム参加者がスピーカーでサイレンを鳴らしたら正解
  - 最終問題については、右クリック＋左クリック＋マウスホイールクリックでクリア判定をゲームマスターが行う

- 準備ができたら同じタイミングくらいでゲームを実行する

## Unity Version
- 2022.3.11f1

## Used Library
- [PCSC-Sharp](https://github.com/danm-de/pcsc-sharp) version 6.2.0
- [UniTask](https://github.com/Cysharp/UniTask) version 2.5.5

## Used Assets
- Sound
  - 効果音ラボ様：https://soundeffect-lab.info/
- Font
  - 日本語フリーフォント | マキナス 4 シリーズ | もじワク研究 様：https://moji-waku.com/makinas/
- Image
  - かわいいフリー素材集 いらすとや様：https://www.irasutoya.com/
  - 冬のイラスト | 商用可・フリーイラスト素材集｜ちょうどいいイラスト様：https://tyoudoii-illust.com/11999/
  - 雪の結晶のベクターイラスト – SILHOUETTE DESIGN様：https://kage-design.com/2015/01/01/xmassnow1/
  - SSL通信のシルエット04 | 無料のAi・PNG白黒シルエットイラスト様：https://www.silhouette-illust.com/illust/24651

## Install PCSC to ubuntu
<details>
<summary>version 20.04に PCSC をインストールする手順</summary>

UbuntuにPC/SC（`pcsc`）をインストールするには、以下の手順を実行する。

### 1. **パッケージリストを更新**
まず、ターミナルを開いてパッケージリストを最新にします。
```bash
sudo apt-get update
```

### 2. **PC/SC関連のパッケージをインストール**
次に、PC/SCのライブラリとデーモンをインストールします。以下のコマンドを実行してください。
```bash
sudo apt-get install libpcsclite1 pcscd pcsc-tools
```
- `libpcsclite1`: PC/SCライブラリを提供します。
- `pcscd`: PC/SCデーモンで、スマートカードリーダーと通信するために必要です。
- `pcsc-tools`: `pcsc_scan`などのツールが含まれています。

### 3. **PC/SCデーモンの動作確認**
インストール後、PC/SCデーモン（`pcscd`）が正しく動作しているか確認します。
```bash
sudo systemctl status pcscd
```
- デーモンが停止している場合は、以下のコマンドで起動します。
  ```bash
  sudo systemctl start pcscd
  ```
- 自動起動を有効にするには、以下のコマンドを実行します。
  ```bash
  sudo systemctl enable pcscd
  ```

### 4. **デバイスの確認**
デバイスが正しく認識されているか確認するには、`pcsc_scan`を実行します。
```bash
pcsc_scan
```
カードリーダーにカードを置くと、NFCカードの情報が表示されるはずです。

## Install drivers to ubuntu

### 1. **ドライバの問題**
   - **USBドライバのサポート**: UbuntuにSony PaSoRiデバイス用のUSBドライバが正しくインストールされていることを確認してください。通常、`libccid`はほとんどのスマートカードリーダーをサポートしますが、PaSoRi用の特定のドライバが不足しているか、正常に動作していない可能性があります。次のコマンドを実行して、必要なライブラリをインストールしてください。
     ```bash
     sudo apt-get install libusb-1.0-0-dev
     sudo apt-get install libccid
     ```
   - **カスタムUdevルール**: デバイスへのアクセスを適切に許可するために、カスタムのudevルールを設定する必要がある場合があります。以下のようにして、udevルールファイルを作成します。
     ```bash
     sudo nano /etc/udev/rules.d/99-pasori.rules
     ```
     次の行を追加します（`YOUR_VID`と`YOUR_PID`は、PaSoRiデバイスの実際のベンダーIDと製品IDに置き換えてください。`lsusb`コマンドで確認できます）。
     ```
     SUBSYSTEM=="usb", ATTR{idVendor}=="YOUR_VID", ATTR{idProduct}=="YOUR_PID", MODE="0666"
     ```
     その後、udevルールをリロードします。
     ```bash
     sudo udevadm control --reload-rules
     sudo udevadm trigger
     ```

### 2. **PC/SCデーモン（pcscd）の設定**
   - `pcscd`デーモンが正常に動作していることを確認します。
     ```bash
     sudo systemctl status pcscd
     ```
   - もし動作していなければ、以下のコマンドで起動します。
     ```bash
     sudo systemctl start pcscd
     ```
   - 自動起動を有効にします。
     ```bash
     sudo systemctl enable pcscd
     ```

</details>
