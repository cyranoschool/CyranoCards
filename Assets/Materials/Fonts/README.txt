In order to use textmeshes properly the proper font asset has to be created for each font.
The unicode characters to support must be declared as a range in the Font Creator window.

For example if you want to support Chinese font characters:
Find your range:
https://www.ssec.wisc.edu/~tomw/java/unicode.html#xAC00
http://jrgraphix.net/r/Unicode/4E00-9FFF
Stackoverflow etc.
(There are a lot of asian characters so a separate font asset can be created for extensions as well)

CJK Unified Set is in the range: 4E00-9FFF
Go to Windows->TextMeshPro->Font Creator
Set Hex range, proper atlas size, etc
Save Font asset

Add new font asset to TMP Settings as a fallback (In Assets Folder search "TMP Settings"

Noto cjk
0000-007E,4E00-9FA5,3000-303F,FF00-FFEF,3040-309F,30A0-30FF,AC00-D7AF
Size 19, padding 2, SourceHanSerif (not all ttf fonts support all the unicode characters needed)
https://github.com/adobe-fonts/source-han-serif

Latin Supplements A and B: 0100-024F