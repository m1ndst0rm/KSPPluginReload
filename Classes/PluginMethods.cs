//
// This file is part of the KSPPluginReload plugin for Kerbal Space Program, Copyright Joop Selen
// License: http://creativecommons.org/licenses/by-nc-sa/3.0/
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KSPPluginReload.Classess;
using UnityEngine;

namespace KSPPluginReload.Classess
{
    public class PluginMethod : MonoBehaviour
    {
        public KSPAddon.Startup Startup;

        public IEnumerable<PluginClass> PluginClasses
        {
            get
            {
                return PluginReloadModule.PluginClasses.Where(pc => pc.KSPAddon.startup == Startup);
            }
        }

        public void OnDestroy()
        {
            foreach (PluginClass p in this.PluginClasses)
            {
                p.Alive = false;
            }
            ExecuteMethod("OnDestroy");
        }

        public void OnDisable()
        {
            ExecuteMethod("OnDisable");
        }

        public void OnGUI()
        {
            ExecuteMethod("OnGUI");
        }

        public void Update()
        {
            ExecuteMethod("Update");
        }

        public void Awake()
        {
            foreach (PluginClass p in this.PluginClasses)
            {
                p.Alive = true;
            }
            ExecuteMethod("Awake");
        }

        private void ExecuteMethod(string methodName, bool log = false)
        {
            if (PluginClasses.Count() > 0)
            {
                if (log == true)
                    Debug.Log(string.Format("PluginReload: StartupType {0}, executing method {2} {1} times.", Startup, PluginClasses.Count(), methodName));
                PluginReloadModule.ExecuteMethods(this.PluginClasses, methodName);
            }
        }
    }

    [KSPAddon(KSPAddon.Startup.Credits, false)]
    public class Credits : PluginMethod
    {
        public Credits()
        {
            Startup = KSPAddon.Startup.Credits;
        }
    }

    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class EditorAny : PluginMethod
    {
        public EditorAny()
        {
            Startup = KSPAddon.Startup.EditorAny;
        }
    }

    [KSPAddon(KSPAddon.Startup.EditorSPH, false)]
    public class EditorSPH : PluginMethod
    {
        public EditorSPH()
        {
            Startup = KSPAddon.Startup.EditorSPH;
        }
    }

    [KSPAddon(KSPAddon.Startup.EditorVAB, false)]
    public class EditorVAB : PluginMethod
    {
        public EditorVAB()
        {
            Startup = KSPAddon.Startup.EditorVAB;
        }
    }

    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class EveryScene : PluginMethod
    {
        public EveryScene()
        {
            Startup = KSPAddon.Startup.EveryScene;
        }
    }

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class Flight : PluginMethod
    {
        public Flight()
        {
            Startup = KSPAddon.Startup.Flight;
        }
    }

    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class MainMenu : PluginMethod
    {
        public MainMenu()
        {
            Startup = KSPAddon.Startup.MainMenu;
        }
    }

    [KSPAddon(KSPAddon.Startup.Settings, false)]
    public class PSystemSpawn : PluginMethod
    {
        public PSystemSpawn()
        {
            Startup = KSPAddon.Startup.PSystemSpawn;
        }
    }

    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class SpaceCentre : PluginMethod
    {
        public SpaceCentre()
        {
            Startup = KSPAddon.Startup.SpaceCentre;
        }
    }

    [KSPAddon(KSPAddon.Startup.TrackingStation, false)]
    public class TrackingStation : PluginMethod
    {
        public TrackingStation()
        {
            Startup = KSPAddon.Startup.TrackingStation;
        }
    }
}
