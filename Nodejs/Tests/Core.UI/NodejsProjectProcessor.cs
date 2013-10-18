﻿/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the Apache License, Version 2.0, please send an email to 
 * vspython@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Apache License, Version 2.0.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************/

using System.ComponentModel.Composition;
using TestUtilities.SharedProject;
using MSBuild = Microsoft.Build.Evaluation;

namespace Microsoft.Nodejs.Tests.UI {
    [Export(typeof(IProjectProcessor))]
    [ProjectExtension(".njsproj")]
    public class NodejsProjectProcessor : IProjectProcessor {
        public void PreProcess(MSBuild.Project project) {
            project.SetProperty("ProjectHome", ".");
            project.SetProperty("WorkingDirectory", ".");
            project.SetProperty("ProjectView", "ShowAllFiles");
        }

        public void PostProcess(MSBuild.Project project) {
            // Node.js projects are flavored which enables a lot of our functionality, so
            // setup our flavor.
            project.SetProperty("ProjectTypeGuids", "{3AF33F2E-1136-4D97-BBB7-1795711AC8B8};{349c5851-65df-11da-9384-00065b846f21};{9092AA53-FB77-4645-B42D-1CCCA6BD08BD}");

            var prop = project.SetProperty("VisualStudioVersion", "11.0");

            project.Xml.AddProperty("VisualStudioVersion", "11.0").Condition = "'$(VisualStudioVersion)' == ''";
            project.Xml.AddProperty("VSToolsPath", "$(MSBuildExtensionsPath32)\\Microsoft\\VisualStudio\\v$(VisualStudioVersion)").Condition = "'$(VSToolsPath)' == ''";

            var import = project.Xml.AddImport("$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props");
            import.Condition = "Exists('$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props')";
            project.Xml.AddImport("$(VSToolsPath)\\Node.js Tools\\Microsoft.NodejsTools.targets");
        }
    }
}