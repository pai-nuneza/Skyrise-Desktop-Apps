using System;
using System.Collections.Generic;
using System.Text;

namespace CommonPostingLibrary
{
    public static class Encrypt
    {
        //The following variables are used in initialization of the Encryption method
        private static string passPhrase = "Pas5pr@se"; // can be any string
        private static string saltValue = "s@1tValue"; // can be any string
        private static string hashAlgorithm = "SHA1"; // can be "MD5"
        private static int passwordIterations = 2; // can be any number
        private static string initVector = "@1B2c3D4e5F6g7H8"; // must be 16 bytes
        private static int keySize = 256; // can be 192 or 128
        public static string gridKey = "";

        public static string encryptText(String Text)
        {
            string plainText = Text; // original plaintext
            return Encryption.Encrypt(Text,
            passPhrase,
            saltValue,
            hashAlgorithm,
            passwordIterations,
            initVector,
            keySize);
        }

        public static string decryptText(String Text)
        {
            string plainText = Text; // original plaintext
            return Encryption.Decrypt(Text,
            passPhrase,
            saltValue,
            hashAlgorithm,
            passwordIterations,
            initVector,
            keySize);
        }
    }
}
