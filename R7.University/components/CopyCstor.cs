﻿//
// CopyCstor.cs
//
// Author:
//       Roman M. Yagodin <roman.yagodin@gmail.com>
//
// Copyright (c) 2015 
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
using System.Runtime.CompilerServices;
using System.CodeDom;
using System.Web.Services.Description;
using Telerik.Web.UI;

namespace R7.University
{
    public static class CopyCstor
    {
        /// <summary>
        /// Copy the specified src object properties to dest object.
        /// </summary>
        /// <param name="src">Source object.</param>
        /// <param name="dest">Destination object.</param>
        /// <typeparam name="T">Common base type for src and dest objects.</typeparam>
        public static void Copy<T> (T src, T dest)
        {
            foreach (var pi in typeof (T).GetProperties ())
                if (pi.GetSetMethod () != null)
                    pi.SetValue (dest, pi.GetValue (src, null), null);
        }
    }
}

