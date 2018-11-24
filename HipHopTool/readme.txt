= HipHopTool v0.4.9 by igorseabra4 =
Tool to extract and create HIP/HOP archive files used in games by Heavy Iron Studios.

Currently supported games:
- Scooby-Doo: Night of 100 Frights
- Spongebob Squarepants: Battle For Bikini Bottom
- The Incredibles Game
- The Spongebob Squarepants Movie Game
- The Incredibles: Rise of the Underminer
All platforms (GameCube, Xbox, Playstation 2, PC) for all games available in them should be supported.

= File Description =
HIP archives (internally, they are all HIPs, HOP is just a filename thing) are used by the games above to put together all assets used in a scene ingame. They are divided into multiple layers, and each layer contains multiple (sometimes thousands of) assets. Every texture, model, object, sound, pickup object etc is an asset.

This tool can unpack a HIP archive's assets each into a separate file so you can edit them and then create a new archive with your edited assets. You can remove and add new assets to the file as well. An INI file is used to determine some general settings for the HIP file and each of the assets. All assets are referenced by an asset ID, which is included in the INI and in the asset filenames (the names themselves don't matter, only the asset numbers).


= Usage =
You can simply open the tool and be presented with options to extract an archive or create one from an INI file.
You can also drag one or more HIP/HOP files into the executable to extract them to a default folder.
You can also create a batch script:


== Extracting ==
HipHopTool.exe -extract EXAMPLE.HIP -dest EXAMPLE_OUT

will extract the EXAMPLE.HIP file to a folder called EXAMPLE_OUT. You can also use -e, -unpack and -u for the same effect of -extract and -d for the same effect of -dest. Ommiting -dest will have the HIP extracted to a default folder.

By default, all assets will be exported to folders named after their types, but there's a -mode (or -m) setting which you can set to "single" or "multi". It's set to multi by default, but you can set it to single and all assets will be extracted to one single folder instead of multiple. For example:

HipHopTool.exe -extract EXAMPLE.HOP -mode single

will extract the EXAMPLE.HOP file to a default folder and all assets to one folder.

HipHopTool.exe -extract EXAMPLE.HOP -alphabetical

will extract the EXAMPLE.HOP file to a folder called EXAMPLE_OUT and the Settings.ini will have assets sorted alphabetically by their names. You can also use -alpha, -a for the same effect.


== Creating ==
HipHopTool.exe -create EXAMPLE_OUT\Settings.ini -dest OUT.HIP

will create a new archive called OUT.HIP from an INI file called Settings.ini located in the EXAMPLE_OUT folder. You can also use -c for the same effect of -create. Ommiting -dest will save the HIP to the same folder as Settings.ini (extension defaults to .HIP).

Note that you can use full paths and not just filenames for -extract, -create and -dest. If the path includes spaces, make sure to enclose it in quotes.


== INI Format ==
HipHopTool generates a Settings.ini file when extracting an archive, which is used to rebuild it later. You can edit this file in a text editor.

If you want to add a new asset, add it to the list in the layer you want it to be. Give it a unique asset ID and make sure the asset is also present in a folder with that same ID. Which folder you put it in doesn't matter, the file name doesn't matter either; as long as the asset ID which is the hexadecimal number at the start of the filename is the same, the program will find the asset and include it. The order doesn't matter either, as HipHopTool will sort the assets automatically when building the file. The asset ID in the INI is the hexadecimal number at the start of the asset entry (don't bother with the one at the end, that's a checksum which actually goes unused).

If an asset ID is referenced in the INI but there's no file for it, the program will show an error, and the resulting file will be unusable as it'll have an asset with no data. If there are multiple files with the same asset ID, the program will show an error, and the first one found will be used.

You can remove assets simply by removing the line from the INI (or commenting it with a # at the beginning), there's no need to delete the asset file as it will be ignored.