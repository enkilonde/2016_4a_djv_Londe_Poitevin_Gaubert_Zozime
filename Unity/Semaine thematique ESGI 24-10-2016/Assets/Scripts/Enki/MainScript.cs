using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using System;
using UnityEngine.UI;

public class MainScript : MonoBehaviour
{

	public GameObject logoGO;
	public GameObject menuGO;
	public GameObject ingameGO;
	public GameObject endgameGO;
	public Transform ballTransform;
	public Rigidbody ballRigidbody;
	public Button playGameButton;

	public GameObject avatar;

	void Start ()
	{

		var gameStateStream = Observable.Return("start")
			.Concat(Observable.Return("logo"))
			.Concat(Observable.Return("menu").Delay(TimeSpan.FromSeconds(2f)))
			.Concat(playGameButton.OnClickAsObservable().Take(1).Select(_ => "ingame"))
			.Concat(ballTransform.gameObject.OnBecameInvisibleAsObservable().Take(1).Select(_ => "endgame"))
			.Concat(Observable.Return("restartgame").Delay(TimeSpan.FromSeconds(2f)))
			.Repeat();



		var startGameStream = gameStateStream.Where(state => state == "start");
		var logoGameStream = gameStateStream.Where(state => state == "logo");
		var menuGameStream = gameStateStream.Where(state => state == "menu");
		var ingameGameStream = gameStateStream.Where(state => state == "ingame");
		var endgameGameStream = gameStateStream.Where(state => state == "endgame");
		var restardgameGameStream = gameStateStream.Where(state => state == "restardgame");



		var logoIsActiveStream = logoGameStream.Select(_ => true).Merge(menuGameStream.Select(_ => false));

		var menuIsActiveStream = menuGameStream.Select(_ => true)
			.Merge(ingameGameStream.Select(_ => false));
			//.Merge(Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Escape)).Select(_ => true));

		var ingameIsActiveStream = ingameGameStream.Select(_ => true).Merge(endgameGameStream.Select(_ => false));
		var endgameIsActiveStream = endgameGameStream.Select(_ => true).Merge(logoGameStream.Select(_ => false));


		logoIsActiveStream.Subscribe(logoGO.SetActive);
		menuIsActiveStream.Subscribe(menuGO.SetActive);
		ingameIsActiveStream.Subscribe(ingameGO.SetActive);
		endgameIsActiveStream.Subscribe(endgameGO.SetActive);

		ingameGameStream.Subscribe(_ => ballTransform.position = new Vector3(0, 0, 0));
		ingameGameStream.Subscribe(_ => ballRigidbody.velocity = new Vector3(0, 0, 0));


		var gameStateUpdateLoop = ingameGameStream.SelectMany(_ => Observable.EveryFixedUpdate()).TakeUntil(endgameGameStream).Repeat();

		gameStateUpdateLoop.Subscribe(_AppDomain => Debug.Log(UnityEngine.Random.Range(0, 2000)));


		var customeGameStateStream = gameStateUpdateLoop.Scan(new GameState(), (gameState, ticks) => GetNextState(gameState));

		//gameStateStream.Subscribe(Debug.Log);

		customeGameStateStream.Subscribe(ApplyNextState);



	}
	
	public struct GameState
	{
        public float posX;
		public Vector2 IAPose;
		public Vector2 playerPos;
	}

	public GameState GetNextState(GameState currentState)
	{
		return new GameState() { posX = currentState.posX + 0.02f };
	}

	public void ApplyNextState(GameState state)
	{

		avatar.transform.position = new Vector3(state.posX, avatar.transform.position.y, avatar.transform.position.z);
	}



}






























