using PayTrack.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Infrastructure.Repository
{
    public class PhotoStorageService : IPhotoStorageService
    {
        public async Task<string> SaveAttendancePhotoAsync(string photoBase64)
        {
            if (string.IsNullOrWhiteSpace(photoBase64))
                throw new Exception("Photo is empty");

            // support data URL OR raw base64
            var base64Data = photoBase64.Contains(",")
                ? photoBase64.Split(',')[1]
                : photoBase64;

            byte[] bytes;

            try
            {
                bytes = Convert.FromBase64String(base64Data);
            }
            catch (FormatException ex)
            {
                throw new Exception("Invalid base64 image format", ex);
            }

            var fileName = $"{Guid.NewGuid()}.jpg";

            var folder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "AttendancePhotos"
            );

            Directory.CreateDirectory(folder);

            var path = Path.Combine(folder, fileName);

            await File.WriteAllBytesAsync(path, bytes);

            return Path.Combine("AttendancePhotos", fileName)
                .Replace("\\", "/");
        }
    }
}
