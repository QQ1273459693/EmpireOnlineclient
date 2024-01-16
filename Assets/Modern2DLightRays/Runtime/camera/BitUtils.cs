namespace LightRays2D
{

    public static class BitUtils
    {
        public static bool IsBitSet(int bitmask, int idx)
        {
            if ((bitmask & (1 << idx)) > 0) return true;
            else return false;
        }

        public static int SetBit(int bitmask, int idx)
        {
            return bitmask |= (1 << idx);
        }

        public static int RemoveBit(int bitmask, int idx)
        {
            return bitmask &= ~(1 << idx);
        }
    }

}