using System;
using System.Globalization;
using MelonLoader;
using UnityEngine;
using UnityEngine.AI;

[assembly: MelonInfo(typeof(SchoolChange.SchoolChangeMod), "SchoolChange", "1.0.0", "Op")]
[assembly: MelonGame(null, null)]

namespace SchoolChange
{
    public class SchoolChangeMod : MelonMod
    {
        private const KeyCode ToggleKey = KeyCode.F2;

        private bool _open;
        private int  _tab;

        private Rect _mainRect   = new Rect(120f, 120f, 260f, 200f);
        private Rect _playerRect = new Rect(400f, 120f, 320f, 170f);
        private Rect _momRect    = new Rect(400f, 310f, 320f, 240f);
        private Rect _dadRect    = new Rect(400f, 570f, 320f, 240f);

        private NavMeshAgent        _momAgent;
        private NavMeshAgent        _dadAgent;
        private CharacterController _playerCC;

        private string _momSpeed   = "0";
        private string _momAccel   = "0";
        private string _momAngular = "0";

        private string _dadSpeed   = "0";
        private string _dadAccel   = "0";
        private string _dadAngular = "0";

        private string _playerHeight = "0";

        public override void OnInitializeMelon()
        {
            LoggerInstance.Msg("SchoolChange loaded. F2 to toggle.");
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(ToggleKey)) _open = !_open;
        }

        public override void OnGUI()
        {
            if (!_open) return;

            _mainRect = GUI.Window(0x5C01, _mainRect, DrawMainWindow, "SchoolChange — Cheat Menu");

            if (_tab == 1) _playerRect = GUI.Window(0x5C02, _playerRect, DrawPlayerTab, "Player Manager");
            if (_tab == 2) _momRect    = GUI.Window(0x5C03, _momRect,    DrawMomTab,    "Mom Manager");
            if (_tab == 3) _dadRect    = GUI.Window(0x5C04, _dadRect,    DrawDadTab,    "Dad Manager");
        }

        private void DrawMainWindow(int id)
        {
            GUILayout.Space(6f);
            GUILayout.Label("Select target:");

            if (GUILayout.Button("Player Manager", GUILayout.Height(32f)))
            {
                EnsureTargets();
                SyncPlayerBuffers();
                _tab = 1;
            }
            if (GUILayout.Button("Mom Manager", GUILayout.Height(32f)))
            {
                EnsureTargets();
                SyncMomBuffers();
                _tab = 2;
            }
            if (GUILayout.Button("Dad Manager", GUILayout.Height(32f)))
            {
                EnsureTargets();
                SyncDadBuffers();
                _tab = 3;
            }

            GUILayout.Space(4f);
            if (GUILayout.Button("Refresh targets"))
                EnsureTargets();

            GUI.DragWindow(new Rect(0f, 0f, 10000f, 20f));
        }

        private void DrawPlayerTab(int id)
        {
            GUILayout.Space(6f);
            if (_playerCC == null) EnsureTargets();

            if (_playerCC == null)
            {
                GUILayout.Label("FP_Controller not found.");
                if (GUILayout.Button("Back")) _tab = 0;
                GUI.DragWindow(new Rect(0f, 0f, 10000f, 20f));
                return;
            }

            GUILayout.Label($"Current height: {_playerCC.height:F2}");
            GUILayout.Space(4f);
            GUILayout.Label("Height:");
            _playerHeight = GUILayout.TextField(_playerHeight, GUILayout.Height(22f));

            if (GUILayout.Button("Apply"))
            {
                if (float.TryParse(_playerHeight, NumberStyles.Float, CultureInfo.InvariantCulture, out float v))
                    _playerCC.height = v;
            }

            GUILayout.Space(8f);
            if (GUILayout.Button("Back")) _tab = 0;

            GUI.DragWindow(new Rect(0f, 0f, 10000f, 20f));
        }

