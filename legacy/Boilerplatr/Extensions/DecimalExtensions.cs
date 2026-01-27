namespace Boilerplatr.Extensions;

public static class DecimalExtensions
{
    public static byte[] ToByteArray(this decimal value)
    {
        int[] bits = decimal.GetBits(value);
        byte[] bytes = new byte[16];

        for (int i = 0; i < bits.Length; i++)
        {
            Buffer.BlockCopy(BitConverter.GetBytes(bits[i]), 0, bytes, i * 4, 4);
        }

        return bytes;
    }
}