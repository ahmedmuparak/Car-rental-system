using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Application.Interfaces
{
    public interface IInvoiceService
    {
        Task<byte[]> GenerateInvoicePdfAsync(int bookingId);
        Task SendInvoiceEmailAsync(int bookingId);
    }
}
