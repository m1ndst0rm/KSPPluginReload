KSPPluginReload
================================
Author: m1nd0 (joopselen@gmail.com)

Description:
Plugin which will allow DLLs to be reloading into KSP without the need for a reboot.


Notice:
* Only classes which inherit MonoBehaviour and include the attribute KSPAddon will be reloaded. Parts & partmodules will not be reloaded.
* Only use this for developing/debugging. Since KSP doesn't allow AppDomains each reload loads a second instance of the plugin, therefore increasing the memory usage.

Installation
------------
1. download latest build and extract to ksp directory:
2. edit Settings.cfg in "\GameData\PluginReload". 
3. open AssemblyInfo.cs in your project and change AssemblyVersion & AssemblyFileVersion to "1.0.0.*". Visual Studio will auto-increment your build, this is the only way to allow reloading of a DLL.

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
* using KSPAddon.Startup.PSystemSpawn may cause issues since the solar system is allready created at reload. (untested)

Credits
-------
Ezriilc (http://www.kerbaltekaerospace.com/). Used hyperedit GUI code as base for GUI.

Terms of Use
------------
http://creativecommons.org/licenses/by-nc-sa/3.0/