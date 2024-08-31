msbuild Source/PawnTimeline.sln
rm -rf $HOME/.var/app/com.valvesoftware.Steam/.local/share/Steam/steamapps/common/RimWorld/Mods/rimworld-pawn-timeline
cp -r ../rimworld-pawn-timeline/ $HOME/.var/app/com.valvesoftware.Steam/.local/share/Steam/steamapps/common/RimWorld/Mods/

