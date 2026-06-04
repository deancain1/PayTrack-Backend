using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTrack.Application.Interfaces
{
    public interface IQrCodeService
    {
        string GenerateQrCodeBase64(string employeeNumber);
    }
}
