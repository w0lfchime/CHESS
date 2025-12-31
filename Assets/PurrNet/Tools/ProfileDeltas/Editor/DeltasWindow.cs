using System;
using UnityEditor;
using UnityEngine;

namespace PurrNet.Profiler.Deltas.Editor
{
    public class DeltasWindow : EditorWindow
    {
        private const int PADDING = 30;
        private const int GRAPH_HEIGHT = 200;
        private const int POINT_SIZE = 4;
        private const int POINT_COUNT = DeltaProfiler.MAX_ITERATIONS;
        private readonly Color GRAPH_COLOR = new Color(0.2f, 0.2f, 0.2f);
        private readonly Color BASELINE_COLOR = new Color(0.0f, 0.8f, 0.0f, 1.0f);
        private readonly Color BASELINE_LINE_COLOR = new Color(0.0f, 0.8f, 0.0f, 0.5f);
        private readonly Color DELTA_COLOR = new Color(0.0f, 0.8f, 0.8f, 1.0f);
        private readonly Color DELTA_LINE_COLOR = new Color(0.0f, 0.8f, 0.8f, 0.5f);
        private readonly Color ZERO_LINE_COLOR = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        private readonly Color GRID_LINE_COLOR = new Color(0.3f, 0.3f, 0.3f, 0.3f);

        readonly double[] _baseLine = new double[POINT_COUNT];
        readonly double[] _delta = new double[POINT_COUNT];
        readonly string[] _heightsLabels = new string[POINT_COUNT];
        readonly Vector3[] _baseLinePositions = new Vector3[POINT_COUNT];
        readonly Vector3[] _deltaPositions = new Vector3[POINT_COUNT];
        private EvaluationMode _currentMode = EvaluationMode.Linear;

        static Type[] _deltaProfilers;
        private int _selectedProfilerIndex = 0;
        private string[] _profilerNames;
        DeltaProfiler _selected;

        void OnEnable()
        {
            NetworkManager.CalculateHashes();
            _deltaProfilers ??= DeltaProfilerUtils.GetAllDeltaProfilers();
            _profilerNames = Array.ConvertAll(_deltaProfilers, t => t.Name);
            CreateSelectedProfiler();
        }

        private void CreateSelectedProfiler()
        {
            if (_deltaProfilers is { Length: > 0 })
            {
                _selected = (DeltaProfiler)Activator.CreateInstance(_deltaProfilers[_selectedProfilerIndex]);
            }
        }

