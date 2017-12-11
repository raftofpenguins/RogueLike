using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null; // can access this class's variables and functions from any script in game
	public BoardManager boardScript; // instance of the Board Manager

	private int level = 3;



	// Use this for initialization -- Awake instead of Start
	void Awake () {

		// Destroys any other instances of this object, so only 1 exists at a time
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);

		boardScript = GetComponent<BoardManager>();
		InitGame();
	
	}

	void InitGame () {
		boardScript.SetupScene(level); // Tells boardScript which level to setup


	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
