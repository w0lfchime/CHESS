using UnityEngine;
using System; // for Action

public class CursorManager : MonoBehaviour
{
	[Header("Settings")]
	public LayerMask tileLayer;
	public float maxRayDistance = 100f;

	[Header("Cursor Visuals")]
	public Transform cursorRoot;     // Empty parent, world aligned
	public Renderer cursorRenderer;  // Renderer on the mesh child

	[Header("Movement")]
	public float springStrength = 8f;
	public float damping = 0.8f;
	public float overshootFactor = 0.2f; // How "bouncy" the overshoot is

	[Header("Fade")]
	public float fadeOutSpeed = 1f;   // Seconds to fully fade out
	public float fadeInSpeed = 6f;    // Faster fade in

	// --- Callback slot ---
	public Action<Transform> onNextClick;

	private Transform currentTile;
	private Vector3 targetPos;
	private Vector3 velocity;

	private MaterialPropertyBlock mpb;
	private float currentAlpha = 1f;

	void Awake()
	{
		if (cursorRenderer != null)
			mpb = new MaterialPropertyBlock();
	}

	void Update()
	{
		CheckTileUnderCursor();
		UpdateCursorMovement();
		UpdateCursorFade();

		CheckClickInvoke();
	}

	void CheckTileUnderCursor()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, tileLayer))
		{
			Transform hitTile = hit.transform;

			// Step up to parent empty if child mesh is hit
			if (hitTile.parent != null && hitTile.parent.CompareTag("Tile"))
				hitTile = hitTile.parent;

			if (hitTile != currentTile)
			{
				currentTile = hitTile;
				targetPos = new Vector3(currentTile.position.x, cursorRoot.position.y, currentTile.position.z);
			}
		}
		else
		{
			currentTile = null;
		}
	}

	void UpdateCursorMovement()
	{
		if (currentTile != null)
			targetPos = new Vector3(currentTile.position.x, cursorRoot.position.y, currentTile.position.z);

		float dt = Time.deltaTime;

		Vector3 displacement = targetPos - cursorRoot.position;
		Vector3 acceleration = displacement * springStrength - velocity * damping;

		velocity += acceleration * dt;
		cursorRoot.position += velocity * dt;
	}

	void UpdateCursorFade()
	{
		if (cursorRenderer == null) return;

		float targetAlpha = currentTile ? 1f : 0f;
		float speed = (targetAlpha > currentAlpha) ? fadeInSpeed : fadeOutSpeed;

		currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, Time.deltaTime * speed);

		cursorRenderer.GetPropertyBlock(mpb);
		mpb.SetColor("_Color", new Color(1f, 1f, 1f, currentAlpha));
		cursorRenderer.SetPropertyBlock(mpb);
	}

	void CheckClickInvoke()
	{
		if (onNextClick != null && Input.GetMouseButtonDown(0))
		{
			if (currentTile != null)
			{
				onNextClick.Invoke(currentTile);
			}
			// Clear the slot no matter what
			onNextClick = null;
		}
	}
}
