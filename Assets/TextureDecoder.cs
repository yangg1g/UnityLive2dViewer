using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Texture2DDecoder
{
    unsafe partial class TextureDecoder
    {

        [DllImport("Texture2DDecoderNative.dll", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DecodeDXT1(void* data, int width, int height, void* image);

        [DllImport("Texture2DDecoderNative.dll", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DecodeDXT5(void* data, int width, int height, void* image);

        [DllImport("Texture2DDecoderNative.dll", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DecodePVRTC(void* data, int width, int height, void* image, [MarshalAs(UnmanagedType.Bool)] bool is2bpp);

        [DllImport("Texture2DDecoderNative.dll", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DecodeETC1(void* data, int width, int height, void* image);

        [DllImport("Texture2DDecoderNative.dll", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DecodeETC2(void* data, int width, int height, void* image);

        [DllImport("Texture2DDecoderNative.dll", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DecodeETC2A1(void* data, int width, int height, void* image);

        [DllImport("Texture2DDecoderNative.dll", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DecodeETC2A8(void* data, int width, int height, void* image);

        [DllImport("Texture2DDecoderNative.dll", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DecodeEACR(void* data, int width, int height, void* image);

        [DllImport("Texture2DDecoderNative.dll", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DecodeEACRSigned(void* data, int width, int height, void* image);

        [DllImport("Texture2DDecoderNative.dll", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DecodeEACRG(void* data, int width, int height, void* image);

        [DllImport("Texture2DDecoderNative.dll", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DecodeEACRGSigned(void* data, int width, int height, void* image);

        [DllImport("Texture2DDecoderNative.dll", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DecodeBC4(void* data, int width, int height, void* image);

        [DllImport("Texture2DDecoderNative.dll", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DecodeBC5(void* data, int width, int height, void* image);

        [DllImport("Texture2DDecoderNative.dll", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DecodeBC6(void* data, int width, int height, void* image);

        [DllImport("Texture2DDecoderNative.dll", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DecodeBC7(void* data, int width, int height, void* image);

        [DllImport("Texture2DDecoderNative.dll", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DecodeATCRGB4(void* data, int width, int height, void* image);

        [DllImport("Texture2DDecoderNative.dll", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DecodeATCRGBA8(void* data, int width, int height, void* image);

        [DllImport("Texture2DDecoderNative.dll", CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DecodeASTC(void* data, int width, int height, int blockWidth, int blockHeight, void* image);

        [DllImport("Texture2DDecoderNative.dll", CallingConvention = CallingConvention.Winapi)]
        private static extern void DisposeBuffer(ref void* ppBuffer);

        [DllImport("Texture2DDecoderNative.dll", CallingConvention = CallingConvention.Winapi)]
        private static extern void UnpackCrunch(void* data, uint dataSize, out void* result, out uint resultSize);

        [DllImport("Texture2DDecoderNative.dll", CallingConvention = CallingConvention.Winapi)]
        private static extern void UnpackUnityCrunch(void* data, uint dataSize, out void* result, out uint resultSize);

    }

    public static unsafe partial class TextureDecoder
    {

        public static bool DecodeDXT1(byte[] data, int width, int height, byte[] image)
        {
            fixed (byte* pData = data)
            {
                fixed (byte* pImage = image)
                {
                    return DecodeDXT1(pData, width, height, pImage);
                }
            }
        }

        public static bool DecodeDXT5(byte[] data, int width, int height, byte[] image)
        {
            fixed (byte* pData = data)
            {
                fixed (byte* pImage = image)
                {
                    return DecodeDXT5(pData, width, height, pImage);
                }
            }
        }

        public static bool DecodePVRTC(byte[] data, int width, int height, byte[] image, bool is2bpp)
        {
            fixed (byte* pData = data)
            {
                fixed (byte* pImage = image)
                {
                    return DecodePVRTC(pData, width, height, pImage, is2bpp);
                }
            }
        }

        public static bool DecodeETC1(byte[] data, int width, int height, byte[] image)
        {
            fixed (byte* pData = data)
            {
                fixed (byte* pImage = image)
                {
                    return DecodeETC1(pData, width, height, pImage);
                }
            }
        }

        public static bool DecodeETC2(byte[] data, int width, int height, byte[] image)
        {
            fixed (byte* pData = data)
            {
                fixed (byte* pImage = image)
                {
                    return DecodeETC2(pData, width, height, pImage);
                }
            }
        }

        public static bool DecodeETC2A1(byte[] data, int width, int height, byte[] image)
        {
            fixed (byte* pData = data)
            {
                fixed (byte* pImage = image)
                {
                    return DecodeETC2A1(pData, width, height, pImage);
                }
            }
        }

        public static bool DecodeETC2A8(byte[] data, int width, int height, byte[] image)
        {
            fixed (byte* pData = data)
            {
                fixed (byte* pImage = image)
                {
                    return DecodeETC2A8(pData, width, height, pImage);
                }
            }
        }

        public static bool DecodeEACR(byte[] data, int width, int height, byte[] image)
        {
            fixed (byte* pData = data)
            {
                fixed (byte* pImage = image)
                {
                    return DecodeEACR(pData, width, height, pImage);
                }
            }
        }

        public static bool DecodeEACRSigned(byte[] data, int width, int height, byte[] image)
        {
            fixed (byte* pData = data)
            {
                fixed (byte* pImage = image)
                {
                    return DecodeEACRSigned(pData, width, height, pImage);
                }
            }
        }

        public static bool DecodeEACRG(byte[] data, int width, int height, byte[] image)
        {
            fixed (byte* pData = data)
            {
                fixed (byte* pImage = image)
                {
                    return DecodeEACRG(pData, width, height, pImage);
                }
            }
        }

        public static bool DecodeEACRGSigned(byte[] data, int width, int height, byte[] image)
        {
            fixed (byte* pData = data)
            {
                fixed (byte* pImage = image)
                {
                    return DecodeEACRGSigned(pData, width, height, pImage);
                }
            }
        }

        public static bool DecodeBC4(byte[] data, int width, int height, byte[] image)
        {
            fixed (byte* pData = data)
            {
                fixed (byte* pImage = image)
                {
                    return DecodeBC4(pData, width, height, pImage);
                }
            }
        }

        public static bool DecodeBC5(byte[] data, int width, int height, byte[] image)
        {
            fixed (byte* pData = data)
            {
                fixed (byte* pImage = image)
                {
                    return DecodeBC5(pData, width, height, pImage);
                }
            }
        }

        public static bool DecodeBC6(byte[] data, int width, int height, byte[] image)
        {
            fixed (byte* pData = data)
            {
                fixed (byte* pImage = image)
                {
                    return DecodeBC6(pData, width, height, pImage);
                }
            }
        }

        public static bool DecodeBC7(byte[] data, int width, int height, byte[] image)
        {
            fixed (byte* pData = data)
            {
                fixed (byte* pImage = image)
                {
                    return DecodeBC7(pData, width, height, pImage);
                }
            }
        }

        public static bool DecodeATCRGB4(byte[] data, int width, int height, byte[] image)
        {
            fixed (byte* pData = data)
            {
                fixed (byte* pImage = image)
                {
                    return DecodeATCRGB4(pData, width, height, pImage);
                }
            }
        }

        public static bool DecodeATCRGBA8(byte[] data, int width, int height, byte[] image)
        {
            fixed (byte* pData = data)
            {
                fixed (byte* pImage = image)
                {
                    return DecodeATCRGBA8(pData, width, height, pImage);
                }
            }
        }

        public static bool DecodeASTC(byte[] data, int width, int height, int blockWidth, int blockHeight, byte[] image)
        {
            fixed (byte* pData = data)
            {
                fixed (byte* pImage = image)
                {
                    return DecodeASTC(pData, width, height, blockWidth, blockHeight, pImage);
                }
            }
        }

        public static byte[] UnpackCrunch(byte[] data)
        {
            void* pBuffer;
            uint bufferSize;

            fixed (byte* pData = data)
            {
                UnpackCrunch(pData, (uint)data.Length, out pBuffer, out bufferSize);
            }

            if (pBuffer == null)
            {
                return null;
            }

            var result = new byte[bufferSize];

            Marshal.Copy(new IntPtr(pBuffer), result, 0, (int)bufferSize);

            DisposeBuffer(ref pBuffer);

            return result;
        }

        public static byte[] UnpackUnityCrunch(byte[] data)
        {
            void* pBuffer;
            uint bufferSize;

            fixed (byte* pData = data)
            {
                UnpackUnityCrunch(pData, (uint)data.Length, out pBuffer, out bufferSize);
            }

            if (pBuffer == null)
            {
                return null;
            }

            var result = new byte[bufferSize];

            Marshal.Copy(new IntPtr(pBuffer), result, 0, (int)bufferSize);

            DisposeBuffer(ref pBuffer);

            return result;
        }

    }
}