﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Rajneesh Noonia" email="Rajneesh.Noonia@Xansa.com"/>
//     <version>$Revision: 1661 $</version>
// </file>

namespace AIMS.Libraries.Scripting
{
    // define some delegates for functional programming
    public delegate void Action();

    // void Action<A>(A arg1) is already defined in System.
    public delegate void Action<A, B>(A arg1, B arg2);

    public delegate void Action<A, B, C>(A arg1, B arg2, C arg3);

    public delegate R Func<R>();

    public delegate R Func<A, R>(A arg1);

    public delegate R Func<A, B, R>(A arg1, B arg2);

    public delegate R Func<A, B, C, R>(A arg1, B arg2, C arg3);
}