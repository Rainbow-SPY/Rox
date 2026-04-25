using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Rox.Entertainment
{
    public class NeteaseMusic_Decrypt
    {
        private const string _byte = "hzHRAmso5kInbaxW";
        private const string _Header = "CTENFDAM";

        public static string DecryptNcm(string ncmPath, string outputDir)
        {
            using (var fs = new FileStream(ncmPath, FileMode.Open, FileAccess.Read))
            {
                // 1. 检查魔术字
                var header = new byte[8];
                _ = fs.Read(header, 0, 8);

                if (Encoding.ASCII.GetString(header) != _Header)
                    throw new Exception("不是有效的 .ncm 文件");

                // 2. 读取 RC4 key（AES 加密部分）
                var keyLenBytes = new byte[4];
                _ = fs.Read(keyLenBytes, 0, 4);
                var keyLen = BitConverter.ToInt32(keyLenBytes, 0);

                var keyData = new byte[keyLen];
                _ = fs.Read(keyData, 0, keyLen);

                // XOR 0x64
                for (var i = 0; i < keyData.Length; i++)
                    keyData[i] ^= 0x64;

                // AES-ECB 解密（core_key）
                var coreKey = Encoding.ASCII.GetBytes(_byte); // 固定 16 字节
                byte[] rc4Key;
                using (var aes = Aes.Create())
                {
                    aes.Key = coreKey;
                    aes.Mode = CipherMode.ECB;
                    aes.Padding = PaddingMode.PKCS7;
                    using (var decryptor = aes.CreateDecryptor())
                    {
                        var decrypted = decryptor.TransformFinalBlock(keyData, 0, keyData.Length);
                        rc4Key = new byte[decrypted.Length - 17];
                        Array.Copy(decrypted, 17, rc4Key, 0, rc4Key.Length);
                    }
                }

                // 3. 跳过元数据（不解析也能正常解密音频）
                var metaLenBytes = new byte[4];
                _ = fs.Read(metaLenBytes, 0, 4);
                var metaLen = BitConverter.ToInt32(metaLenBytes, 0);
                fs.Seek(metaLen, SeekOrigin.Current); // 直接跳过

                // 4. 读取剩余加密音频数据
                var audioLen = fs.Length - fs.Position;
                var encryptedAudio = new byte[audioLen];
                _ = fs.Read(encryptedAudio, 0, (int)audioLen);

                // 5. RC4 解密
                var decryptedAudio = Rc4Decrypt(encryptedAudio, rc4Key);

                // 6. 判断格式并保存
                var ext = (decryptedAudio.Length > 4 &&
                           decryptedAudio[0] == 0x66 &&
                           decryptedAudio[1] == 0x4C &&
                           decryptedAudio[2] == 0x61 &&
                           decryptedAudio[3] == 0x43)
                    ? ".flac"
                    : ".mp3";

                var fileName = Path.GetFileNameWithoutExtension(ncmPath) + ext;
                var outputPath = Path.Combine(outputDir, fileName);

                File.WriteAllBytes(outputPath, decryptedAudio);
                return outputPath;
            }
        }

        // RC4 解密（加密/解密相同）
        private static byte[] Rc4Decrypt(byte[] data, byte[] key)
        {
            var S = new byte[256];
            for (var i = 0; i < 256; i++) S[i] = (byte)i;

            var j = 0;
            for (var i = 0; i < 256; i++)
            {
                j = (j + S[i] + key[i % key.Length]) % 256;
                (S[i], S[j]) = (S[j], S[i]);
            }

            var result = new byte[data.Length];
            int i1 = 0, j1 = 0;
            for (var k = 0; k < data.Length; k++)
            {
                i1 = (i1 + 1) % 256;
                j1 = (j1 + S[i1]) % 256;
                (S[i1], S[j1]) = (S[j1], S[i1]);
                var t = (byte)((S[i1] + S[j1]) % 256);
                result[k] = (byte)(data[k] ^ S[t]);
            }

            return result;
        }
    }
}