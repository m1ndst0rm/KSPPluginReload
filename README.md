KSPPluginReload
================================
Author: m1nd0 (joopselen@gmail.com)

Description:
Plugin which will allow DLLs to be reloading into KSP without the need for a reboot.


Notice:
* Only classes which inherit MonoBehaviour and include the attribute KSPAddon will be reloaded. Parts & partmodules will not be reloaded.
* Custom parts and partmodules won't work when loading a DLL with this (see note1)
* Only use this for developing/debugging. Since KSP doesn't allow AppDomains each reload loads a second instance of the plugin, therefore increasing the memory usage.

Installation
------------
1. download latest build (http://tawot.com/KSPPluginReload.zip) or build from source and extract to ksp directory.
2. remove your plugin.dll from \GameData\YourPlugins\plugins\ (see note1 for part & partmodule code)
3. edit Settings.cfg in "\GameData\PluginReload". 
4. open AssemblyInfo.cs in your project and change AssemblyVersion & AssemblyFileVersion to "1.0.0.*". Visual Studio will auto-increment your build, this is the only way to allow reloading of a DLL.

Usage
------
Launch your game. Test something. Rebuild the dll, press Reload plugins to reload plugins. 
ALT+P opens/closes the rebuild menu.

Settings example
**Settings.cfg**:

	PluginSetting
	{
		name = Myplugginname
		path = C:\develop\myplugin\debug\myplugin.dll
		loadOnce = false
		methodsAllowedToFail = false
	}

Issues
------
* using KSPAddon.Startup.PSystemSpawn may cause issues since the solar system is already created at reload. (untested)

Credits
-------
Ezriilc (http://www.kerbaltekaerospace.com/). Used hyperedit GUI code as base for GUI.

Terms of Use
------------
http://creativecommons.org/licenses/by-nc-sa/3.0/

Note1:
You can leave your plugin in \GameData\ if you need your parts/partmodules in your code. However you MUST put your parts and modules inside a different DLL which loads them. If you place any code that launches (KSPAddon) it won't be unloaded when all plugins are reloaded, and you will wind up with duplicated code being executing.