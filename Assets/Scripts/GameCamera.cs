using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour
{
	public int turn;

	[Header("References")]
	public Transform cameraPivot;   // Empty pivot object
	public Transform cam;           // The Camera itself (child of pivot)

	[Header("Movement Settings")]
	public float moveSpeed = 10f;
	[Tooltip("0 = instant, higher = smoother. Recommended 8–15 for snappy.")]
	public float movementSmoothFactor = 12f;

	[Header("Zoom Settings")]
	public float zoomSpeed = 10f;
	[Tooltip("0 = instant zoom, higher = smoother.")]
	public float zoomSmoothFactor = 12f;
	public float minZoomDistance;
	public float maxZoomDistance;

	[Header("Zoom Bounds (relative to initial setup)")]
	public float zoomHalfRange = 5f;
	public bool zoomHalfRangeIsPercent = false;

	[Header("Turn Animation Settings")]
	public float resetPositionDelay = .5f;
	public float resetPositionTime = 1f;

	// Dragging
	[Header("Dragging Settings")]
	public float dragSpeed = 0.02f; // lower = slower drag, tweak to taste
	private Vector3 lastMousePos;

	private Vector3 pivotTargetPos;
	private bool inputEnabled = true;

	private Vector3 defaultCameraLocalPos;

	private float initialZoomDistance;
	private float targetZoomDistance;
	private float currentZoomDistance;

	void Awake()
	{
		if (cameraPivot == null) cameraPivot = this.transform;
		if (cam == null) cam = Camera.main.transform;

		pivotTargetPos = cameraPivot.position;
		defaultCameraLocalPos = cam.localPosition;

		Vector3 axisBack = cam.localRotation * Vector3.back;
		initialZoomDistance = Mathf.Abs(Vector3.Dot(defaultCameraLocalPos, axisBack));

		//RecomputeZoomBounds();

		targetZoomDistance = maxZoomDistance;
		currentZoomDistance = maxZoomDistance;

		cam.localPosition = axisBack * currentZoomDistance;
	}

	void Update()
	{
		if (!inputEnabled) return;

		HandleMovement();
		HandleZoom();
		HandleDragging();
	}

	void HandleMovement()
	{
		float h = Input.GetAxisRaw("Horizontal");
		float v = Input.GetAxisRaw("Vertical");

		Vector3 moveDir = new Vector3(h, 0, v).normalized;
		moveDir = cameraPivot.TransformDirection(moveDir);

		pivotTargetPos += moveDir * moveSpeed * Time.deltaTime;

		cameraPivot.position = Vector3.Lerp(
			cameraPivot.position,
			pivotTargetPos,
			1f - Mathf.Exp(-movementSmoothFactor * Time.deltaTime)
		);
	}

	void HandleDragging()
	{
		if (Input.GetMouseButtonDown(1)) // right click pressed
		{
			lastMousePos = Input.mousePosition;
		}
		else if (Input.GetMouseButton(1)) // holding right click
		{
			Vector3 delta = Input.mousePosition - lastMousePos;
			lastMousePos = Input.mousePosition;

			// Convert screen delta into world movement relative to camera
			Vector3 move = new Vector3(-delta.x, 0, -delta.y) * dragSpeed;

			// Apply drag movement in cameraPivot's local space (so "up" is forward/back)
			move = cam.transform.TransformDirection(move);
			move.y = 0f; // lock to XZ plane

			pivotTargetPos += move;
		}
	}

	void HandleZoom()
	{
		Vector3 pos = new Vector3(GameManager.Instance.Cursor.hitPoint.x, 0, GameManager.Instance.Cursor.hitPoint.z);
		pivotTargetPos = pos * (1 - (currentZoomDistance / maxZoomDistance));
		
		float scroll = Input.GetAxis("Mouse ScrollWheel");
		if (Mathf.Abs(scroll) > 0.01f)
		{
			targetZoomDistance -= scroll * zoomSpeed;
			targetZoomDistance = Mathf.Clamp(targetZoomDistance, minZoomDistance, maxZoomDistance);
		}

		currentZoomDistance = Mathf.Lerp(
			currentZoomDistance,
			targetZoomDistance,
			1f - Mathf.Exp(-zoomSmoothFactor * Time.deltaTime)
		);

		Vector3 axisBack = cam.localRotation * Vector3.back;
		cam.localPosition = axisBack * currentZoomDistance;
	}

	private void SyncPivotTargetToCurrent()
	{
		pivotTargetPos = cameraPivot.position;
	}

	public void SetTurn(int turn)
	{
		this.turn = turn;
		StopAllCoroutines();
		StartCoroutine(SwitchTurnRoutine(turn));
	}

	IEnumerator SwitchTurnRoutine(int turn)
	{
		inputEnabled = false;

		Quaternion startRot = cameraPivot.rotation;
		Quaternion endRot = (GameManager.Instance.CurrentTurn == Team.White) ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180f, 0);

		Vector3 startPos = cameraPivot.position;
		Vector3 endPos = Vector3.zero;
		float t = 0f;

		yield return new WaitForSeconds(resetPositionDelay);

		targetZoomDistance = maxZoomDistance;

		while (t < 1f)
		{
			t += Time.deltaTime / resetPositionTime;
			float smooth = 1f - Mathf.Exp(-5f * t);
			cameraPivot.position = Vector3.Lerp(startPos, endPos, smooth);
			cameraPivot.rotation = Quaternion.Slerp(startRot, endRot, smooth);

			Vector3 axisBack = cam.localRotation * Vector3.back;
			cam.localPosition = Vector3.Lerp(axisBack * currentZoomDistance, axisBack * maxZoomDistance, smooth);
			yield return null;
		}

		currentZoomDistance = maxZoomDistance;

		cameraPivot.position = endPos;
		pivotTargetPos = endPos;
		cameraPivot.rotation = endRot;

		SyncPivotTargetToCurrent();

		inputEnabled = true;
	}

	public void ResetCamera()
	{
		pivotTargetPos = Vector3.zero;
		cameraPivot.position = Vector3.zero;
		cameraPivot.rotation = Quaternion.identity;

		//RecomputeZoomBounds();

		targetZoomDistance = initialZoomDistance;
		currentZoomDistance = initialZoomDistance;

		Vector3 axisBack = cam.localRotation * Vector3.back;
		cam.localPosition = axisBack * currentZoomDistance;
		cam.LookAt(cameraPivot);
	}

	private void RecomputeZoomBounds()
	{
		float halfRange = zoomHalfRangeIsPercent
			? Mathf.Max(0f, zoomHalfRange) * initialZoomDistance
			: Mathf.Max(0f, zoomHalfRange);

		minZoomDistance = Mathf.Max(0.01f, initialZoomDistance - halfRange);
		maxZoomDistance = initialZoomDistance + halfRange;
	}

	[ContextMenu("Recenter Zoom Bounds From Current Local Position")]
	private void RecenterFromCurrentCameraLocal()
	{
		defaultCameraLocalPos = cam.localPosition;
		Vector3 axisBack = cam.localRotation * Vector3.back;
		initialZoomDistance = Mathf.Abs(Vector3.Dot(defaultCameraLocalPos, axisBack));
		//RecomputeZoomBounds();
		targetZoomDistance = currentZoomDistance = initialZoomDistance;
	}
}
