# EscapeGameUsingUDPAndNFC

## Unity Version
- 

## Used Library
- PCSC-Sharp
- UniTask

## ubuntuへのPCSCのインストール

- version 20.04

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

これで、UbuntuにPC/SCがインストールされ、NFCカードの読み取りが可能になります。問題が発生した場合や、さらなるサポートが必要な場合は教えてください。


## ubuntuのUSBドライバのインストール

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
