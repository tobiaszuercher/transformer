using System.Security.Cryptography;

namespace Transformer.Cryptography
{
    /// <summary>
    /// Code adapted from https://stackoverflow.com/questions/19298801/generating-random-string-using-rngcryptoserviceprovider
    /// </summary>
    public static class RandomStringProvider
    {
        private static readonly char[] AvailableCharacters =
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
            'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
        };

        private static readonly RNGCryptoServiceProvider RngCryptoServiceProvider = new RNGCryptoServiceProvider();

        public static string Provide(int length)
        {
            var identifier = new char[length];
            var randomData = new byte[length];

            RngCryptoServiceProvider.GetBytes(randomData);

            for (var idx = 0; idx < identifier.Length; idx++)
            {
                var pos = randomData[idx] % AvailableCharacters.Length;
                identifier[idx] = AvailableCharacters[pos];
            }

            return new string(identifier);
        }
    }
}