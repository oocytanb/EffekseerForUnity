﻿# 更新履歴

## 1.43j

- EffekseerEmitterがUnityのレイヤーに対応
- パフォーマンスの改善 (モデルのインスタンシングに対応)
- モデルによる歪みに対応
- プレビュー中にUnityのゲームを実行するとフリーズする不具合の修正

## 1.43i

- リアルタイムのフラグありでリフレクションプローブを使用したときにメモリリークする不具合を修正
- 歪みフラグなしで歪みがあるエフェクトをUnityRendererで再生中のとき例外が発生する不具合を修正
- テクスチャの読み込みに失敗した場合に警告を出すようにした

## 1.43h

- MacOSと歪みの致命的なメモリリークを修正

## 1.43g

- Android 64bitに対応

## 1.43f

- EffekseerEmitterコンポーネントのチェックを外した時に表示しなくなるように変更

## 1.43e

- 歪みの設定をPCとモバイルに分離
- たまにMetal環境でクラッシュする不具合を修正
- ライティングが有効なモデルが表示されない不具合を修正
- フレームレートが60以上の時に高速化する不具合を修正
- UnityRendererで乗算が正しく描画されない不具合を修正
- モデルのメモリリークを修正

## 1.43d

- たまにエフェクトのホットリロードが失敗する不具合修正

## 1.43c

- 様々なメモリに関する不具合を修正しました (この修正はとても重要です)

## 1.43b

- iOSでパーティクルが多いとクラッシュするバグを修正しました

## 1.43
- リニューアル

## 1.40
- 歪みの仕様が変更された
- 関数の追加

## 1.30
- 歪みの仕様が変更された
- 座標系の仕様が変更された

## 1.23
- [Windows] Unity5.5βで正しく描画できるように対応
- [macOS] OpenGLCoreで正しく描画できるように対応
- インスタンス、四角形のデフォルト数を増加させた

## 1.22
- [Android] 稀に表示されないエフェクトがある不具合を修正
- カリングマスクに対応

## 1.21
- AssetBundleからのロードに対応
- [WebGL] WebGL出力に対応
- 軌跡タイプのエフェクトが描画されないバグを修正
- EffekseerEmitterとHandleにSetTargetLocationを追加
- EffekseerEmitterにpausedとshownを追加
- ヘルプの内容を加筆
- リファレンスマニュアルを追加

## 1.20
- 歪みエフェクトに対応
- EffekseerEmitterとHandleにStopRoot()を追加
- 更新処理をLateUpdateで行うように変更
- テクスチャ解放時の不具合を修正
- [iOS] Metal環境で実行したときにエラーを出力するように変更

## 1.10b
- [Windows] Deferred Renderingで正常に描画できるように修正
- [Mac] Deferred Renderingで正常に描画できるように修正
- [Android] β版追加
- [iOS] β版追加

## 1.10a
- [Windows] x86ビルドで落ちる不具合修正

## 1.10
- リソースの配置場所を StreamingAssets/Effekseer から Resources/Effekseer に変更
- ファイル読み込みにResources.Loadを使用するように変更
- サウンド再生にUnity標準のAudioSourceを使うように変更
- テクスチャロードにUnity標準のTexture2Dを使うように変更

## 1.01
- Unity5.2の新しいネイティブプラグイン仕様に対応