        private void OnGUI()
        {
            // Profiler selector
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Profiler Type:", GUILayout.Width(100));
            int newIndex = EditorGUILayout.Popup(_selectedProfilerIndex, _profilerNames);
            if (newIndex != _selectedProfilerIndex)
            {
                _selectedProfilerIndex = newIndex;
                CreateSelectedProfiler();
            }
            EditorGUILayout.EndHorizontal();

            // Mode selector
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Evaluation Mode:", GUILayout.Width(100));
            _currentMode = (EvaluationMode)EditorGUILayout.EnumPopup(_currentMode);
            EditorGUILayout.EndHorizontal();

            // Create graph background
            Rect graphRect = GUILayoutUtility.GetRect(0, GRAPH_HEIGHT);

            // add padding to the graph rect
            graphRect.x += PADDING;
            graphRect.y += PADDING;
            graphRect.width -= PADDING * 2;
            graphRect.height -= PADDING * 2;

            EditorGUI.DrawRect(graphRect, GRAPH_COLOR);

            double minHeight = double.MaxValue;
            double maxHeight = double.MinValue;

            for (int i = 0; i < POINT_COUNT; i++)
            {
                _baseLine[i] = _selected.GetSize(i, _currentMode);
                _delta[i] = _selected.GetPackedSize(Mathf.Max(i - 1, 0), i, _currentMode);
                _heightsLabels[i] = _selected.ToString(i, _currentMode);

                if (_baseLine[i] < minHeight) minHeight = _baseLine[i];
                if (_baseLine[i] > maxHeight) maxHeight = _baseLine[i];
                if (_delta[i] < minHeight) minHeight = _delta[i];
                if (_delta[i] > maxHeight) maxHeight = _delta[i];
            }

            if (minHeight > 0)
                minHeight = 0;

            if (maxHeight < 1)
                maxHeight = 1;

            // Draw grid lines
            float gridSpacing = graphRect.height / 4;
            for (int i = 1; i < 4; i++)
            {
                float y = graphRect.y + (i * gridSpacing);
                EditorGUI.DrawRect(new Rect(graphRect.x, y, graphRect.width, 1), GRID_LINE_COLOR);
            }

            // Draw zero line
            float zeroY = graphRect.y + graphRect.height * 0.5f;
            EditorGUI.DrawRect(new Rect(graphRect.x, zeroY, graphRect.width, 1), ZERO_LINE_COLOR);

            // Calculate positions for both curves
            for (int i = 0; i < POINT_COUNT; i++)
            {
                float normalizedBaseHeight = (float)((_baseLine[i] - minHeight) / (maxHeight - minHeight));
                float normalizedDeltaHeight = (float)((_delta[i] - minHeight) / (maxHeight - minHeight));
                float x = graphRect.x + (i * (graphRect.width / (POINT_COUNT - 1)));

                _baseLinePositions[i] = new Vector3(x, graphRect.y + graphRect.height - (normalizedBaseHeight * graphRect.height), -1);
                _deltaPositions[i] = new Vector3(x, graphRect.y + graphRect.height - (normalizedDeltaHeight * graphRect.height), -1);
            }

            // Draw baseline curve
            Handles.color = BASELINE_LINE_COLOR;
            for (int i = 0; i < POINT_COUNT - 1; i++)
                Handles.DrawLine(_baseLinePositions[i], _baseLinePositions[i + 1], 2);

            // Draw delta curve
            Handles.color = DELTA_LINE_COLOR;
            for (int i = 0; i < POINT_COUNT - 1; i++)
                Handles.DrawLine(_deltaPositions[i], _deltaPositions[i + 1], 2);

            // Draw points and value labels
            for (int i = 0; i < POINT_COUNT; i++)
            {
                // Draw baseline point and value
                Rect basePointRect = new Rect(_baseLinePositions[i].x - POINT_SIZE/2, _baseLinePositions[i].y - POINT_SIZE/2, POINT_SIZE, POINT_SIZE);
                EditorGUI.DrawRect(basePointRect, BASELINE_COLOR);
                GUI.Label(new Rect(_baseLinePositions[i].x - 20, _baseLinePositions[i].y - 20, 40, 20), $"{_baseLine[i]:F1}", new GUIStyle { normal = { textColor = BASELINE_COLOR } });

                // Draw delta point and value
                Rect deltaPointRect = new Rect(_deltaPositions[i].x - POINT_SIZE/2, _deltaPositions[i].y - POINT_SIZE/2, POINT_SIZE, POINT_SIZE);
                EditorGUI.DrawRect(deltaPointRect, DELTA_COLOR);
                GUI.Label(new Rect(_deltaPositions[i].x - 20, _deltaPositions[i].y - 20, 40, 20), $"{_delta[i]:F1}", new GUIStyle { normal = { textColor = DELTA_COLOR } });
            }

            // Draw height labels at the bottom
            for (int i = 0; i < POINT_COUNT; i++)
            {
                float x = graphRect.x + (i * (graphRect.width / (POINT_COUNT - 1)));
                float y = graphRect.y + graphRect.height + 5;
                GUI.Label(new Rect(x - 30, y, 60, 20), _heightsLabels[i], new GUIStyle {
                    normal = { textColor = Color.white },
                    alignment = TextAnchor.UpperCenter
                });
            }

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Baseline", new GUIStyle { normal = { textColor = BASELINE_COLOR } });
            GUILayout.Space(10);
            GUILayout.Label("Delta", new GUIStyle { normal = { textColor = DELTA_COLOR } });
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (Event.current.type == EventType.Repaint)
                Repaint();
        }
    }
}
