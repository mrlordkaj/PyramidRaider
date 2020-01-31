using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if ANDROID
using Android.App;
#else
using System.Resources;
#endif

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Pyramid Raider")]
[assembly: AssemblyProduct("Pyramid Raider")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyDescription("Pyramid Raider")]
[assembly: AssemblyCompany("Openitvn")]
[assembly: AssemblyCopyright("Copyright © Openitvn 2014")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type. Only Windows
// assemblies support COM.
[assembly: ComVisible(false)]

// On Windows, the following GUID is for the ID of the typelib if this
// project is exposed to COM. On other platforms, it unique identifies the
// title storage container when deploying this assembly to the device.
[assembly: Guid("2cdc4ff1-984f-4022-b034-30f477d16313")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
[assembly: AssemblyVersion("1.5.0")]
[assembly: AssemblyFileVersion("1.5.0")]

#if ANDROID
#if DEBUG
[assembly: Application(Debuggable = true)]
#else
[assembly: Application(Debuggable = false)]
#endif
#else
[assembly: NeutralResourcesLanguageAttribute("en-US")]
#endif