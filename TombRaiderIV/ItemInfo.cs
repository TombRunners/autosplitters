#pragma warning disable IDE1006 // Naming Styles

// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace TR4;

public struct ItemInfo
{
    public struct PHD_3DPOS
    {
        public PHD_3DPOS(int x_pos, int y_pos, int z_pos, short x_rot, short y_rot, short z_rot)
        {
            this.x_pos = x_pos;
            this.y_pos = y_pos;
            this.z_pos = z_pos;
            this.x_rot = x_rot;
            this.y_rot = y_rot;
            this.z_rot = z_rot;
        }

        public int x_pos { get; set; }
        public int y_pos { get; set; }
        public int z_pos { get; set; }
        public short x_rot { get; set; }
        public short y_rot { get; set; }
        public short z_rot { get; set; }
    }

    public struct PHD_VECTOR
    {
        public PHD_VECTOR(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }
    }  
        
    public struct PCLIGHT
    {
        public PCLIGHT(float x, float y, float z, float r, float g, float b, int shadow,
            float Inner, float Outer, float InnerAngle, float OuterAngle, float Cutoff,
            float nx, float ny, float nz, int ix, int iy, int iz, int inx, int iny, int inz,
            float tr, float tg, float tb, float rs, float gs, float bs, int fcnt, byte Type, byte Active, PHD_VECTOR rlp, int Range)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.r = r;
            this.g = g;
            this.b = b;
            this.shadow = shadow;
            this.Inner = Inner;
            this.Outer = Outer;
            this.InnerAngle = InnerAngle;
            this.OuterAngle = OuterAngle;
            this.Cutoff = Cutoff;
            this.nx = nx;
            this.ny = ny;
            this.nz = nz;
            this.ix = ix;
            this.iy = iy;
            this.iz = iz;
            this.inx = inx;
            this.iny = iny;
            this.inz = inz;
            this.tr = tr;
            this.tg = tg;
            this.tb = tb;
            this.rs = rs;
            this.gs = gs;
            this.bs = bs;
            this.fcnt = fcnt;
            this.Type = Type;
            this.Active = Active;
            this.rlp = rlp;
            this.Range = Range;
        }

        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        public float r { get; set; }
        public float g { get; set; }
        public float b { get; set; }
        public int shadow { get; set; }
        public float Inner { get; set; }
        public float Outer { get; set; }
        public float InnerAngle { get; set; }
        public float OuterAngle { get; set; }
        public float Cutoff { get; set; }
        public float nx { get; set; }
        public float ny { get; set; }
        public float nz { get; set; }
        public int ix { get; set; }
        public int iy { get; set; }
        public int iz { get; set; }
        public int inx { get; set; }
        public int iny { get; set; }
        public int inz { get; set; }
        public float tr { get; set; }
        public float tg { get; set; }
        public float tb { get; set; }
        public float rs { get; set; }
        public float gs { get; set; }
        public float bs { get; set; }
        public int fcnt { get; set; }
        public byte Type { get; set; }
        public byte Active { get; set; }
        public PHD_VECTOR rlp { get; set; }
        public int Range { get; set; }
    }
        
    public struct ITEM_LIGHT
    {
        public ITEM_LIGHT(int r, int g, int b, int ambient, int rs, int gs, int bs, int fcnt, 
            PCLIGHT_Array CurrentLights, PCLIGHT_Array PrevLights, int nCurrentLights, int nPrevLights,
            int room_number, int RoomChange, PHD_VECTOR item_pos, uint pCurrentLights, uint pPrevLights)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.ambient = ambient;
            this.rs = rs;
            this.gs = gs;
            this.bs = bs;
            this.fcnt = fcnt;
            this.CurrentLights = CurrentLights;
            this.PrevLights = PrevLights;
            this.nCurrentLights = nCurrentLights;
            this.nPrevLights = nPrevLights;
            this.room_number = room_number;
            this.RoomChange = RoomChange;
            this.item_pos = item_pos;
            this.pCurrentLights = pCurrentLights;
            this.pPrevLights = pPrevLights;
        }

        public int r { get; set; }
        public int g { get;  } 
        public int b { get; set; }
        public int ambient { get; set; }
        public int rs { get; set; }
        public int gs { get; set; }
        public int bs { get; set; }
        public int fcnt { get; set; }
        public PCLIGHT_Array CurrentLights { get; set; }
        public PCLIGHT_Array PrevLights { get; set; }
        public int nCurrentLights { get; set; }
        public int nPrevLights { get; set; }
        public int room_number { get; set; }
        public int RoomChange { get; set; }
        public PHD_VECTOR item_pos { get; set; }
        public uint pCurrentLights { get; set; }
        public uint pPrevLights { get; set; }
    }        

    public struct PCLIGHT_Array
    {
        public PCLIGHT_Array(PCLIGHT light00, PCLIGHT light01, PCLIGHT light02, PCLIGHT light03, PCLIGHT light04,
            PCLIGHT light05, PCLIGHT light06, PCLIGHT light07, PCLIGHT light08, PCLIGHT light09, PCLIGHT light10,
            PCLIGHT light11, PCLIGHT light12, PCLIGHT light13, PCLIGHT light14, PCLIGHT light15, PCLIGHT light16,
            PCLIGHT light17, PCLIGHT light18, PCLIGHT light19, PCLIGHT light20)
        {
            this.light00 = light00;
            this.light01 = light01;
            this.light02 = light02;
            this.light03 = light03;
            this.light04 = light04;
            this.light05 = light05;
            this.light06 = light06;
            this.light07 = light07;
            this.light08 = light08;
            this.light09 = light09;
            this.light10 = light10;
            this.light11 = light11;
            this.light12 = light12;
            this.light13 = light13;
            this.light14 = light14;
            this.light15 = light15;
            this.light16 = light16;
            this.light17 = light17;
            this.light18 = light18;
            this.light19 = light19;
            this.light20 = light20;
        }

        public PCLIGHT light00 { get; set; }
        public PCLIGHT light01 { get; set; }
        public PCLIGHT light02 { get; set; }
        public PCLIGHT light03 { get; set; }
        public PCLIGHT light04 { get; set; }
        public PCLIGHT light05 { get; set; }
        public PCLIGHT light06 { get; set; }
        public PCLIGHT light07 { get; set; }
        public PCLIGHT light08 { get; set; }
        public PCLIGHT light09 { get; set; }
        public PCLIGHT light10 { get; set; }
        public PCLIGHT light11 { get; set; }
        public PCLIGHT light12 { get; set; }
        public PCLIGHT light13 { get; set; }
        public PCLIGHT light14 { get; set; }
        public PCLIGHT light15 { get; set; }
        public PCLIGHT light16 { get; set; }
        public PCLIGHT light17 { get; set; }
        public PCLIGHT light18 { get; set; }
        public PCLIGHT light19 { get; set; }
        public PCLIGHT light20 { get; set; }
    }

    public struct item_flags_Array
    {
        public item_flags_Array(short item_flag00, short item_flag01, short item_flag02, short item_flag03)
        {
            this.item_flag00 = item_flag00;
            this.item_flag01 = item_flag01;
            this.item_flag02 = item_flag02;
            this.item_flag03 = item_flag03;
        }

        public short item_flag00 { get; set; }
        public short item_flag01 { get; set; }
        public short item_flag02 { get; set; }
        public short item_flag03 { get; set; }
    }

    public int floor { get; set; }
    public uint touch_bits { get; set; }
    public uint mesh_bits { get; set; }
    public short object_number { get; set; }
    public short current_anim_state { get; set; }
    public short goal_anim_state { get; set; }
    public short required_anim_state { get; set; }
    public short anim_number { get; set; }
    public short frame_number { get; set; }
    public short room_number { get; set; }
    public short next_item { get; set; }
    public short next_active { get; set; }
    public short speed { get; set; }
    public short fallspeed { get; set; }
    public short hit_points { get; set; }
    public ushort box_number { get; set; }
    public short timer { get; set; }
    public ushort flags { get; set; }
    public short shade { get; set; }
    public short trigger_flags { get; set; }
    public short carried_item { get; set; }

    public short after_death { get; set; }

    public ushort fired_weapon { get; set; }
    public item_flags_Array item_flags { get; set; }
    public uint pData { get; set; }
    public PHD_3DPOS pos { get; set; }
    public ITEM_LIGHT il { get; set; }
    public uint bitfield { get; set; }
    public uint meshswap_meshbits { get; set; }
    public short draw_room { get; set; }
    public short TOSSPAD { get; set; }

    public ItemInfo(int floor, uint touch_bits, uint mesh_bits, short object_number, 
        short current_anim_state, short goal_anim_state, short required_anim_state, short anim_number, short frame_number,
        short room_number, short next_item, short next_active, short speed, short fallspeed, short hit_points, ushort box_number,
        short timer, ushort flags, short shade, short trigger_flags, short carried_item, short after_death, ushort fired_weapon,
        item_flags_Array item_flags, uint pData, PHD_3DPOS pos, ITEM_LIGHT il, uint bitfield, uint meshswap_meshbits, short draw_room, short TOSSPAD)
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

#pragma warning restore IDE1006 // Naming Styles