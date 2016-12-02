# CSSHemi
C# CSS Query Library

This library is used to query HTML elements out of a string using standard CSS selectors.

The core elements of this library use string extensions.  

Example:

```C#
string html = "<div><a class='link' href='www.google.com'>Test</a></div>"

var result = html.Query("a[class]").First();

```


