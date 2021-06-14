using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Security.Cryptography;

namespace MATE
{
    public abstract class Cryipt : IDisposable, System.Security.Cryptography.ICryptoTransform
    {
        public abstract int InputBlockSize { get; }
        public abstract int OutputBlockSize { get; }
        public abstract bool CanTransformMultipleBlocks { get; }
        public abstract bool CanReuseTransform { get; }

        public abstract void Dispose();
        public abstract int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset);
        public abstract byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount);
    }

    public class Md5Crypt
    {
        public string CalculateHash(string password)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            md5.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(password));

            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();

            for(int i = 0; i < result.Length; i++)
            {
                strBuilder.Append(result[i].ToString("x2"));
            }
            return strBuilder.ToString();
        }
    }
}