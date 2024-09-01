msbuild Source/AllColonistsTab.sln
rm -rf $HOME/.var/app/com.valvesoftware.Steam/.local/share/Steam/steamapps/common/RimWorld/Mods/rimworld-all-colonists-tab
cp -r ../rimworld-all-colonists-tab/ $HOME/.var/app/com.valvesoftware.Steam/.local/share/Steam/steamapps/common/RimWorld/Mods/

