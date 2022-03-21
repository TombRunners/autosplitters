namespace TR4
{
    internal unsafe struct ItemInfo
    {
        internal struct PHD_3DPOS
        {
            public long x_pos { get; }
            public long y_pos { get; }
            public long z_pos { get; }
            public short x_rot { get; }
            public short y_rot { get; }
            public short z_rot { get; }
        };
        
        internal struct PHD_VECTOR
        {
            public long x { get; }
            public long y { get; }
            public long z { get; }
        };				
        
        internal struct PCLIGHT
        {
            public float x { get; }
            public float y { get; }
            public float z { get; }
            public float r { get; }
            public float g { get; }
            public float b { get; }
            public long shadow { get; }
            public float Inner { get; }
            public float Outer { get; }
            public float InnerAngle { get; }
            public float OuterAngle { get; }
            public float Cutoff { get; }
            public float nx { get; }
            public float ny { get; }
            public float nz { get; }
            public long ix { get; }
            public long iy { get; }
            public long iz { get; }
            public long inx { get; }
            public long iny { get; }
            public long inz { get; }
            public float tr { get; }
            public float tg { get; }
            public float tb { get; }
            public float rs { get; }
            public float gs { get; }
            public float bs { get; }
            public long fcnt { get; }
            public byte Type { get; }
            public byte Active { get; }
            public PHD_VECTOR rlp { get; }
            public long Range { get; }
        };

        internal struct ITEM_LIGHT
        {
            long r { get; }
            long g { get;  } 
            long b { get; }
            long ambient { get; }
            long rs { get; }
            long gs { get; }
            long bs { get; }
            long fcnt { get; }
            PCLIGHT_Array CurrentLights { get; }
            PCLIGHT_Array PrevLights { get; }
            long nCurrentLights { get; }
            long nPrevLights { get; }
            long room_number { get; }
            long RoomChange { get; }
            PHD_VECTOR item_pos { get; }
            void* pCurrentLights { get; }
            void* pPrevLights { get; }
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

        public long floor { get; }
        public ulong touch_bits { get; }
        public ulong mesh_bits { get; }
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
        public short flags { get; }
        public short shade { get; }
        public short trigger_flags { get; }
        public short carried_item { get; }
        public short after_death { get; }
        public ushort fired_weapon { get; }
        public item_flags_Array item_flags { get; }
        public void* data { get; }
        public PHD_3DPOS pos { get; }
        public ITEM_LIGHT il { get; }
        public ulong bitfield { get; }
        public ulong meshswap_meshbits { get; }
        public short draw_room { get; }
        public short TOSSPAD {  get; }

        public ItemInfo(long floor, ulong touch_bits, ulong mesh_bits, short object_number, short current_anim_state, short goal_anim_state, short required_anim_state, short anim_number, short frame_number, short room_number, short next_item, short next_active, short speed, short fallspeed, short hit_points, ushort box_number, short timer, short flags, short shade, short trigger_flags, short carried_item, short after_death, ushort fired_weapon, item_flags_Array item_flags, void* data, PHD_3DPOS pos, ITEM_LIGHT il, ulong bitfield, ulong meshswap_meshbits, short draw_room, short TOSSPAD)
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
            this.data = data;
            this.pos = pos;
            this.il = il;
            this.bitfield = bitfield;
            this.meshswap_meshbits = meshswap_meshbits;
            this.draw_room = draw_room;
            this.TOSSPAD = TOSSPAD;
        }
    }
}
