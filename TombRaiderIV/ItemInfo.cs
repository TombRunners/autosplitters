namespace TR4
{
    internal struct ItemInfo
    {
        internal struct PHD_3DPOS
        {
            public int x_pos { get; }
            public int y_pos { get; }
            public int z_pos { get; }
            public short x_rot { get; }
            public short y_rot { get; }
            public short z_rot { get; }
        };
        
        internal struct PHD_VECTOR
        {
            public int x { get; }
            public int y { get; }
            public int z { get; }
        };				
        
        internal struct PCLIGHT
        {
            public float x { get; }
            public float y { get; }
            public float z { get; }
            public float r { get; }
            public float g { get; }
            public float b { get; }
            public int shadow { get; }
            public float Inner { get; }
            public float Outer { get; }
            public float InnerAngle { get; }
            public float OuterAngle { get; }
            public float Cutoff { get; }
            public float nx { get; }
            public float ny { get; }
            public float nz { get; }
            public int ix { get; }
            public int iy { get; }
            public int iz { get; }
            public int inx { get; }
            public int iny { get; }
            public int inz { get; }
            public float tr { get; }
            public float tg { get; }
            public float tb { get; }
            public float rs { get; }
            public float gs { get; }
            public float bs { get; }
            public int fcnt { get; }
            public byte Type { get; }
            public byte Active { get; }
            public PHD_VECTOR rlp { get; }
            public int Range { get; }
        };

        internal struct ITEM_LIGHT
        {
            public int r { get; }
            public int g { get;  } 
            public int b { get; }
            public int ambient { get; }
            public int rs { get; }
            public int gs { get; }
            public int bs { get; }
            public int fcnt { get; }
            public PCLIGHT_Array CurrentLights { get; }
            public PCLIGHT_Array PrevLights { get; }
            public int nCurrentLights { get; }
            public int nPrevLights { get; }
            public int room_number { get; }
            public int RoomChange { get; }
            public PHD_VECTOR item_pos { get; }
            public uint pCurrentLights { get; }
            public uint pPrevLights { get; }
        };
        
        // C# Doesn't allow fixed buffer sizes for strictly non-composed primitive types.
        internal struct PCLIGHT_Array
        {
            public PCLIGHT light00 { get; }
            public PCLIGHT light01 { get; }
            public PCLIGHT light02 { get; }
            public PCLIGHT light03 { get; }
            public PCLIGHT light04 { get; }
            public PCLIGHT light05 { get; }
            public PCLIGHT light06 { get; }
            public PCLIGHT light07 { get; }
            public PCLIGHT light08 { get; }
            public PCLIGHT light09 { get; }
            public PCLIGHT light10 { get; }
            public PCLIGHT light11 { get; }
            public PCLIGHT light12 { get; }
            public PCLIGHT light13 { get; }
            public PCLIGHT light14 { get; }
            public PCLIGHT light15 { get; }
            public PCLIGHT light16 { get; }
            public PCLIGHT light17 { get; }
            public PCLIGHT light18 { get; }
            public PCLIGHT light19 { get; }
            public PCLIGHT light20 { get; }
        }

        internal struct item_flags_Array
        {
            public short item_flag00 { get; }
            public short item_flag01 { get; }
            public short item_flag02 { get; }
            public short item_flag03 { get; }
        }

        public int floor { get; }
        public uint touch_bits { get; }
        public uint mesh_bits { get; }
        public short object_number { get; }
        public short current_anim_state { get; }
        public short goal_anim_state { get; }
        public short required_anim_state { get; }
        public short anim_number { get; }
        public short frame_number { get; }
        public short room_number { get; }
        public short next_item { get; }
        public short next_active { get; }
        public short speed { get; }
        public short fallspeed { get; }
        public short hit_points { get; }
        public ushort box_number { get; }
        public short timer { get; }
        public ushort flags { get; }
        public short shade { get; }
        public short trigger_flags { get; }
        public short carried_item { get; }
        public short after_death { get; }
        public ushort fired_weapon { get; }
        public item_flags_Array item_flags { get; }
        public uint pData { get; }
        public PHD_3DPOS pos { get; }
        public ITEM_LIGHT il { get; }
        public uint bitfield { get; }
        public uint meshswap_meshbits { get; }
        public short draw_room { get; }
        public short TOSSPAD {  get; }

        public ItemInfo(int floor, uint touch_bits, uint mesh_bits, short object_number,
            short current_anim_state, short goal_anim_state, short required_anim_state, short anim_number, short frame_number,
            short room_number, short next_item, short next_active, short speed, short fallspeed, short hit_points,
            ushort box_number, short timer, ushort flags, short shade, short trigger_flags, short carried_item, short after_death,
            ushort fired_weapon, item_flags_Array item_flags, uint pData, PHD_3DPOS pos, ITEM_LIGHT il,
            uint bitfield, uint meshswap_meshbits, short draw_room, short TOSSPAD)
        {
            this.floor = floor;
            this.touch_bits = touch_bits;
            this.mesh_bits = mesh_bits;
            this.object_number = object_number;
            this.current_anim_state = current_anim_state;
            this.goal_anim_state = goal_anim_state;
            this.required_anim_state = required_anim_state;
            this.anim_number = anim_number;
            this.frame_number = frame_number;
            this.room_number = room_number;
            this.next_item = next_item;
            this.next_active = next_active;
            this.speed = speed;
            this.fallspeed = fallspeed;
            this.hit_points = hit_points;
            this.box_number = box_number;
            this.timer = timer;
            this.flags = flags;
            this.shade = shade;
            this.trigger_flags = trigger_flags;
            this.carried_item = carried_item;
            this.after_death = after_death;
            this.fired_weapon = fired_weapon;
            this.item_flags = item_flags;
            this.pData = pData;
            this.pos = pos;
            this.il = il;
            this.bitfield = bitfield;
            this.meshswap_meshbits = meshswap_meshbits;
            this.draw_room = draw_room;
            this.TOSSPAD = TOSSPAD;
        }
    }
}
