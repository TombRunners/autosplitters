
<Structures>
  <Structure Name="Tr1LevelStats" AutoFill="0" AutoCreate="1" DefaultHex="0" AutoDestroy="0" DoNotSaveLocal="0" RLECompression="1" AutoCreateStructsize="4096">
    <Elements>
      <Element Offset="0" Vartype="4 Bytes" Bytesize="4" OffsetHex="00000000" Description="Crystals Used" DisplayMethod="signed integer"/>
      <Element Offset="4" Vartype="4 Bytes" Bytesize="4" OffsetHex="00000004" Description="Timer" DisplayMethod="unsigned integer"/>
      <Element Offset="8" Vartype="4 Bytes" Bytesize="4" OffsetHex="00000008" Description="Ammo Used" DisplayMethod="unsigned integer"/>
      <Element Offset="12" Vartype="4 Bytes" Bytesize="4" OffsetHex="0000000C" Description="Ammo Hit" DisplayMethod="unsigned integer"/>
      <Element Offset="16" Vartype="4 Bytes" Bytesize="4" OffsetHex="00000010" Description="Kills" DisplayMethod="unsigned integer"/>
      <Element Offset="20" Vartype="4 Bytes" Bytesize="4" OffsetHex="00000014" Description="Distance Travelled" DisplayMethod="unsigned integer"/>
      <Element Offset="24" Vartype="2 Bytes" Bytesize="2" OffsetHex="00000018" Description="Secrets Bit Array" DisplayMethod="unsigned integer"/>
      <Element Offset="26" Vartype="Byte" Bytesize="1" OffsetHex="0000001A" Description="Pickups" DisplayMethod="signed integer"/>
      <Element Offset="27" Vartype="Byte" Bytesize="1" OffsetHex="0000001B" Description="Health Used (1 = Small, 2 = Large)" DisplayMethod="signed integer"/>
    </Elements>
  </Structure>
  <Structure Name="Tr1StartInfo" AutoFill="0" AutoCreate="1" DefaultHex="0" AutoDestroy="0" DoNotSaveLocal="0" RLECompression="1" AutoCreateStructsize="4096">
    <Elements>
      <Element Offset="0" Vartype="2 Bytes" Bytesize="2" OffsetHex="00000000" Description="Pistol Ammo" DisplayMethod="unsigned integer"/>
      <Element Offset="2" Vartype="2 Bytes" Bytesize="2" OffsetHex="00000002" Description="Magnum Ammo" DisplayMethod="unsigned integer"/>
      <Element Offset="4" Vartype="2 Bytes" Bytesize="2" OffsetHex="00000004" Description="Uzi Ammo" DisplayMethod="unsigned integer"/>
      <Element Offset="6" Vartype="2 Bytes" Bytesize="2" OffsetHex="00000006" Description="Shotgun Ammo" DisplayMethod="unsigned integer"/>
      <Element Offset="8" Vartype="Byte" Bytesize="1" OffsetHex="00000008" Description="Small Medis" DisplayMethod="unsigned integer"/>
      <Element Offset="9" Vartype="Byte" Bytesize="1" OffsetHex="00000009" Description="Large Medis" DisplayMethod="unsigned integer"/>
      <Element Offset="10" Vartype="Byte" Bytesize="1" OffsetHex="0000000A" Description="Scions" DisplayMethod="unsigned integer"/>
      <Element Offset="11" Vartype="Byte" Bytesize="1" OffsetHex="0000000B" Description="Gun Status" DisplayMethod="signed integer"/>
      <Element Offset="12" Vartype="Byte" Bytesize="1" OffsetHex="0000000C" Description="Gun Type" DisplayMethod="signed integer"/>
      <Element Offset="13" Vartype="Byte" Bytesize="1" RLECount="3" OffsetHex="0000000D" Description="N/A" DisplayMethod="unsigned integer"/>
      <Element Nested="1" Offset="16" Vartype="Pointer" Bytesize="8" OffsetHex="00000010" ChildStruct="Tr1LevelStats" Description="Stats" DisplayMethod="unsigned integer"/>
      <Element Offset="44" Vartype="2 Bytes" Bytesize="2" OffsetHex="0000002C" Description="Bits: Available, Pistols, Uzis, Magnum, Shotgun, Costume" DisplayMethod="unsigned integer"/>
    </Elements>
  </Structure>
  <Structure Name="Tr1SaveGame" AutoFill="0" AutoCreate="1" DefaultHex="0" AutoDestroy="0" DoNotSaveLocal="0" RLECompression="1" AutoCreateStructsize="4096">
    <Elements>
      <Element Offset="0" Vartype="4 Bytes" Bytesize="4" OffsetHex="00000000" Description="Saved" DisplayMethod="signed integer"/>
      <Element Offset="4" Vartype="4 Bytes" Bytesize="4" OffsetHex="00000004" Description="New Game Plus Save" DisplayMethod="signed integer"/>
      <Element Offset="8" Vartype="4 Bytes" Bytesize="4" OffsetHex="00000008" Description="Save Game Number (Counter)" DisplayMethod="signed integer"/>
      <Element Nested="1" Offset="12" Vartype="Pointer" Bytesize="8" OffsetHex="0000000C" ChildStruct="Tr1StartInfo" Description="StartInfo" DisplayMethod="unsigned integer"/>
      <Element Nested="1" Offset="60" Vartype="Pointer" Bytesize="8" OffsetHex="0000003C" ChildStruct="Tr1StartInfo" Description="StartInfo" DisplayMethod="unsigned integer"/>
      <Element Nested="1" Offset="108" Vartype="Pointer" Bytesize="8" OffsetHex="0000006C" ChildStruct="Tr1StartInfo" Description="StartInfo" DisplayMethod="unsigned integer"/>
      <Element Nested="1" Offset="156" Vartype="Pointer" Bytesize="8" OffsetHex="0000009C" ChildStruct="Tr1StartInfo" Description="StartInfo" DisplayMethod="unsigned integer"/>
      <Element Nested="1" Offset="204" Vartype="Pointer" Bytesize="8" OffsetHex="000000CC" ChildStruct="Tr1StartInfo" Description="StartInfo" DisplayMethod="unsigned integer"/>
      <Element Nested="1" Offset="252" Vartype="Pointer" Bytesize="8" OffsetHex="000000FC" ChildStruct="Tr1StartInfo" Description="StartInfo" DisplayMethod="unsigned integer"/>
      <Element Nested="1" Offset="300" Vartype="Pointer" Bytesize="8" OffsetHex="0000012C" ChildStruct="Tr1StartInfo" Description="StartInfo" DisplayMethod="unsigned integer"/>
      <Element Nested="1" Offset="348" Vartype="Pointer" Bytesize="8" OffsetHex="0000015C" ChildStruct="Tr1StartInfo" Description="StartInfo" DisplayMethod="unsigned integer"/>
      <Element Nested="1" Offset="396" Vartype="Pointer" Bytesize="8" OffsetHex="0000018C" ChildStruct="Tr1StartInfo" Description="StartInfo" DisplayMethod="unsigned integer"/>
      <Element Nested="1" Offset="444" Vartype="Pointer" Bytesize="8" OffsetHex="000001BC" ChildStruct="Tr1StartInfo" Description="StartInfo" DisplayMethod="unsigned integer"/>
      <Element Nested="1" Offset="492" Vartype="Pointer" Bytesize="8" OffsetHex="000001EC" ChildStruct="Tr1StartInfo" Description="StartInfo" DisplayMethod="unsigned integer"/>
      <Element Nested="1" Offset="540" Vartype="Pointer" Bytesize="8" OffsetHex="0000021C" ChildStruct="Tr1StartInfo" Description="StartInfo" DisplayMethod="unsigned integer"/>
      <Element Nested="1" Offset="588" Vartype="Pointer" Bytesize="8" OffsetHex="0000024C" ChildStruct="Tr1StartInfo" Description="StartInfo" DisplayMethod="unsigned integer"/>
      <Element Nested="1" Offset="636" Vartype="Pointer" Bytesize="8" OffsetHex="0000027C" ChildStruct="Tr1StartInfo" Description="StartInfo" DisplayMethod="unsigned integer"/>
      <Element Nested="1" Offset="684" Vartype="Pointer" Bytesize="8" OffsetHex="000002AC" ChildStruct="Tr1StartInfo" Description="StartInfo" DisplayMethod="unsigned integer"/>
      <Element Nested="1" Offset="732" Vartype="Pointer" Bytesize="8" OffsetHex="000002DC" ChildStruct="Tr1StartInfo" Description="StartInfo" DisplayMethod="unsigned integer"/>
      <Element Nested="1" Offset="780" Vartype="Pointer" Bytesize="8" OffsetHex="0000030C" ChildStruct="Tr1StartInfo" Description="StartInfo" DisplayMethod="unsigned integer"/>
      <Element Nested="1" Offset="828" Vartype="Pointer" Bytesize="8" OffsetHex="0000033C" ChildStruct="Tr1StartInfo" Description="StartInfo" DisplayMethod="unsigned integer"/>
      <Element Nested="1" Offset="876" Vartype="Pointer" Bytesize="8" OffsetHex="0000036C" ChildStruct="Tr1StartInfo" Description="StartInfo" DisplayMethod="unsigned integer"/>
      <Element Nested="1" Offset="924" Vartype="Pointer" Bytesize="8" OffsetHex="0000039C" ChildStruct="Tr1StartInfo" Description="StartInfo" DisplayMethod="unsigned integer"/>
      <Element Nested="1" Offset="972" Vartype="Pointer" Bytesize="8" OffsetHex="000003CC" ChildStruct="Tr1StartInfo" Description="StartInfo" DisplayMethod="unsigned integer"/>
      <Element Nested="1" Offset="1020" Vartype="Pointer" Bytesize="8" OffsetHex="000003FC" ChildStruct="Tr1StartInfo" Description="StartInfo" DisplayMethod="unsigned integer"/>
      <Element Nested="1" Offset="1068" Vartype="Pointer" Bytesize="8" OffsetHex="0000042C" ChildStruct="Tr1StartInfo" Description="StartInfo" DisplayMethod="unsigned integer"/>
      <Element Nested="1" Offset="1116" Vartype="Pointer" Bytesize="8" OffsetHex="0000045C" ChildStruct="Tr1StartInfo" Description="StartInfo" DisplayMethod="unsigned integer"/>
      <Element Nested="1" Offset="1164" Vartype="Pointer" Bytesize="8" OffsetHex="0000048C" ChildStruct="Tr1StartInfo" Description="StartInfo" DisplayMethod="unsigned integer"/>
      <Element Nested="1" Offset="1212" Vartype="Pointer" Bytesize="8" OffsetHex="000004BC" ChildStruct="Tr1StartInfo" Description="StartInfo" DisplayMethod="unsigned integer"/>
      <Element Nested="1" Offset="1260" Vartype="Pointer" Bytesize="8" OffsetHex="000004EC" ChildStruct="Tr1StartInfo" Description="StartInfo" DisplayMethod="unsigned integer"/>
      <Element Nested="1" Offset="1308" Vartype="Pointer" Bytesize="8" OffsetHex="0000051C" ChildStruct="Tr1StartInfo" Description="StartInfo" DisplayMethod="unsigned integer"/>
      <Element Nested="1" Offset="1356" Vartype="Pointer" Bytesize="8" OffsetHex="0000054C" ChildStruct="Tr1StartInfo" Description="StartInfo" DisplayMethod="unsigned integer"/>
      <Element Nested="1" Offset="1404" Vartype="Pointer" Bytesize="8" OffsetHex="0000057C" ChildStruct="Tr1StartInfo" Description="StartInfo" DisplayMethod="unsigned integer"/>
      <Element Nested="1" Offset="1452" Vartype="Pointer" Bytesize="8" OffsetHex="000005AC" ChildStruct="Tr1StartInfo" Description="StartInfo" DisplayMethod="unsigned integer"/>
      <Element Nested="1" Offset="1500" Vartype="Pointer" Bytesize="8" OffsetHex="000005DC" ChildStruct="Tr1StartInfo" Description="StartInfo" DisplayMethod="unsigned integer"/>
      <Element Nested="1" Offset="1548" Vartype="Pointer" Bytesize="8" OffsetHex="0000060C" ChildStruct="Tr1LevelStats" Description="Current Level Stats" DisplayMethod="unsigned integer"/>
      <Element Offset="1576" Vartype="2 Bytes" Bytesize="2" OffsetHex="00000628" Description="Save Level Number" DisplayMethod="signed integer"/>
      <Element Offset="1578" Vartype="Byte" Bytesize="1" OffsetHex="0000062A" Description="Bonus Flag" DisplayMethod="unsigned integer"/>
      <Element Offset="1579" Vartype="Byte" Bytesize="1" OffsetHex="0000062B" Description="Num Pickup 1" DisplayMethod="unsigned integer"/>
      <Element Offset="1580" Vartype="Byte" Bytesize="1" OffsetHex="0000062C" Description="Num Pickup 2" DisplayMethod="unsigned integer"/>
      <Element Offset="1581" Vartype="Byte" Bytesize="1" OffsetHex="0000062D" Description="Num Puzzle 1" DisplayMethod="unsigned integer"/>
      <Element Offset="1582" Vartype="Byte" Bytesize="1" OffsetHex="0000062E" Description="Num Puzzle 2" DisplayMethod="unsigned integer"/>
      <Element Offset="1583" Vartype="Byte" Bytesize="1" OffsetHex="0000062F" Description="Num Puzzle 3" DisplayMethod="unsigned integer"/>
      <Element Offset="1584" Vartype="Byte" Bytesize="1" OffsetHex="00000630" Description="Num Puzzle 4" DisplayMethod="unsigned integer"/>
      <Element Offset="1585" Vartype="Byte" Bytesize="1" OffsetHex="00000631" Description="Num Key 1" DisplayMethod="unsigned integer"/>
      <Element Offset="1586" Vartype="Byte" Bytesize="1" OffsetHex="00000632" Description="Num Key 2" DisplayMethod="unsigned integer"/>
      <Element Offset="1587" Vartype="Byte" Bytesize="1" OffsetHex="00000633" Description="Num Key 3" DisplayMethod="unsigned integer"/>
      <Element Offset="1588" Vartype="Byte" Bytesize="1" OffsetHex="00000634" Description="Num Key 4" DisplayMethod="unsigned integer"/>
      <Element Offset="1589" Vartype="Byte" Bytesize="1" OffsetHex="00000635" Description="Num Lead Bars" DisplayMethod="unsigned integer"/>
      <Element Offset="1590" Vartype="Byte" Bytesize="1" RLECount="2" OffsetHex="00000636" Description="N/A" DisplayMethod="unsigned integer"/>
      <Element Offset="1592" Vartype="Array of byte" Bytesize="28" OffsetHex="00000638" Description="Trophy / Achievements" DisplayMethod="unsigned integer"/>
      <Element Offset="1620" Vartype="Array of byte" Bytesize="120" OffsetHex="00000654" Description="N/A" DisplayMethod="unsigned integer"/>
      <Element Nested="1" Offset="1740" Vartype="Pointer" Bytesize="8" OffsetHex="000006CC" ChildStruct="Tr1LaraParams" Description="Lara Params" DisplayMethod="unsigned integer"/>
      <Element Offset="1752" Vartype="Byte" Bytesize="1" RLECount="4" OffsetHex="000006D8" Description="N/A" DisplayMethod="unsigned integer"/>
      <Element Offset="1756" Vartype="Byte" Bytesize="1" OffsetHex="000006DC" Description="Checksum" DisplayMethod="signed integer"/>
      <Element Offset="1757" Vartype="Array of byte" Bytesize="14336" OffsetHex="000006DD" Description="Buffer" DisplayMethod="unsigned integer"/>
      <Element Offset="16093" Vartype="Byte" Bytesize="1" RLECount="3" OffsetHex="00003EDD" Description="N/A" DisplayMethod="unsigned integer"/>
    </Elements>
  </Structure>
  <Structure Name="Tr1LaraParams" AutoFill="0" AutoCreate="1" DefaultHex="0" AutoDestroy="0" DoNotSaveLocal="0" RLECompression="1" AutoCreateStructsize="4096">
    <Elements>
      <Element Offset="0" Vartype="4 Bytes" Bytesize="4" OffsetHex="00000000" Description="Outfit" DisplayMethod="signed integer"/>
      <Element Offset="4" Vartype="Byte" Bytesize="1" OffsetHex="00000004" Description="Left Hand" DisplayMethod="signed integer"/>
      <Element Offset="5" Vartype="Byte" Bytesize="1" OffsetHex="00000005" Description="Right Hand" DisplayMethod="signed integer"/>
      <Element Offset="6" Vartype="Byte" Bytesize="1" OffsetHex="00000006" Description="Left Holster" DisplayMethod="signed integer"/>
      <Element Offset="7" Vartype="Byte" Bytesize="1" OffsetHex="00000007" Description="Right Holster" DisplayMethod="signed integer"/>
      <Element Offset="8" Vartype="Byte" Bytesize="1" OffsetHex="00000008" Description="Back" DisplayMethod="signed integer"/>
      <Element Offset="9" Vartype="Byte" Bytesize="1" OffsetHex="00000009" Description="Face" DisplayMethod="signed integer"/>
      <Element Offset="10" Vartype="Byte" Bytesize="1" OffsetHex="0000000A" Description="Left Holster Back" DisplayMethod="signed integer"/>
      <Element Offset="11" Vartype="Byte" Bytesize="1" OffsetHex="0000000B" Description="Right Holster Back" DisplayMethod="signed integer"/>
      <Element Offset="12" Vartype="Byte" Bytesize="1" OffsetHex="0000000C" Description="Pad Byte" DisplayMethod="signed integer"/>
    </Elements>
  </Structure>
  <Structure Name="Tr1Achievements" AutoFill="0" AutoCreate="1" DefaultHex="0" AutoDestroy="0" DoNotSaveLocal="0" RLECompression="1" AutoCreateStructsize="4096">
    <Elements>
      <Element Offset="0" Vartype="Array of byte" Bytesize="19" OffsetHex="00000000" Description="Level Secrets Array" DisplayMethod="unsigned integer"/>
      <Element Offset="19" Vartype="Byte" Bytesize="1" OffsetHex="00000013" Description="N/A" DisplayMethod="unsigned integer"/>
      <Element Offset="20" Vartype="Byte" Bytesize="1" OffsetHex="00000014" Description="Bits: Level Skip, Weapon Cheat, Thor Damage, T. Rex Damaged, No Medipack, Raptor Damage, Dopperganger Damaged, Centaur Killed" DisplayMethod="unsigned integer"/>
      <Element Offset="21" Vartype="Byte" Bytesize="1" OffsetHex="00000015" Description="Bits: Fired Non Pistols, Time Device Peru, Time Device Greece, Throne Q, Throne T, Throne N, Mummy Caves, Mummy Qualopec" DisplayMethod="unsigned integer"/>
      <Element Offset="22" Vartype="Byte" Bytesize="1" OffsetHex="00000016" Description="Bits: Rolling Ball Damage, Stepped On Slide Trap, Picked Up Cog, Centaur Damage, Used Hive Slide, Medipack Secret, Killed Crocs, Got Thor Key" DisplayMethod="unsigned integer"/>
      <Element Offset="23" Vartype="Byte" Bytesize="1" OffsetHex="00000017" Description="Bits: Got Uzis, Got Uzi Clips, Reserved * 4, N/A * 2" DisplayMethod="unsigned integer"/>
      <Element Offset="24" Vartype="Array of byte" Bytesize="19" OffsetHex="00000018" Description="Damocles Sword Damage (0 = Inactive, 2 = Yes, 3 = No)" DisplayMethod="unsigned integer"/>
      <Element Offset="43" Vartype="Byte" Bytesize="1" OffsetHex="0000002B" Description="Level Meds" DisplayMethod="unsigned integer"/>
      <Element Offset="44" Vartype="Byte" Bytesize="1" OffsetHex="0000002C" Description="Level Meds Collected" DisplayMethod="unsigned integer"/>
      <Element Offset="45" Vartype="Byte" Bytesize="1" OffsetHex="0000002D" Description="N/A" DisplayMethod="unsigned integer"/>
      <Element Offset="46" Vartype="2 Bytes" Bytesize="2" OffsetHex="0000002E" Description="Pierre Damage By Magnums" DisplayMethod="unsigned integer"/>
      <Element Offset="48" Vartype="2 Bytes" Bytesize="2" OffsetHex="00000030" Description="Natla Damage By Magnums" DisplayMethod="unsigned integer"/>
      <Element Offset="50" Vartype="2 Bytes" Bytesize="2" OffsetHex="00000032" Description="Baldy Damage By Shotgun" DisplayMethod="unsigned integer"/>
      <Element Offset="52" Vartype="2 Bytes" Bytesize="2" OffsetHex="00000034" Description="Emperor Box Ape Kills" DisplayMethod="unsigned integer"/>
      <Element Offset="54" Vartype="2 Bytes" Bytesize="2" OffsetHex="00000036" Description="Emperor Box Lion Kills" DisplayMethod="unsigned integer"/>
      <Element Offset="56" Vartype="2 Bytes" Bytesize="2" OffsetHex="00000038" Description="Emperor Box Lioness Kills" DisplayMethod="unsigned integer"/>
      <Element Offset="58" Vartype="Array of byte" Bytesize="3" OffsetHex="0000003A" Description="Bear Seen Walking" DisplayMethod="unsigned integer"/>
      <Element Offset="61" Vartype="Byte" Bytesize="1" OffsetHex="0000003D" Description="Pickups Underwater" DisplayMethod="unsigned integer"/>
      <Element Offset="62" Vartype="2 Bytes" Bytesize="2" OffsetHex="0000003E" Description="Pickups" DisplayMethod="unsigned integer"/>
      <Element Offset="64" Vartype="2 Bytes" Bytesize="2" OffsetHex="00000040" Description="Pickups UB" DisplayMethod="unsigned integer"/>
      <Element Offset="66" Vartype="2 Bytes" Bytesize="2" OffsetHex="00000042" Description="Kills" DisplayMethod="unsigned integer"/>
      <Element Offset="68" Vartype="Array of byte" Bytesize="9" OffsetHex="00000044" Description="Collaped Floor Tiles" DisplayMethod="unsigned integer"/>
      <Element Offset="77" Vartype="Byte" Bytesize="1" OffsetHex="0000004D" Description="N/A" DisplayMethod="unsigned integer"/>
      <Element Offset="78" Vartype="Byte" Bytesize="1" OffsetHex="0000004E" Description="Bad Moves Made" DisplayMethod="unsigned integer"/>
      <Element Offset="79" Vartype="Byte" Bytesize="1" OffsetHex="0000004F" Description="N/A" DisplayMethod="unsigned integer"/>
      <Element Offset="80" Vartype="Byte" Bytesize="1" OffsetHex="00000050" Description="Finished Shadow of the Cat" DisplayMethod="unsigned integer"/>
      <Element Offset="81" Vartype="Byte" Bytesize="1" OffsetHex="00000051" Description="Finished Unfinished Business" DisplayMethod="unsigned integer"/>
      <Element Offset="82" Vartype="Byte" Bytesize="1" OffsetHex="00000052" Description="Atlantean Stronghold Kills" DisplayMethod="unsigned integer"/>
      <Element Offset="83" Vartype="Byte" Bytesize="1" OffsetHex="00000053" Description="N/A" DisplayMethod="unsigned integer"/>
      <Element Offset="84" Vartype="2 Bytes" Bytesize="2" OffsetHex="00000054" Description="Centaur Kills" DisplayMethod="unsigned integer"/>
      <Element Offset="86" Vartype="Array of byte" Bytesize="4" OffsetHex="00000056" Description="UB Balls Rolled" DisplayMethod="unsigned integer"/>
      <Element Offset="90" Vartype="Byte" Bytesize="1" OffsetHex="0000005A" Description="Time Looked At Item" DisplayMethod="unsigned integer"/>
      <Element Offset="91" Vartype="Byte" Bytesize="1" OffsetHex="0000005B" Description="N/A" DisplayMethod="unsigned integer"/>
      <Element Offset="92" Vartype="2 Bytes" Bytesize="2" OffsetHex="0000005C" Description="Scream Time" DisplayMethod="unsigned integer"/>
      <Element Offset="94" Vartype="Array of byte" Bytesize="17" OffsetHex="0000005E" Description="Universal" DisplayMethod="unsigned integer"/>
      <Element Offset="110" Vartype="2 Bytes" Bytesize="2" OffsetHex="0000006E" Description="Cat Entrance Block Moved" DisplayMethod="unsigned integer"/>
      <Element Offset="112" Vartype="2 Bytes" Bytesize="2" OffsetHex="00000070" Description="Gym Exercises" DisplayMethod="unsigned integer"/>
      <Element Offset="114" Vartype="2 Bytes" Bytesize="2" OffsetHex="00000072" Description="N/A" DisplayMethod="unsigned integer"/>
      <Element Offset="116" Vartype="4 Bytes" Bytesize="4" OffsetHex="00000074" Description="Cat Secret Block Moves" DisplayMethod="unsigned integer"/>
      <Element Offset="120" Vartype="4 Bytes" Bytesize="4" OffsetHex="00000078" Description="Num Saves" DisplayMethod="unsigned integer"/>
      <Element Offset="124" Vartype="4 Bytes" Bytesize="4" RLECount="6" OffsetHex="0000007C" Description="Reserved" DisplayMethod="unsigned integer"/>
    </Elements>
  </Structure>
</Structures>