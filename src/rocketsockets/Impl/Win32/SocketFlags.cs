namespace rocketsockets
{
    public enum SocketFlags : int
    {
        Overlapped = 0x01,
        Multipoint_C_Root = 0x02,
        Multipoint_C_Leaf = 0x04,
        Multipoint_D_Root = 0x08,
        Multipoint_D_Leaf = 0x10,
        Access_System_Security = 0x40,
        No_Handle_Inherit = 0x80
    }
}