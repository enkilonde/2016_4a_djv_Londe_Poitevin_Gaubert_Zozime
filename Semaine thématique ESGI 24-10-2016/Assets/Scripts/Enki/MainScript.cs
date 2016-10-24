using UnityEngine;
using System.Collections;
using UniRx;
using System;

public class MainScript : MonoBehaviour
{

	public GameObject logoGO;
	public GameObject menuGO;


	void Start ()
	{

		var gameStateStream = new[] { "start", "logo", "menu", "ingame", "endgame", "restardgame" }
		.ToObservable()
		.Zip(Observable.Timer(TimeSpan.FromSeconds(0f), TimeSpan.FromSeconds(2f)), (gameStateEvent, _var) => gameStateEvent).Repeat();



		var startGameStream = gameStateStream.Where(state => state == "start");
		var logoGameStream = gameStateStream.Where(state => state == "logo");
		var menuGameStream = gameStateStream.Where(state => state == "menu");
		var ingameGameStream = gameStateStream.Where(state => state == "ingame");
		var endgameGameStream = gameStateStream.Where(state => state == "endgame");
		var restardgameGameStream = gameStateStream.Where(state => state == "restardgame");



		var logoIsActiveStream = logoGameStream.Select(_ => true).Merge(menuGameStream.Select(_ => false));

		var menuIsActiveStream = menuGameStream.Select(_ => true).Merge(ingameGameStream.Select(_ => false));


		logoIsActiveStream.Subscribe(logoGO.SetActive);
		menuIsActiveStream.Subscribe(menuGO.SetActive);

		gameStateStream.Subscribe(Debug.Log);



	}
	



}






























