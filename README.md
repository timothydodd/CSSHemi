# CSSHemi
C# CSS Query Library

This library is used to query HTML elements using the standard CSS similar to how JQuery works.

Example:

```C#
string html = "<div><a class='link' href='www.google.com'>Test</a></div>"

var result = html.Query("a[class]").First();

```


