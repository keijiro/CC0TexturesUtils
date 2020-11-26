CC0TexturesUtils
================

Unity editor scripts for preprocessing [CC0 Textures]

[CC0 Textures]: https://cc0textures.com/

NormalFix.cs
------------

The normal map format used in CC0 Textures is different from the standard normal map format in Unity. If you use it without a fix, it will end up giving wrong bump directions.

![screenshot](https://i.imgur.com/krEgi7k.jpg)
![screenshot](https://i.imgur.com/eYhtupr.jpg)

(left: original normal map, right: fixed normal map)

To fix the normal map format, select textures in the project view, then select "CC0 Textures" -> "Fix Normal Map" from the right-click menu.
