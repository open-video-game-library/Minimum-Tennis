# Minimum Tennis

Minimum Tennisは、現実のテニスに則ったシンプルなテニスゲームです。

前後左右に移動しながら、タイミングよくボールを打ち返しましょう。

実験のノイズとならないように、キャラクターの見た目ではプリミティブなデザインを採用し、キャラクターへの先入観の要因となる、人種・体格・性別を排除しました。

![MiniumuTennis](https://user-images.githubusercontent.com/77042312/187229293-41eef3ab-3adc-469b-8806-3919aefdc7d5.png)

## Contents

ゲームの概要，機能，調整できるパラメータ，出力できるパラメータ，想定される利用方法などを画像を用いて記載  
ゲームの操作方法やルールもここに記載

### ルール

基本的なルールは、現実のテニスに則っています。

- 失点となる行為
	- 打ったボールが相手コートに入らない
	- 打ったボールがネットにかかる
	- 相手の打ったボールが自分のコート内で2回バウンドする前に打ち返さない
	- 打ったサーブが2回連続でフォールトとなる 

- デュース
	- 1つのゲームにおいて、両者が3回ずつ得点した場合はデュースとなる
	- デュースとなった場合、その状態からどちらかが2回連続で得点すると、そのゲームを獲得できる
	- 2回連続で得点できなかった場合は、再びデュースとなる

- 勝敗
	- パラメータ調整機能によって指定されたゲーム数を先に獲得すると勝利となる
	- ◯セットマッチではなく、◯ゲーム先取である点に注意

### 操作方法

- キーボード操作  
![MinimumTennis_操作方法_キーボード](https://user-images.githubusercontent.com/77042312/187409521-a9babea3-39ee-4838-af45-43a4e764ecf7.png)

- ゲームパッド操作  
![MinimumTennis_操作方法_ゲームパッド](https://user-images.githubusercontent.com/77042312/187409768-07f14ef2-a8f3-418d-82cd-848223f3fe47.png)

- Joy-Con操作（モーション操作）  
画像

### 機能

- パラメータ調整機能

	- 以下のパラメータをゲーム画面上で調整できます
	
		- プレイヤと対戦相手の移動速度
		- プレイヤと対戦相手の打つボールの速度
		- プレイヤの反応速度（モーション操作時）
		- 対戦相手の反応速度
		- 対戦相手のショットに辿り着くまでの移動距離
		- プレイスタイル
		- 試合終了条件

- パラメータ出力機能

	- 以下のパラメータをゲーム終了時にCSVファイルとして出力できます
	
		- 勝敗
		- プレイヤが勝ち取ったゲーム数
		- 対戦相手が勝ち取ったゲーム数
		- プレイヤのネットした回数
		- プレイヤのアウトした回数
		- プレイヤの2バウンドした回数
		- プレイヤのダブルフォルトした回数
		- 最大ラリー回数

- 対戦プレイ

	- パソコンにゲームパッドを2つ接続することで、2人で対戦プレイをすることができます
	- 下の画像のように、タイトル画面に現在登録されているコントローラ数が表示されます（下の画像では2つ接続されている）
	
	![MinimumTennis_登録コントローラ数](https://user-images.githubusercontent.com/77042312/187391138-cc945035-79b5-4f0b-b90d-22efeb7b9c2e.png)
	
	- ホーム画面で「Competition」ボタンをクリックすると、対戦プレイの画面へ遷移します


- 複数のコントローラによる操作

	- 本ゲームは、キーボード操作/ゲームパッド操作/Joy-Con操作（モーション操作） の3つに対応しています
	- キーボード操作/ゲームパッド操作でプレイする場合は、ホーム画面で「Normal Control」をクリックして下さい
	- Joy-Con操作（モーション操作）でプレイする場合は、ホーム画面で「Motion Control」をクリックして下さい

## Operating Environment

Windows, Mac, Unity 2021.1.17f1

## Programming Languages

Unity, C#

## Introduction

コンテンツの導入方法を記載

## How to Use

使い方を記載

## Licence

1. Minumum Tennis は無料でご利用できます。

2. 営利目的・公序良俗に反する目的でない限り、研究目的での利用を含め、あらゆる用途で利用できます。

3. ダウンロードしたデータを改変し、ご利用いただいても構いません。

4. 利用報告をする必要はありませんが、改変したデータを二次配布する場合を含め、Minumum Tennis を利用した場合は、その旨を明記して下さい。

5. Minumum Tennis を研究で利用する場合は、ご自身の論文内で以下の論文を引用して下さい。
    1. [研究利用しやすく標準性を目指したビデオゲームの設計と開発](http://id.nii.ac.jp/1001/00212465/)
    2. [研究者が利用しやすいオープンなスポーツゲームの試作](http://www.interaction-ipsj.org/proceedings/2022/data/pdf/4D18.pdf)

## Note

- Minumum Tennis からダウンロードしたものを、自身が開発したものと偽って公開する行為はご遠慮下さい。

- Minumum Tennis を利用したことによるトラブル/損害が発生した場合、一切の責任を負いません。

- Minumum Tennis のコンテンツや利用規約は、予告なしに変更される場合があります。

- Minimum Tennis ではゲーム内のパラメータを変更できますが、研究の公平性や再現性を保つために、設定したパラメータの値を明示するようにしてください。

## Contact

意見や要望、質問などがありましたら、[こちら](https://open-video-game-library.github.io/info/contact/)からお問い合わせ下さい。

