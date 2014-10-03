//
// This file is part of the KSPPluginReload plugin for Kerbal Space Program, Copyright Joop Selen
// License: http://creativecommons.org/licenses/by-nc-sa/3.0/
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using KSP.IO;
using KSPPluginReload.Classess;
using KSPPluginReload.UI;
using UnityEngine;


[KSPAddon(KSPAddon.Startup.MainMenu, true)]
public class KSPPluginReloadModule : MonoBehaviour
{
    public KSPPluginReloadModule()
    {
        KSPPluginReload.Classe.Immortal.AddImmortal<KSPPluginReload.PluginReloadModule>();
    }

}

namespace KSPPluginReload
{
    public class PluginReloadModule : MonoBehaviour
    {
        public bool GUIActive;
        public static List<PluginClass> PluginClasses = new List<PluginClass>();
        public static List<PluginSetting> PluginSettings = new List<PluginSetting>();
        public static PluginReloadWindow PluginReloadWindow = new PluginReloadWindow()
        {
            ReloadCallback = LoadPlugins
        };

        public PluginReloadModule()
        {
            Debug.Log(String.Format("KSPPluginReloadModule loaded. Version: {0}.'", Assembly.GetExecutingAssembly().GetName().Version));

            LoadConfig();
            LoadPlugins();
            PluginReloadWindow.OpenWindow();
        }

        private static void LoadConfig()
        {
            try
            {
                ConfigNode settings = ConfigNode.Load(KSPUtil.ApplicationRootPath + "GameData/PluginReload/Settings.cfg");

                foreach (ConfigNode node in settings.GetNodes("PluginSetting"))
                {
                    PluginSetting pluginSetting = new PluginSetting()
                    {
                        Name = node.GetValue("name"),
                        Path = node.GetValue("path"),
                        LoadOnce = bool.Parse(node.GetValue("loadOnce")),
                        MethodsAllowedToFail = bool.Parse(node.GetValue("methodsAllowedToFail"))
                    };

                    PluginSettings.Add(pluginSetting);
                }
            }
            catch (Exception ex)
            {
                Debug.Log(string.Format("Failed to load settings.cfg. Error:\n{0}", ex));
                return;
            }
        }

        private static Assembly LoadAssembly(string location)
        {
            try
            {
                byte[] assemblyBytes = System.IO.File.ReadAllBytes(location);
                Assembly a = Assembly.Load(assemblyBytes);
                Debug.Log(string.Format("Reloaded assembly: {0} version: {1}.", a.GetName().Name, a.GetName().Version));
                return a;
            }
            catch (Exception ex)
            {
                Debug.Log(string.Format("Failed to load plugin from file {0}. Error:\n\n{1}", location, ex));
            }
            return null;
        }

