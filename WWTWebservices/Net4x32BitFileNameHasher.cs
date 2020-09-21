using System;

namespace WWTWebservices
{
    public class Net4x32BitFileNameHasher : IFileNameHasher
    {
        /// <summary>
        /// Implementation to mimix string.GetHashCode() of 32-bit .NET 4.x. Was used originally for hashing to get id
        /// </summary>
        /// <param name="s">Input string</param>
        /// <returns>Stable hash</returns>
        public int HashName(string s)
            => Math.Abs(GetHashCode32BitNet4x(s));

        private static unsafe int GetHashCode32BitNet4x(string s)
        {
            fixed (char* str = s.ToCharArray())
            {
                char* chPtr = str;
                int num = 0x15051505;
                int num2 = num;
                int* numPtr = (int*)chPtr;

                for (int i = s.Length; i > 0; i -= 4)
                {
                    num = (((num << 5) + num) + (num >> 0x1b)) ^ numPtr[0];
                    if (i <= 2)
                    {
                        break;
                    }
                    num2 = (((num2 << 5) + num2) + (num2 >> 0x1b)) ^ numPtr[1];
                    numPtr += 2;
                }

                return (num + (num2 * 0x5d588b65));
            }
        }
    }
}
