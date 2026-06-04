using PayTrack.Application.Interfaces;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Infrastructure.Services
{
    public class QRCodeService : IQrCodeService
    {
        public string GenerateQrCodeBase64(string employeeNumber)
        {
            using var qrGenerator = new QRCodeGenerator();

            var qrData = qrGenerator.CreateQrCode(
                employeeNumber,
                QRCodeGenerator.ECCLevel.Q);

            var pngQrCode = new PngByteQRCode(qrData);

            byte[] qrBytes = pngQrCode.GetGraphic(20);

            return Convert.ToBase64String(qrBytes);
        }
    }
}
