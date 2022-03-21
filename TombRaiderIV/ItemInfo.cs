namespace TR4
{
    internal unsafe struct ItemInfo
    {
        internal struct PHD_3DPOS
        {
	        long x_pos { get; }
	        long y_pos { get; }
	        long z_pos { get; }
	        short x_rot { get; }
	        short y_rot { get; }
	        short z_rot { get; }
        };
        
		internal struct PHD_VECTOR
		{
			long x { get; }
			long y { get; }
			long z { get; }
		};				
		
        internal struct PCLIGHT
		{
			float x { get; }
			float y { get; }
			float z { get; }
			float r { get; }
			float g { get; }
			float b { get; }
			long shadow { get; }
			float Inner { get; }
			float Outer { get; }
			float InnerAngle { get; }
			float OuterAngle { get; }
			float Cutoff { get; }
			float nx { get; }
			float ny { get; }
			float nz { get; }
			long ix { get; }
			long iy { get; }
			long iz { get; }
			long inx { get; }
			long iny { get; }
			long inz { get; }
			float tr { get; }
			float tg { get; }
			float tb { get; }
			float rs { get; }
			float gs { get; }
			float bs { get; }
			long fcnt { get; }
			byte Type { get; }
			byte Active { get; }
			PHD_VECTOR rlp { get; }
			long Range { get; }
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
			PCLIGHT light00 { get; }
			PCLIGHT light01 { get; }
			PCLIGHT light02 { get; }
			PCLIGHT light03 { get; }
			PCLIGHT light04 { get; }
			PCLIGHT light05 { get; }
			PCLIGHT light06 { get; }
			PCLIGHT light07 { get; }
			PCLIGHT light08 { get; }
			PCLIGHT light09 { get; }
			PCLIGHT light10 { get; }
			PCLIGHT light11 { get; }
			PCLIGHT light12 { get; }
			PCLIGHT light13 { get; }
			PCLIGHT light14 { get; }
			PCLIGHT light15 { get; }
			PCLIGHT light16 { get; }
			PCLIGHT light17 { get; }
			PCLIGHT light18 { get; }
			PCLIGHT light19 { get; }
			PCLIGHT light20 { get; }
        }

		internal struct item_flags_Array
        {
			short item_flag00 { get; }
			short item_flag01 { get; }
			short item_flag02 { get; }
			short item_flag03 { get; }
        }

        long floor { get; }
        ulong touch_bits { get; }
        ulong mesh_bits { get; }
        short object_number { get; }
        short current_anim_state { get; }
        short goal_anim_state { get; }
        short required_anim_state { get; }
        short anim_number { get; }
        short frame_number { get; }
        short room_number { get; }
        short next_item { get; }
        short next_active { get; }
        short speed { get; }
        short fallspeed { get; }
        short hit_points { get; }
        ushort box_number { get; }
        short timer { get; }
        short flags { get; }
        short shade { get; }
        short trigger_flags { get; }
        short carried_item { get; }
        short after_death { get; }
        ushort fired_weapon { get; }
        item_flags_Array item_flags { get; }
        void* data { get; }
        PHD_3DPOS pos { get; }
		ITEM_LIGHT il { get; }
		ulong bitfield { get; }
		ulong meshswap_meshbits { get; }
		short draw_room { get; }
		short TOSSPAD {  get; }

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
