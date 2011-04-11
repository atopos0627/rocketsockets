namespace rocketsockets
{
    public unsafe struct fd_set
    {
        public const int FD_SETSIZE = 64;
        public uint fd_count;
        public fixed uint fd_array[FD_SETSIZE];
    }
}