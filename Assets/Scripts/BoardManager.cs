using UnityEngine;
using System; // Added to use Serializable attribute
using System.Collections.Generic; // Added .Generic to use Lists
using Random = UnityEngine.Random; // Specify because there is a Random in both UnityEngine and System

public class NewBehaviourScript : MonoBehaviour {

	[Serializable]
	public class Count {
		public int minimum;
		public int maximum;

		public Count (int min, int max) {
			minimum = min;
			maximum = max;
		}
	}

	// Defines size of map
	public int columns = 8;
	public int rows = 8;

	// Creates range of certain tiles to populate room
	public Count wallCount = new Count (5,9); 
	public Count foodCount = new Count (1,5);

	// Variables to hold Prefabs spawned
	public GameObject exit; // only 1 exit object

	// Arrays so we choose a random tile from list of options
	public GameObject[] floorTiles; 
	public GameObject[] wallTiles;
	public GameObject[] foodTiles;
	public GameObject[] enemyTiles;
	public GameObject[] outerWallTiles;

	// Helps keep child objects organized; collapse in Hierarchy
	private Transform boardHolder;

	// Tracks possible positions on map and detects whether an object spawned in that spot
	private List <Vector3> gridPositions = new List<Vector3>();

	void InitializeList() {
		gridPositions.Clear(); // Clears list of positions to start fresh

		// Create list of possible positions for enemies, walls, items
		// Iterates through 1 less than map size to leave open perimeter
		for (int x = 1; x < columns - 1; x++) {
			for (int y = 1; y < rows - 1; y++) {
				gridPositions.Add(new Vector3(x,y,0f));
			}		
		}
	}

	// Setup outer wall and floor of game board
	void BoardSetup() {
		boardHolder = new GameObject ("Board").transform;

		// Iterates through coordinates inaccessible to the player, so that it creates outer perimeter
		for (int x = -1; x < columns + 1; x++) {
			for (int y = -1; y < rows + 1; y++) {
				// Declaring variable of type GameObject and setting it equal to index in array of floorTiles 
				// Chosen randomly between 0 and length of array, so length doesn't have to be pre-specified
				GameObject toInstantiate = floorTiles[Random.Range (0, floorTiles.Length)];

				// If in outer wall area, use an outer wall tile
				if (x == -1 || x == columns || y == -1 || y == rows) {
					// Que up a random outer wall tile to be instantiated in this spot from the list of available outer wall tiles
					toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
				}

				// GameObject variable assigned to object instantiating, @ new V3 with our x,y coords, 0f z-coord, Quat => no rotation
				GameObject instance = Instantiate(toInstantiate, new Vector3 (x,y,0f), Quaternion.identity) as GameObject;

				// Set boardHolder as parent object to help keep Hierarchy organized
				instance.transform.SetParent(boardHolder);
			}		
		}
	}

	// Generates random position within bounds that isn't already being used by another object
	Vector3 RandomPosition() {
		// Generates random number between 0 and positions stored in gridPositions list
		int randomIndex = Random.Range(0, gridPositions.Count);

		// Set equal to the grid position stored in list at random index
		Vector3 randomPosition = gridPositions[randomIndex];

		// To make sure we don't spawn 2 objects at same location
		gridPositions.RemoveAt(randomIndex);

		// Can use this position to spawn object in random location
		return randomPosition;
	}

	// Generates random object at a random position
	void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum) {
		// Controls how many of a given object we're going to spawn in a given level
		int objectCount = Random.Range(minimum, maximum + 1);

		for (int i = 0; i < objectCount; i++){

			// Calls RandomPosition function to get the position of this object
			Vector3 randomPosition = RandomPosition();

			// Choose random tile from array of game objects from tileArray
			GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];

			// Instantiate object at random position
			Instantiate(tileChoice, randomPosition, Quaternion.identity);
		}
	}

	// Only public function => this is the function being used by GameManager
	public void SetupScene(int level) {
		BoardSetup();
		InitializeList();

		// Pass in arrays for food and wall tiles, generated at random
		LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
		LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);

		// Create logarithmic increase in difficulty; 1 enemy at lvl 2, 2 at 4, 3 at 8, etc. ; Mathf.log() returns float, so we cast as int
		int enemyCount = (int)Mathf.log(level,2f);

		// Place enemies at random; min and max are the same, so these are arguments for each parameter
		LayoutObjectAtRandom(enemyCount, enemyCount, enemyCount);

		// Place exit in same location -- top right corner of walkable perimeter
		Instantiate(exit, new Vector3(columns - 1, rows - 1, 0F), Quaternion.identity);

	}

	/* Delete these functions
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	*/
}
