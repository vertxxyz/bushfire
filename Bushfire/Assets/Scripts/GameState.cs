using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameState : MonoBehaviour {
	public enum GameStateType
	{
		Beginning,
		Start,
	}

	public GameStateType startGameState = GameStateType.Beginning;
	[ReadOnly]
	public GameStateType currentGameState;

	private void Start()
	{
		currentGameState = startGameState;
		BeginGameState();
	}

	public UnityEvent beginningEvent;
	public UnityEvent startEvent;
	
	private void BeginGameState()
	{
		switch (currentGameState)
		{
			case GameStateType.Beginning:
				beginningEvent.Invoke();
				break;
			case GameStateType.Start:
				startEvent.Invoke();
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	public void GoNextGameState()
	{
		switch (currentGameState)
		{
			case GameStateType.Beginning:
				currentGameState = GameStateType.Start;
				break;
			case GameStateType.Start:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		BeginGameState();
	}
}
