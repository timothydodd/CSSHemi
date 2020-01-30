# CSSHemi
C# CSS Query Library

This library is used to extract HTML or XML elements out of a string using CSS selectors. Tuned for performance. Helps with scraping elements out of HTML.

The core elements of this library use string extensions.  

## Example:

```C#
string html = "<div><a class='link' href='www.google.com'>Test</a></div>"

string result = html.Query("a[class]").First();

```

## Results:

```C#
"<a class='link' href='www.google.com'>Test</a>"
```
