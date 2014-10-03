//
// This file is part of the KSPPluginReload plugin for Kerbal Space Program, Copyright Joop Selen
// License: http://creativecommons.org/licenses/by-nc-sa/3.0/
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KSPPluginReload.Classess
{
    public class PluginClass
    {
        public Type Type;
        public KSPAddon KSPAddon;
        private object _Instance;
        public object Instance
        {
            get
            {
                //If requested and its not yet created create an instance of the class.
                if (_Instance == null)
                {
                    CreateInstance();
                }
                return _Instance;
            }
            set
            {
                _Instance = value;
            }
        }

        public bool Fired = false;
        public bool Alive = false;
        public PluginSetting PluginSetting;

        internal void CreateInstance()
        {
            Fired = true;
            Instance = Activator.CreateInstance(Type);
        }
    }
}
