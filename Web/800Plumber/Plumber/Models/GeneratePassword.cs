using System;
using System.Security.Cryptography;
namespace Security
{
    public class GeneratePassword
    {
        private static int _defaultMinPasswordLength = 6;
        private static int _defaultMaxPasswordLength = 12;

        private static string _lowerCase = "abcdefghijklmnopqrstuvwxyz";
        private static string _upperCase = "ABCDEFGHIJKLMNPQRSTUVWXYZ";
        private static string _numeric = "123456789";
        private static string _characters = "&-_=+$%*?/!";

        public static string generatePassword()
        {
            Random passwordLengthRandom = new Random();
            var passwordLength = passwordLengthRandom.Next(_defaultMinPasswordLength, _defaultMaxPasswordLength);

            char[][] chars = new char[][] 
        {
            _lowerCase.ToCharArray(),
            _upperCase.ToCharArray(),
            _numeric.ToCharArray(),
            _characters.ToCharArray()
        };

            int[] charactersLeftInGroup = new int[chars.Length];

            for (int i = 0; i < chars.Length; i++)
                charactersLeftInGroup[i] = chars[i].Length;

            int[] groups = new int[chars.Length];

            for (int i = 0; i < groups.Length; i++)
                groups[i] = i;

            byte[] randomByte = new byte[4];

            RNGCryptoServiceProvider rngProvider = new RNGCryptoServiceProvider();

            rngProvider.GetBytes(randomByte);

            int seed = (randomByte[0] & 0x7f) << 24 |
                        randomByte[1] << 16 |
                        randomByte[2] << 8 |
                        randomByte[3];

            Random random = new Random(seed);

            char[] password = new char[passwordLength];

            // Index values
            int nextChar; // Character to add
            int nextGroup; // Character group
            int nextLeftGroup;
            int lastChar;
            int lastLeftGroup;

            lastLeftGroup = groups.Length - 1;

            for (int i = 0; i < password.Length; i++)
            {
                if (lastLeftGroup == 0)
                    nextLeftGroup = 0;
                else
                    nextLeftGroup = random.Next(0, lastLeftGroup);

                nextGroup = groups[nextLeftGroup];

                lastChar = charactersLeftInGroup[nextGroup] - 1;

                if (lastChar == 0)
                    nextChar = 0;
                else
                    nextChar = random.Next(0, lastChar + 1);

                password[i] = chars[nextGroup][nextChar];

                if (lastChar == 0)
                    charactersLeftInGroup[nextGroup] = chars[nextGroup].Length;
                else
                {
                    if (lastChar != nextChar)
                    {
                        char temp = chars[nextGroup][lastChar];
                        chars[nextGroup][lastChar] = chars[nextGroup][nextChar];
                        chars[nextGroup][nextChar] = temp;
                    }
                    charactersLeftInGroup[nextGroup]--;
                }

                if (lastLeftGroup == 0)
                    lastLeftGroup = groups.Length - 1;
                else
                {
                    if (lastLeftGroup != nextLeftGroup)
                    {
                        int temp = groups[lastLeftGroup];
                        groups[lastLeftGroup] =
                                    groups[nextLeftGroup];
                        groups[nextLeftGroup] = temp;
                    }
                    lastLeftGroup--;
                }
            }

            return new string(password);
        }

    }
}