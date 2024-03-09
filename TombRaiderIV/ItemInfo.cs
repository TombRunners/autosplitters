#pragma warning disable IDE1006 // Naming Styles

// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace TR4;

public struct ItemInfo(
    int floor, uint touch_bits, uint mesh_bits, short object_number,
    short current_anim_state, short goal_anim_state, short required_anim_state, short anim_number, short frame_number,
    short room_number, short next_item, short next_active, short speed, short fallspeed, short hit_points, ushort box_number,
    short timer, ushort flags, short shade, short trigger_flags, short carried_item, short after_death, ushort fired_weapon,
    ItemInfo.item_flags_Array item_flags, uint pData, ItemInfo.PHD_3DPOS pos, ItemInfo.ITEM_LIGHT il, uint bitfield, uint meshswap_meshbits,
    short draw_room, short TOSSPAD)
{
    public struct PHD_3DPOS(int x_pos, int y_pos, int z_pos, short x_rot, short y_rot, short z_rot)
    {
        public int x_pos { get; set; } = x_pos;
        public int y_pos { get; set; } = y_pos;
        public int z_pos { get; set; } = z_pos;
        public short x_rot { get; set; } = x_rot;
        public short y_rot { get; set; } = y_rot;
        public short z_rot { get; set; } = z_rot;
    }

    public struct PHD_VECTOR(int x, int y, int z)
    {
        public int x { get; set; } = x;
        public int y { get; set; } = y;
        public int z { get; set; } = z;
    }  
        
    public struct PCLIGHT(
        float x, float y, float z, float r, float g, float b, int shadow,
        float Inner, float Outer, float InnerAngle, float OuterAngle, float Cutoff,
        float nx, float ny, float nz, int ix, int iy, int iz, int inx, int iny, int inz,
        float tr, float tg, float tb, float rs, float gs, float bs, int fcnt, byte Type, byte Active, PHD_VECTOR rlp, int Range)
    {
        public float x { get; set; } = x;
        public float y { get; set; } = y;
        public float z { get; set; } = z;
        public float r { get; set; } = r;
        public float g { get; set; } = g;
        public float b { get; set; } = b;
        public int shadow { get; set; } = shadow;
        public float Inner { get; set; } = Inner;
        public float Outer { get; set; } = Outer;
        public float InnerAngle { get; set; } = InnerAngle;
        public float OuterAngle { get; set; } = OuterAngle;
        public float Cutoff { get; set; } = Cutoff;
        public float nx { get; set; } = nx;
        public float ny { get; set; } = ny;
        public float nz { get; set; } = nz;
        public int ix { get; set; } = ix;
        public int iy { get; set; } = iy;
        public int iz { get; set; } = iz;
        public int inx { get; set; } = inx;
        public int iny { get; set; } = iny;
        public int inz { get; set; } = inz;
        public float tr { get; set; } = tr;
        public float tg { get; set; } = tg;
        public float tb { get; set; } = tb;
        public float rs { get; set; } = rs;
        public float gs { get; set; } = gs;
        public float bs { get; set; } = bs;
        public int fcnt { get; set; } = fcnt;
        public byte Type { get; set; } = Type;
        public byte Active { get; set; } = Active;
        public PHD_VECTOR rlp { get; set; } = rlp;
        public int Range { get; set; } = Range;
    }
        
    public struct ITEM_LIGHT(
        int r, int g, int b, int ambient, int rs, int gs, int bs, int fcnt,
        PCLIGHT_Array CurrentLights, PCLIGHT_Array PrevLights, int nCurrentLights, int nPrevLights,
        int room_number, int RoomChange, PHD_VECTOR item_pos, uint pCurrentLights, uint pPrevLights)
    {
        public int r { get; set; } = r;
        public int g { get;  } = g;
        public int b { get; set; } = b;
        public int ambient { get; set; } = ambient;
        public int rs { get; set; } = rs;
        public int gs { get; set; } = gs;
        public int bs { get; set; } = bs;
        public int fcnt { get; set; } = fcnt;
        public PCLIGHT_Array CurrentLights { get; set; } = CurrentLights;
        public PCLIGHT_Array PrevLights { get; set; } = PrevLights;
        public int nCurrentLights { get; set; } = nCurrentLights;
        public int nPrevLights { get; set; } = nPrevLights;
        public int room_number { get; set; } = room_number;
        public int RoomChange { get; set; } = RoomChange;
        public PHD_VECTOR item_pos { get; set; } = item_pos;
        public uint pCurrentLights { get; set; } = pCurrentLights;
        public uint pPrevLights { get; set; } = pPrevLights;
    }        

    public struct PCLIGHT_Array(
        PCLIGHT light00, PCLIGHT light01, PCLIGHT light02, PCLIGHT light03, PCLIGHT light04,
        PCLIGHT light05, PCLIGHT light06, PCLIGHT light07, PCLIGHT light08, PCLIGHT light09, PCLIGHT light10,
        PCLIGHT light11, PCLIGHT light12, PCLIGHT light13, PCLIGHT light14, PCLIGHT light15, PCLIGHT light16,
        PCLIGHT light17, PCLIGHT light18, PCLIGHT light19, PCLIGHT light20)
    {
        public PCLIGHT light00 { get; set; } = light00;
        public PCLIGHT light01 { get; set; } = light01;
        public PCLIGHT light02 { get; set; } = light02;
        public PCLIGHT light03 { get; set; } = light03;
        public PCLIGHT light04 { get; set; } = light04;
        public PCLIGHT light05 { get; set; } = light05;
        public PCLIGHT light06 { get; set; } = light06;
        public PCLIGHT light07 { get; set; } = light07;
        public PCLIGHT light08 { get; set; } = light08;
        public PCLIGHT light09 { get; set; } = light09;
        public PCLIGHT light10 { get; set; } = light10;
        public PCLIGHT light11 { get; set; } = light11;
        public PCLIGHT light12 { get; set; } = light12;
        public PCLIGHT light13 { get; set; } = light13;
        public PCLIGHT light14 { get; set; } = light14;
        public PCLIGHT light15 { get; set; } = light15;
        public PCLIGHT light16 { get; set; } = light16;
        public PCLIGHT light17 { get; set; } = light17;
        public PCLIGHT light18 { get; set; } = light18;
        public PCLIGHT light19 { get; set; } = light19;
        public PCLIGHT light20 { get; set; } = light20;
    }

    public struct item_flags_Array(short item_flag00, short item_flag01, short item_flag02, short item_flag03)
    {
        public short item_flag00 { get; set; } = item_flag00;
        public short item_flag01 { get; set; } = item_flag01;
        public short item_flag02 { get; set; } = item_flag02;
        public short item_flag03 { get; set; } = item_flag03;
    }

    public int floor { get; set; } = floor;
    public uint touch_bits { get; set; } = touch_bits;
    public uint mesh_bits { get; set; } = mesh_bits;
    public short object_number { get; set; } = object_number;
    public short current_anim_state { get; set; } = current_anim_state;
    public short goal_anim_state { get; set; } = goal_anim_state;
    public short required_anim_state { get; set; } = required_anim_state;
    public short anim_number { get; set; } = anim_number;
    public short frame_number { get; set; } = frame_number;
    public short room_number { get; set; } = room_number;
    public short next_item { get; set; } = next_item;
    public short next_active { get; set; } = next_active;
    public short speed { get; set; } = speed;
    public short fallspeed { get; set; } = fallspeed;
    public short hit_points { get; set; } = hit_points;
    public ushort box_number { get; set; } = box_number;
    public short timer { get; set; } = timer;
    public ushort flags { get; set; } = flags;
    public short shade { get; set; } = shade;
    public short trigger_flags { get; set; } = trigger_flags;
    public short carried_item { get; set; } = carried_item;
    public short after_death { get; set; } = after_death;
    public ushort fired_weapon { get; set; } = fired_weapon;
    public item_flags_Array item_flags { get; set; } = item_flags;
    public uint pData { get; set; } = pData;
    public PHD_3DPOS pos { get; set; } = pos;
    public ITEM_LIGHT il { get; set; } = il;
    public uint bitfield { get; set; } = bitfield;
    public uint meshswap_meshbits { get; set; } = meshswap_meshbits;
    public short draw_room { get; set; } = draw_room;
    public short TOSSPAD { get; set; } = TOSSPAD;
}

#pragma warning restore IDE1006 // Naming Styles