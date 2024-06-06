using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ZXing.QrCode.Internal;

namespace TaskSe.Services.Utilidades
{
    public class QRGenerator
    {
        public static bool generarQR(string contenido, string ruta_final, int size)
        {
            bool resultado = false;

            try
            {
                var encOptions = new ZXing.Common.EncodingOptions
                {
                    Width = size,
                    Height = size,
                    Margin = 0,
                    PureBarcode = false
                };

                encOptions.Hints.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H);


                var barcodeWriter = new ZXing.BarcodeWriterPixelData
                {
                    Format = ZXing.BarcodeFormat.QR_CODE,
                    Options = encOptions
                };

                Bitmap overlay = new Bitmap(Directory.GetCurrentDirectory() + "\\wwwroot\\images\\qrlogo.png");

                // Generate the QR code image from the input text
                var pixelData = barcodeWriter.Write(contenido);
                using var bitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppArgb);
                using var ms = new MemoryStream(pixelData.Pixels);
                BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, pixelData.Width, pixelData.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                try
                {
                    Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                }
                finally
                {
                    bitmap.UnlockBits(bitmapData);
                }

                Bitmap bp = new Bitmap(bitmap);

                int deltaHeigth = bp.Height - overlay.Height;
                int deltaWidth = bp.Width - overlay.Width;

                Graphics g = Graphics.FromImage(bp);
                g.DrawImage(overlay, new Point(deltaWidth / 2, deltaHeigth / 2));

                bp.Save(ruta_final);

                if (File.Exists(ruta_final))
                {
                    return true;
                }
            }
            catch (Exception exe)
            {
                Serilog.Log.Error(exe, "Error al generar QR");
            }
            finally
            {

            }

            return resultado;
        }

        public static bool generarQRNoLogo(string contenido, string ruta_final, int size)
        {
            bool resultado = false;

            try
            {
                var encOptions = new ZXing.Common.EncodingOptions
                {
                    Width = size,
                    Height = size,
                    Margin = 0,
                    PureBarcode = false
                };

                encOptions.Hints.Add(EncodeHintType.ERROR_CORRECTION, ErrorCorrectionLevel.H);


                var barcodeWriter = new ZXing.BarcodeWriterPixelData
                {
                    Format = ZXing.BarcodeFormat.QR_CODE,
                    Options = encOptions
                };


                // Generate the QR code image from the input text
                var pixelData = barcodeWriter.Write(contenido);
            using var bitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppArgb);
            using var ms = new MemoryStream(pixelData.Pixels);
                BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, pixelData.Width, pixelData.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                try
                {
                    Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
                }
                finally
                {
                    bitmap.UnlockBits(bitmapData);
                }

                Bitmap bp = new Bitmap(bitmap);

                bp.Save(ruta_final);

                if (File.Exists(ruta_final))
                {
                    return true;
                }
            }
            catch (Exception exe)
            {
                Serilog.Log.Error(exe, "Error al generar QR");
            }
            finally
            {

            }

            return resultado;
        }
    }
}
