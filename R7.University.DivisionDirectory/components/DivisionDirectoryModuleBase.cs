﻿//
// DivisionDirectoryModuleBase.cs
//
// Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
// Copyright (c) 2014 
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using DotNetNuke.Entities.Modules;

namespace R7.University.DivisionDirectory
{
    /// <summary>
    /// Employee directory module base.
    /// </summary>
    public class DivisionDirectoryPortalModuleBase : PortalModuleBase
    {
        private DivisionDirectoryController ctrl = null;

        protected DivisionDirectoryController DivisionDirectoryController
        {
            get { return ctrl ?? (ctrl = new DivisionDirectoryController ()); }
        }

        private DivisionDirectorySettings settings = null;

        protected DivisionDirectorySettings DivisionDirectorySettings
        {
            get { return settings ?? (settings = new DivisionDirectorySettings (this)); }
        }
    }

    /// <summary>
    /// Employee directory module settings base.
    /// </summary>
    public class DivisionDirectoryModuleSettingsBase : ModuleSettingsBase
    {
        private DivisionDirectoryController ctrl = null;

        protected DivisionDirectoryController DivisionDirectoryController
        {
            get { return ctrl ?? (ctrl = new DivisionDirectoryController ()); }
        }

        private DivisionDirectorySettings settings = null;

        protected DivisionDirectorySettings DivisionDirectorySettings
        {
            get { return settings ?? (settings = new DivisionDirectorySettings (this)); }
        }
    }
}
