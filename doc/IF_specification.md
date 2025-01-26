**TeslaAPIHandler\Services内のIFを使う際の注意点をまとめる。**
* read系、write系のWakeUpは実際にFleetAPIが実行されるため、[料金](#teslafleetapiの料金)のことを考えて呼びすぎないこと
  * 25/1/26時点では車両スリープ状態のエラー処理ができていないため、スリープ時はread系を実行できない。その場合はWakeUpを先に実行すること。（エラー処理でスリープで実行できないときはWakUpを自動で読んでからリトライするように修正予定）
* WakeUp以外のwrite系は、セキュリティ対策のためプロキシサーバ経由にする必要があり、これは別のプログラム（[システム構成図](##システム構成)のラズパイ）を動かさないと実行されない。（つまりARグラスで操作しただけで急にトランクが開くとかはない）
* requirements.txt内のライブラリが必要
* credentials.jsonはvinやトークンなどの機密情報を含むため、個別に送る。（githubへアップロードしないこと）

## システム構成
![システム構成](/doc/images/2025-01-25-09-04-59.png)

## TeslaFleetAPIの料金
https://developer.tesla.com/docs/fleet-api/billing-and-limits
https://developer.tesla.com/#usage-based-pricing
* 25/1から課金開始
* $10分の無料枠が毎月もらえる
* $0.01/requestほど
  * カテゴリによって違う
  * 一番高いwake_upで$1/50requests