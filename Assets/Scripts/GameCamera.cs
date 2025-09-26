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

	[Header("Zoom Bounds (relative to initial setup)")]
	[Tooltip("Half the total zoom span around the initial camera distance.\nIf 'Use Percent' is OFF, units are world units along camera local -Z.\nIf ON, value is a fraction of the initial distance (e.g., 0.5 = ±50%).")]
	public float zoomHalfRange = 5f;
	public bool zoomHalfRangeIsPercent = false;

	[Header("Turn Animation Settings")]
	public float centerDuration = 0.5f;
	public float rotateDuration = 1f;

	private Vector3 pivotTargetPos;
	private bool inputEnabled = true;

	// Initial/editor setup capture
	private Vector3 defaultCameraLocalPos;

	// Zoom handling (distances are along the camera's local -Z axis)
	private float initialZoomDistance;  // midpoint defined by editor setup
	private float minZoomDistance;
	private float maxZoomDistance;
	private float targetZoomDistance;
	private float currentZoomDistance;

	void Start()
	{
		if (cameraPivot == null) cameraPivot = this.transform;
		if (cam == null) cam = Camera.main.transform;

		pivotTargetPos = cameraPivot.position;
		defaultCameraLocalPos = cam.localPosition;

		// Compute zoom axis from current local rotation
		// and project the editor-set local position onto that axis
		Vector3 axisBack = cam.localRotation * Vector3.back;
		initialZoomDistance = Mathf.Abs(Vector3.Dot(defaultCameraLocalPos, axisBack));

		RecomputeZoomBounds();

		targetZoomDistance = initialZoomDistance;
		currentZoomDistance = initialZoomDistance;

		// Snap camera to the computed axis at the editor-defined midpoint
		cam.localPosition = axisBack * currentZoomDistance;
	}

	void Update()
	{
		if (!inputEnabled) return;

		HandleMovement();
		HandleZoom();
	}

	void HandleMovement()
	{
		float h = Input.GetAxisRaw("Horizontal"); // A/D
		float v = Input.GetAxisRaw("Vertical");   // W/S

		// Movement relative to the pivot's facing direction
		Vector3 moveDir = new Vector3(h, 0, v).normalized;
		moveDir = cameraPivot.TransformDirection(moveDir);

		pivotTargetPos += moveDir * moveSpeed * Time.deltaTime;

		// Exponential smoothing (fast response, still eased)
		cameraPivot.position = Vector3.Lerp(
			cameraPivot.position,
			pivotTargetPos,
			1f - Mathf.Exp(-movementSmoothFactor * Time.deltaTime)
		);
	}

	void HandleZoom()
	{
		float scroll = Input.GetAxis("Mouse ScrollWheel");
		if (Mathf.Abs(scroll) > 0.01f)
		{
			targetZoomDistance -= scroll * zoomSpeed; // scroll up = zoom in
			targetZoomDistance = Mathf.Clamp(targetZoomDistance, minZoomDistance, maxZoomDistance);
		}

		// Smooth toward target
		currentZoomDistance = Mathf.Lerp(
			currentZoomDistance,
			targetZoomDistance,
			1f - Mathf.Exp(-zoomSmoothFactor * Time.deltaTime)
		);

		// Place camera along its local -Z axis at the smoothed distance
		Vector3 axisBack = cam.localRotation * Vector3.back;
		cam.localPosition = axisBack * currentZoomDistance;
	}

	// Add this helper anywhere in the class
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

		// --- Step 1: recenter pivot to origin ---
		Vector3 startPos = cameraPivot.position;
		Vector3 endPos = Vector3.zero;
		float t = 0f;

		while (t < 1f)
		{
			t += Time.deltaTime / centerDuration;
			cameraPivot.position = Vector3.Lerp(startPos, endPos, t);
			yield return null;
		}

		// Snap to ensure no residue from frame time
		cameraPivot.position = endPos;
		// CRITICAL: keep the movement target in sync or it will pull you away
		pivotTargetPos = endPos;

		yield return new WaitForSeconds(0.5f);

		// --- Step 2: rotate 180° ---
		Quaternion startRot = cameraPivot.rotation;
		Quaternion endRot = startRot * Quaternion.Euler(0, 180f, 0);
		t = 0f;

		while (t < 1f)
		{
			t += Time.deltaTime / rotateDuration;
			cameraPivot.rotation = Quaternion.Slerp(startRot, endRot, t);
			yield return null;
		}

		// Hard snap + resync target to the new final pose (belt-and-suspenders)
		cameraPivot.rotation = endRot;
		SyncPivotTargetToCurrent();

		inputEnabled = true;
	}

	public void ResetCamera()
	{
		// Reset pivot
		pivotTargetPos = Vector3.zero;
		cameraPivot.position = Vector3.zero;
		cameraPivot.rotation = Quaternion.identity;

		// Recompute bounds from the original editor setup
		RecomputeZoomBounds();

		targetZoomDistance = initialZoomDistance;
		currentZoomDistance = initialZoomDistance;

		// Place camera on axis at midpoint and face pivot
		Vector3 axisBack = cam.localRotation * Vector3.back;
		cam.localPosition = axisBack * currentZoomDistance;
		cam.LookAt(cameraPivot);
	}

	/// <summary>
	/// Recompute min/max zoom distances symmetric around the editor-defined midpoint.
	/// </summary>
	private void RecomputeZoomBounds()
	{
		float halfRange = zoomHalfRangeIsPercent
			? Mathf.Max(0f, zoomHalfRange) * initialZoomDistance   // treat as percentage
			: Mathf.Max(0f, zoomHalfRange);                        // treat as units

		minZoomDistance = Mathf.Max(0.01f, initialZoomDistance - halfRange);
		maxZoomDistance = initialZoomDistance + halfRange;
	}

	// Optional: call this if you manually move the camera in play mode and want new bounds from that placement.
	[ContextMenu("Recenter Zoom Bounds From Current Local Position")]
	private void RecenterFromCurrentCameraLocal()
	{
		defaultCameraLocalPos = cam.localPosition;
		Vector3 axisBack = cam.localRotation * Vector3.back;
		initialZoomDistance = Mathf.Abs(Vector3.Dot(defaultCameraLocalPos, axisBack));
		RecomputeZoomBounds();
		targetZoomDistance = currentZoomDistance = initialZoomDistance;
	}
}