        private void DrawMomTab(int id)
        {
            GUILayout.Space(6f);
            if (_momAgent == null) EnsureTargets();

            if (_momAgent == null)
            {
                GUILayout.Label("Mom / NavMeshAgent not found.");
                if (GUILayout.Button("Back")) _tab = 0;
                GUI.DragWindow(new Rect(0f, 0f, 10000f, 20f));
                return;
            }

            GUILayout.Label($"Current — spd {_momAgent.speed:F2} | acc {_momAgent.acceleration:F2} | ang {_momAgent.angularSpeed:F2}");
            GUILayout.Space(4f);

            GUILayout.Label("Speed:");
            _momSpeed = GUILayout.TextField(_momSpeed, GUILayout.Height(22f));

            GUILayout.Label("Acceleration:");
            _momAccel = GUILayout.TextField(_momAccel, GUILayout.Height(22f));

            GUILayout.Label("Angular Speed:");
            _momAngular = GUILayout.TextField(_momAngular, GUILayout.Height(22f));

            GUILayout.Space(4f);
            if (GUILayout.Button("Apply"))
            {
                if (float.TryParse(_momSpeed,   NumberStyles.Float, CultureInfo.InvariantCulture, out float s))  _momAgent.speed        = s;
                if (float.TryParse(_momAccel,   NumberStyles.Float, CultureInfo.InvariantCulture, out float a))  _momAgent.acceleration = a;
                if (float.TryParse(_momAngular, NumberStyles.Float, CultureInfo.InvariantCulture, out float an)) _momAgent.angularSpeed = an;
            }

            GUILayout.Space(8f);
            if (GUILayout.Button("Back")) _tab = 0;

            GUI.DragWindow(new Rect(0f, 0f, 10000f, 20f));
        }

        private void DrawDadTab(int id)
        {
            GUILayout.Space(6f);
            if (_dadAgent == null) EnsureTargets();

            if (_dadAgent == null)
            {
                GUILayout.Label("Dad / NavMeshAgent not found.");
                if (GUILayout.Button("Back")) _tab = 0;
                GUI.DragWindow(new Rect(0f, 0f, 10000f, 20f));
                return;
            }

            GUILayout.Label($"Current — spd {_dadAgent.speed:F2} | acc {_dadAgent.acceleration:F2} | ang {_dadAgent.angularSpeed:F2}");
            GUILayout.Space(4f);

            GUILayout.Label("Speed:");
            _dadSpeed = GUILayout.TextField(_dadSpeed, GUILayout.Height(22f));

            GUILayout.Label("Acceleration:");
            _dadAccel = GUILayout.TextField(_dadAccel, GUILayout.Height(22f));

            GUILayout.Label("Angular Speed:");
            _dadAngular = GUILayout.TextField(_dadAngular, GUILayout.Height(22f));

            GUILayout.Space(4f);
            if (GUILayout.Button("Apply"))
            {
                if (float.TryParse(_dadSpeed,   NumberStyles.Float, CultureInfo.InvariantCulture, out float s))  _dadAgent.speed        = s;
                if (float.TryParse(_dadAccel,   NumberStyles.Float, CultureInfo.InvariantCulture, out float a))  _dadAgent.acceleration = a;
                if (float.TryParse(_dadAngular, NumberStyles.Float, CultureInfo.InvariantCulture, out float an)) _dadAgent.angularSpeed = an;
            }

            GUILayout.Space(8f);
            if (GUILayout.Button("Back")) _tab = 0;

            GUI.DragWindow(new Rect(0f, 0f, 10000f, 20f));
        }

        private void SyncMomBuffers()
        {
            if (_momAgent == null) return;
            _momSpeed   = _momAgent.speed.ToString(CultureInfo.InvariantCulture);
            _momAccel   = _momAgent.acceleration.ToString(CultureInfo.InvariantCulture);
            _momAngular = _momAgent.angularSpeed.ToString(CultureInfo.InvariantCulture);
        }

        private void SyncDadBuffers()
        {
            if (_dadAgent == null) return;
            _dadSpeed   = _dadAgent.speed.ToString(CultureInfo.InvariantCulture);
            _dadAccel   = _dadAgent.acceleration.ToString(CultureInfo.InvariantCulture);
            _dadAngular = _dadAgent.angularSpeed.ToString(CultureInfo.InvariantCulture);
        }

        private void SyncPlayerBuffers()
        {
            if (_playerCC == null) return;
            _playerHeight = _playerCC.height.ToString(CultureInfo.InvariantCulture);
        }

        private void EnsureTargets()
        {
            _momAgent = null;
            _dadAgent = null;
            _playerCC = null;

            foreach (var agent in UnityEngine.Object.FindObjectsOfType<NavMeshAgent>())
            {
                string n = agent.gameObject.name.ToLowerInvariant();
                if (n.Contains("mom")) _momAgent = agent;
                else if (n.Contains("dad")) _dadAgent = agent;
            }

            _playerCC = UnityEngine.Object.FindObjectOfType<CharacterController>();
        }
    }
}
