using UnityEngine;
using System.Collections;

// abstract => incomplete class; must be implemented in derived class
public abstract class MovingObject : MonoBehaviour {

	public float moveTime = 0.1f;	// Determines how quickly the object moves
	public LayerMask blockingLayer;	// Determines if space can be moved into

	private BoxCollider2D boxCollider;
	private Rigidbody2D rb2D;			// Store component reference to RB2D component of unit moving
	private float inverseMoveTime;		// Make movement calculations more efficient

	// Use this for initialization
	protected virtual void Start () {					// PV functions can be overwritten by inheriting classes
		boxCollider = GetComponent<BoxCollider2D>();
		rb2D = GetComponent<Rigidbody2D>();
		inverseMoveTime = 1f / moveTime; 				// Store reciprocal so later we can multiply (more efficient than dividing)
	}

	// out allows arguments to be passed by reference; thus we return 2 types of info: bool returned, also hit

	protected bool Move (int xDir, int yDir, out RaycastHit2D hit) {
		Vector2 start = transform.position; // V3 -> V2 discards z-axis data
		Vector2 end = start + new Vector2 (xDir, yDir); // calculated based on direction parameters

		boxCollider.enabled = false;	// Disable so when we cast array, we don't hit our own collider
		hit = Physics2D.Linecast (start, end, blockingLayer); // Cast line from start to end and check for blocking layer
		boxCollider.enabled = true; 	// Re-enable collision

		// Check to see if anything was hit
		if (hit.transform == null) {
			// Open space
			StartCoroutine(SmoothMovement (end));
			return true; // We were able to move
		}

		// Why no else?

		return false; // Move was unsuccessful; occupied space with collision
	}

	// Smooth movement co-routine
	protected IEnumerator SmoothMovement (Vector3 end) {
		float sqrRemainingDistance = (transform.position - end).sqrMagnitude; 	// (Current position - end parameter) squared

		while (sqrRemainingDistance > float.Epsilon) // Essentially means sqr remaining dist > 0
		{
			// Vector3.MoveTowards moves a point in a straight line towards a target point
			// Target is newPosition, so we set newPosition = value returned by Vector3.MoveTowards
			// We move between current position, rb2D.position, and destination, end
			// New position is measured in units defined by inverseMoveTime * Time.deltaTime
			// Time.deltaTime is "The time in seconds it took to complete the last frame (Read Only)."

			Vector3 newPosition = Vector3.MoveTowards (rb2D.position, end, inverseMoveTime * Time.deltaTime);
			rb2D.MovePosition(newPosition); // Moves the object to this new position
			sqrRemainingDistance = (transform.position - end).sqrMagnitude; // Recalculate remaining sq dist
			yield return null; // Wait for frame before reevaluating condition of loop
		}
	}

	protected virtual void AttemptMove <T> (int xDir, int yDir) 
		where T : Component;
		// <T> => generic parameter used to specify type of component we expect unit to interact w/ if blocked
		// For enemies, this is the player; for the player, this is walls, so player can 
	{
		RaycastHit2D hit;
		bool canMove = Move (xDir, yDir, out hit); // Set to true if successful or false if not

		if (hit.transform == null)	// If nothing hit, return
			return;

		// If something was hit, get component reference to component of type T attached to hit object
		T hitComponent = hit.transform.GetComponent<T>();

		// If moving object is blocked and has hit something with which it can interact, call OnCantMove and pass it hitComponent
		if (!canMove && hitComponent != null)
			OnCantMove(hitComponent);

	}


	// Abstract => incomplete implementation; overwritten by functions in inhereiting classes; has no {}
	protected abstract void OnCantMove <T> (T component)
		where T : Component;

}
