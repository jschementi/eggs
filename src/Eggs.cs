﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Resources;

using System.Collections.Generic;

using Microsoft.Scripting.Silverlight;
using Microsoft.Scripting.Hosting;
using IronRuby.Hosting;
using Microsoft.Scripting;
using System.Reflection;

public class Eggs {

    private static ScriptEngine _engine;
    private static ScriptScope _scope;

    private static string[] _dlrAssemblies = new string[] { 
        "Microsoft.Scripting.ExtensionAttribute", 
        "Microsoft.Scripting.Core", 
        "Microsoft.Scripting", 
        "Microsoft.Scripting.Silverlight", 
        "IronRuby", 
        "IronRuby.Libraries" 
    }; //, "IronPython", "IronPython.Modules"

    public static void Start(Uri testsXapUri, StreamResourceInfo eggsXap) {
        var assemblies = LoadDLRAssemblies(eggsXap);
        InitializeDLR(eggsXap, assemblies);
        LoadEggs();
        DownloadTestsXap(testsXapUri, delegate(StreamResourceInfo testsXap) {
            ConfigureAndRunEggs(testsXap);
        });
    }

    private static void InitializeDLR(StreamResourceInfo xap, List<Assembly> assemblies) {
        DynamicApplication.XapFile = xap;

        var setup = DynamicApplication.CreateRuntimeSetup(assemblies);
        setup.DebugMode = true;
        var runtime = new ScriptRuntime(setup);
        
        // Load default silverlight assemblies for the script to have access to
        DynamicApplication.LoadDefaultAssemblies(runtime);

        // Load the assemblies into the runtime, giving the script access to them 
        assemblies.ForEach((a) => runtime.LoadAssembly(a));
        
        _engine = IronRuby.Ruby.GetEngine(runtime);
        _scope = _engine.CreateScope();
    }


    // Load assemblies needed for this code to run
    private static List<Assembly> LoadDLRAssemblies(StreamResourceInfo xap) {
        var assemblies = new List<Assembly>();
        foreach (string assm in _dlrAssemblies) {
            assemblies.Add(new AssemblyPart().Load(Application.GetResourceStream(xap, new Uri(string.Format("{0}.dll", assm), UriKind.Relative)).Stream));
        }
        return assemblies;
    }

    private static void LoadEggs() {
        var code = Package.GetFileContents(DynamicApplication.XapFile, "eggs.rb");
        var sourceCode = _engine.CreateScriptSourceFromString(code, "eggs.rb", SourceCodeKind.File);
        sourceCode.Compile(new ErrorFormatter.Sink()).Execute();

        Repl.Show(_engine, _scope);
    }

    private static Action<StreamResourceInfo> _onDownloadComplete;

    private static void DownloadTestsXap(Uri testsXapUri, Action<StreamResourceInfo> OnDownloadComplete) {
        _onDownloadComplete = OnDownloadComplete;
        WebClient wc = new WebClient(); 
        wc.OpenReadCompleted += new OpenReadCompletedEventHandler(DownloadTestXap_Complete);
        wc.OpenReadAsync(testsXapUri);
    }

    private static void DownloadTestXap_Complete(object sender, OpenReadCompletedEventArgs e) {
        if (e.Error == null) {
            var sri = new StreamResourceInfo(e.Result, null);
            if (_onDownloadComplete != null) {
                _onDownloadComplete.Invoke(sri);
            }
        } else {
            System.Windows.Browser.HtmlPage.Window.Alert("Tests failed to download");
        }
    }

    private static void ConfigureAndRunEggs(StreamResourceInfo testsXap) {
        DynamicApplication.XapFile = testsXap;

        ScriptScope scope = _engine.Runtime.Globals;
        IronRuby.Ruby.GetExecutionContext(_engine).DefineGlobalVariable("engine", _engine);
        
        var code = "Eggs.run($engine)";
        var source = _engine.CreateScriptSourceFromString(code, SourceCodeKind.File);
        source.Compile(new ErrorFormatter.Sink()).Execute(scope);
    }
}
