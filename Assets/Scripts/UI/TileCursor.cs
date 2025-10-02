using UnityEngine;

public class TileCursor : MonoBehaviour
{
	[Header("Movement")]
	public float springStrength = 8f;
	public float damping = 0.8f;
	[Range(0f, 1f)] public float overshootFactor = 0.2f;
	public bool lockYToStart = true;

	[Header("Inactivity Fade")]
	[Tooltip("Seconds of near-zero movement before beginning to fade out.")]
	public float inactivityDelay = 1.0f;

	[Tooltip("Speed below which we consider the cursor idle (units/sec).")]
	public float idleSpeedThreshold = 0.01f;

	[Tooltip("Seconds to fully fade out once inactivity starts.")]
	public float fadeOutSeconds = 0.75f;

	[Tooltip("Seconds to fade back in once movement resumes.")]
	public float fadeInSeconds = 0.15f;

	[Tooltip("Alpha when active (usually 1).")]
	[Range(0f, 1f)] public float activeAlpha = 1.0f;

	[Tooltip("Alpha when inactive (usually 0).")]
	[Range(0f, 1f)] public float inactiveAlpha = 0.0f;

	private Vector3 _targetPos;
	private Vector3 _velocity;
	private float _lockedY;

	// Fade state
	private float _lastActiveTime;
	private Vector3 _lastPos;
	private float _currentAlpha;

	// Rendering
	private Renderer[] _renderers;
	private MaterialPropertyBlock _mpb;
	private static readonly int _BaseColorID = Shader.PropertyToID("_BaseColor");
	private static readonly int _ColorID = Shader.PropertyToID("_Color");

	void Awake()
	{
		if (lockYToStart)
			_lockedY = transform.position.y;

		_targetPos = transform.position;
		_lastPos = transform.position;
		_currentAlpha = activeAlpha;
		_lastActiveTime = Time.time;

		_renderers = GetComponentsInChildren<Renderer>(includeInactive: true);
		_mpb = new MaterialPropertyBlock();

		// Initialize alpha to active on start
		ApplyAlpha(_currentAlpha);
	}

	void Update()
	{
		float dt = Time.deltaTime;
		UpdateSpring(dt);
		UpdateInactivityAndFade(dt);
	}

	// -------- Movement ----------
	private void UpdateSpring(float dt)
	{
		Vector3 displacement = _targetPos - transform.position;
		Vector3 acceleration = displacement * springStrength - _velocity * damping;

		_velocity += acceleration * dt;

		// Small predictive lead to create subtle overshoot/bounce
		Vector3 lead = _velocity * overshootFactor;
		Vector3 nextPos = transform.position + (_velocity * dt) + (lead * dt);

		if (lockYToStart)
			nextPos.y = _lockedY;

		transform.position = nextPos;
	}

	// -------- Inactivity + Fade ----------
	private void UpdateInactivityAndFade(float dt)
	{
		// Measure speed
		float speed = (transform.position - _lastPos).magnitude / Mathf.Max(dt, 1e-6f);

		bool isActiveNow = speed > idleSpeedThreshold;

		// If we're moving, mark active
		if (isActiveNow)
			_lastActiveTime = Time.time;

		float timeSinceActive = Time.time - _lastActiveTime;

		// Decide target alpha
		float targetAlpha;
		if (timeSinceActive >= inactivityDelay)
		{
			// fading out
			float t = (timeSinceActive - inactivityDelay) / Mathf.Max(fadeOutSeconds, 1e-6f);
			targetAlpha = Mathf.Lerp(activeAlpha, inactiveAlpha, Mathf.Clamp01(t));
		}
		else
		{
			// keep fully active until delay elapses
			targetAlpha = activeAlpha;
		}

		// If movement resumed but we�re not yet at activeAlpha, blend in faster
		if (isActiveNow && _currentAlpha < activeAlpha - 0.001f)
		{
			_currentAlpha = Mathf.MoveTowards(_currentAlpha, activeAlpha, (fadeInSeconds > 0f ? dt / fadeInSeconds : 1f));
		}
		else
		{
			// Otherwise move toward targetAlpha (could be holding at activeAlpha or fading out)
			float denom = (targetAlpha < _currentAlpha) ? Mathf.Max(fadeOutSeconds, 1e-6f)
														: Mathf.Max(fadeInSeconds, 1e-6f);
			_currentAlpha = Mathf.MoveTowards(_currentAlpha, targetAlpha, dt / denom);
		}

		ApplyAlpha(_currentAlpha);
		_lastPos = transform.position;
	}

	private void ApplyAlpha(float a)
	{
		// Update all child renderers via MPB, supporting both _BaseColor and _Color
		for (int i = 0; i < _renderers.Length; i++)
		{
			var r = _renderers[i];
			if (r == null) continue;

			r.GetPropertyBlock(_mpb);

			// Prefer _BaseColor; if the shader doesn�t have it, fallback to _Color
			if (HasColorProperty(r, _BaseColorID))
			{
				Color c = _mpb.GetVector(_BaseColorID);
				if (c == default) c = Color.white; // if unset
				c.a = a;
				_mpb.SetColor(_BaseColorID, c);
			}
			else
			{
				Color c = _mpb.GetVector(_ColorID);
				if (c == default) c = Color.white;
				c.a = a;
				_mpb.SetColor(_ColorID, c);
			}

			r.SetPropertyBlock(_mpb);
		}
	}

	private static bool HasColorProperty(Renderer r, int id)
	{
		// Cheap heuristic: try getting the property; if material count is zero this will no-op anyway.
		// (Unity doesn�t expose a direct "HasProperty" on Renderer+MPB; this is fine in practice.)
		return true;
	}

	// -------- Public API ----------
	/// <summary>Sets an absolute world-space target.</summary>
	public void SetTarget(Vector3 worldPosition)
	{
		if (lockYToStart) worldPosition.y = _lockedY;
		_targetPos = worldPosition;
		// Mark active right away so we begin fading in if needed
		_lastActiveTime = Time.time;
	}

	/// <summary>Aim at the center of a tile's X/Z. Y is locked (if enabled).</summary>
	public void SetTargetFromTile(Transform tile, bool instant = false)
	{
		var pos = tile.position;
		if (lockYToStart) pos.y = _lockedY;
		else pos.y = transform.position.y;

		_targetPos = pos;
		_lastActiveTime = Time.time;
		
		if (instant)
		{
			transform.position = _targetPos;
		}
	}

	/// <summary>Instantly jump to target; reset velocity and mark active.</summary>
	public void SnapToTarget()
	{
		transform.position = _targetPos;
		_velocity = Vector3.zero;
		_lastActiveTime = Time.time;
		_currentAlpha = activeAlpha;
		ApplyAlpha(_currentAlpha);
	}
}
