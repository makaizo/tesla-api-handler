# Unityでの環境構築メモ

## 概要

Tesla APIをUnityから利用するためのセットアップ手順を説明します。

## 1. 必要なパッケージのインストール

1. NugetForUnityをインストール
   * パッケージのインストールは他にもありそう。ひとまずNugetForUnityを使ったインストールで進めます
   * https://bluebirdofoz.hatenablog.com/entry/2024/01/10/223126 を参考にインストール

2. 必要なパッケージをインストール
   * 上位のバーから NuGet > Manage NuGet Packages
   * 以下のパッケージを検索してインストール
     * MQTTnet (4.0.2.221)
     * MQTTnet.Extensions.ManagedClient (4.0.2.221)
     * System.Text.Json (9.0.1)

## 2. ファイルを配置

1. 本環境をコピーしてUnityプロジェクトに配置
2. `Assets`配下に`StreamingAssets`フォルダを作成
3. そこに`credentials.json`を用意する

フォルダ構成の例

```
Assets
├── StreamingAssets        <--- 追加
│   └── credentials.json   <--- 追加
├── tesla-api-handler      <--- 本環境をコピー
│   ├── doc
│   └── TeslaAPIHandler
│       ├── Frontend
│       │   ├── how_to_setup_unity.md
│       │   └── ...
│       ├── Services
│       │   └── ...
│       └── ...
├── Packages
├── Plugins
├── Scenes
└── ...
```

## 3. TeslaApiManagerの設定

1. `TeslaApiManager.cs`をGameObjectやCube等にアタッチ
2. 必要に応じてMockの設定を切り替え
   * 本番環境（デフォルト）: useMock = false
   * モックを使用: useMock = true

## 4. 動作確認

1. プロジェクトを実行
2. コンソールログで取得データを確認

## 補足：パッケージ参照情報

```xml
<?xml version="1.0" encoding="utf-8"?>
<packages>
  <package id="MQTTnet" version="4.0.2.221" manuallyInstalled="true" />
  <package id="MQTTnet.Extensions.ManagedClient" version="4.0.2.221" manuallyInstalled="true" />
  <package id="Microsoft.Bcl.AsyncInterfaces" version="9.0.1" />
  <package id="System.IO.Pipelines" version="9.0.1" />
  <package id="System.Runtime.CompilerServices.Unsafe" version="6.0.0" />
  <package id="System.Text.Encodings.Web" version="9.0.1" />
  <package id="System.Text.Json" version="9.0.1" manuallyInstalled="true" />
</packages>
```