        private static void LoadPlugins()
        {
            Debug.Log("(Re)loading plugins.");
            Type type = typeof(MonoBehaviour);
            foreach (PluginSetting setting in PluginSettings)
            {
                //Skipp reloading of loadonce assemblies
                if (setting.LoadOnce == true) continue;
                
                //Call ondestroy on alive classes
                List<PluginClass> toRemove = new List<PluginClass>();
                foreach (PluginClass pluginClass in PluginClasses.Where(pc => pc.PluginSetting == setting))
                {
                    if (pluginClass.Alive)
                    {
                        ExecuteMethod(pluginClass, "OnDestroy");
                    }
                    pluginClass.Instance = null;
                    toRemove.Add(pluginClass);
                }
                //Remove old class references
                foreach (PluginClass r in toRemove)
                {
                    PluginClasses.Remove(r);
                }

                setting.Assembly = LoadAssembly(setting.Path);

                //Remove assembly if reloading failed
                if (setting.Assembly == null)
                {
                    foreach (PluginClass pluginClass in PluginClasses.Where(pc => pc.PluginSetting == setting).ToList())
                    {
                        PluginClasses.Remove(pluginClass);
                    }
                    continue;
                }

                IList<Type> derivedClassess = (from t in setting.Assembly.GetTypes()
                                               where t.BaseType == (type) //&& t.GetConstructor(Type.EmptyTypes) != null
                                               select t).ToList();

                foreach (var derivedClass in derivedClassess)
                {
                    System.Attribute[] attrs = System.Attribute.GetCustomAttributes(derivedClass);

                    foreach (var att in attrs)
                    {
                        if (att is KSPAddon)
                        {
                            PluginClass pluginClass = PluginClasses.FirstOrDefault(pc => pc.Type == derivedClass);
                            //Only create a record for new entries
                            if (pluginClass == null)
                            {
                                pluginClass = new PluginClass();
                                pluginClass.Type = derivedClass;
                                pluginClass.KSPAddon = att as KSPAddon;
                                pluginClass.PluginSetting = setting;
                                PluginClasses.Add(pluginClass);
                            }
                            //Reset the instance
                            pluginClass.Instance = null;

                            if (pluginClass.KSPAddon.once == false || pluginClass.Fired == false)
                            {
                                bool awake = false;
                                switch(pluginClass.KSPAddon.startup)
                                {
                                    case KSPAddon.Startup.Instantly:
                                    case KSPAddon.Startup.EveryScene:
                                    //TODO: Check wether PSystem should even respawn.
                                    case KSPAddon.Startup.PSystemSpawn:
                                        awake = true;
                                    break;
                                    case KSPAddon.Startup.Credits:
                                        awake = (HighLogic.LoadedScene == GameScenes.CREDITS);
                                    break;
                                    case KSPAddon.Startup.EditorAny:
                                        awake = (HighLogic.LoadedScene == GameScenes.EDITOR || HighLogic.LoadedScene == GameScenes.SPH);
                                    break;
                                    case KSPAddon.Startup.EditorSPH:
                                        awake = (HighLogic.LoadedScene == GameScenes.SPH);
                                    break;
                                    case KSPAddon.Startup.EditorVAB:
                                        awake = (HighLogic.LoadedScene == GameScenes.EDITOR);
                                    break;
                                    case KSPAddon.Startup.Flight:
                                        awake = (HighLogic.LoadedScene == GameScenes.FLIGHT);
                                    break;
                                    case KSPAddon.Startup.MainMenu:
                                        awake = (HighLogic.LoadedScene == GameScenes.MAINMENU);
                                    break;
                                    case KSPAddon.Startup.Settings:
                                        awake = (HighLogic.LoadedScene == GameScenes.SETTINGS);
                                    break;
                                    case KSPAddon.Startup.SpaceCentre:
                                        awake = (HighLogic.LoadedScene == GameScenes.SPACECENTER);
                                    break;
                                    case KSPAddon.Startup.TrackingStation:
                                        awake = (HighLogic.LoadedScene == GameScenes.TRACKSTATION);
                                    break;
                                }

                                if (awake)
                                {
                                    //.Instance calls class constructor is class doesn't excist.
                                    ExecuteMethod(pluginClass, "Awake");
                                }
                            }

                        }
                    }
                }
            }

            //Reload gamedatabase and parts
            //GameDatabase.Instance.Recompile = true;
            //GameDatabase.Instance.StartLoad();
            //PartLoader.Instance.Recompile = true;
            //PartLoader.Instance.StartLoad();
            Debug.Log("Plugins (re)loaded.");
        }


        private static object ExecuteMethod(object instance, MethodInfo methodInfo, object[] parameters = null)
        {
            try
            {
                return methodInfo.Invoke(instance, parameters);
            }
            catch (Exception ex)
            {
                Debug.Log(string.Format("Failed to execute method {0}. Error:\n{1}", methodInfo.Name, ex));
            }
            return null;
        }

        internal static object ExecuteMethod(PluginClass pluginClass, string methodName, object[] parameters = null)
        {
            var methodInfo = pluginClass.Instance.GetType().GetMethod(methodName);
            if (methodInfo != null)
            {
                try
                {
                    return methodInfo.Invoke(pluginClass.Instance, parameters);
                }
                catch (Exception ex)
                {
                    if (pluginClass.PluginSetting.MethodsAllowedToFail == false)
                    {
                        PluginClasses.Remove(pluginClass);
                    }
                    Debug.Log(string.Format("Failed to execute method {0}. Error:\n{1}", methodInfo.Name, ex));
                }

            }
            return null;
        }

        internal static void ExecuteMethods(IEnumerable<PluginClass> pluginClasses, string methodName)
        {
            foreach (PluginClass p in pluginClasses)
            {
                ExecuteMethod(p, methodName);
            }
        }

        public void Update()
        {
            if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyDown(KeyCode.P))
            {
                if (PluginReloadWindow.Visible == false)
                    PluginReloadWindow.OpenWindow();
                else
                    PluginReloadWindow.CloseWindow();
                
            }
        }
    }
}
