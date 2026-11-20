using System.Globalization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CarRental.Infrastructure.Invoices
{
    public class InvoiceDocument : IDocument
    {
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;
        private static readonly string Accent = Colors.Blue.Darken3;

        private readonly InvoiceModel model;

        static InvoiceDocument()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public InvoiceDocument(InvoiceModel model)
        {
            this.model = model;
        }

        public DocumentMetadata GetMetadata() => new()
        {
            Title = $"Invoice {model.InvoiceNumber}",
            Author = "Car Rental"
        };

        public byte[] ToPdf() => this.GeneratePdf();

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Element(ComposeHeader);
                page.Content().PaddingVertical(20).Element(ComposeContent);
                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Thank you for choosing Car Rental  |  Page ")
                        .FontColor(Colors.Grey.Medium);
                    text.CurrentPageNumber().FontColor(Colors.Grey.Medium);
                });
            });
        }

        private void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("Car Rental").FontSize(22).Bold().FontColor(Accent);
                    column.Item().Text("Booking Invoice").FontSize(12).FontColor(Colors.Grey.Darken1);
                });

                row.ConstantItem(180).Column(column =>
                {
                    column.Item().AlignRight().Text($"Invoice {model.InvoiceNumber}").Bold().FontSize(12);
                    column.Item().AlignRight().Text($"Issued: {Date(model.IssuedAt)}");
                    column.Item().AlignRight().Text($"Payment: {model.PaymentStatus}")
                        .Bold().FontColor(model.PaymentStatus == "Paid" ? Colors.Green.Darken2 : Colors.Orange.Darken2);
                });
            });
        }

        private void ComposeContent(IContainer container)
        {
            container.Column(column =>
            {
                column.Spacing(18);

                column.Item().Row(row =>
                {
                    row.RelativeItem().Element(c => Block(c, "Billed to", new[]
                    {
                        model.CustomerName,
                        model.CustomerEmail,
                        model.CustomerPhone ?? string.Empty
                    }));

                    row.RelativeItem().Element(c => Block(c, "Vehicle", new[]
                    {
                        model.CarName,
                        $"Year {model.CarYear}, {model.CarColor}"
                    }));
                });

                column.Item().Row(row =>
                {
                    row.RelativeItem().Element(c => Block(c, "Pickup", new[]
                    {
                        model.PickupBranch,
                        Date(model.StartDate)
                    }));

                    row.RelativeItem().Element(c => Block(c, "Return", new[]
                    {
                        model.ReturnBranch,
                        Date(model.EndDate)
                    }));
                });

                column.Item().Element(ComposeTable);
                column.Item().AlignRight().Element(ComposeTotals);

                column.Item().Text($"Booking status: {model.BookingStatus}")
                    .FontColor(Colors.Grey.Darken1);
            });
        }

        private static void Block(IContainer container, string title, IEnumerable<string> lines)
        {
            container.Column(column =>
            {
                column.Item().PaddingBottom(3).Text(title).Bold().FontColor(Accent);

                foreach (var line in lines.Where(l => !string.IsNullOrWhiteSpace(l)))
                {
                    column.Item().Text(line);
                }
            });
        }

        private void ComposeTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(4);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    header.Cell().Element(HeaderCell).Text("Description");
                    header.Cell().Element(HeaderCell).AlignRight().Text("Days");
                    header.Cell().Element(HeaderCell).AlignRight().Text("Price / day");
                    header.Cell().Element(HeaderCell).AlignRight().Text("Amount");
                });

                table.Cell().Element(BodyCell)
                    .Text($"{model.CarName} rental, {Date(model.StartDate)} to {Date(model.EndDate)}");
                table.Cell().Element(BodyCell).AlignRight().Text(model.TotalDays.ToString(Culture));
                table.Cell().Element(BodyCell).AlignRight().Text(Money(model.PricePerDay));
                table.Cell().Element(BodyCell).AlignRight().Text(Money(model.PricePerDay * model.TotalDays));
            });
        }

        private void ComposeTotals(IContainer container)
        {
            container.Width(220).Column(column =>
            {
                column.Item().Row(row =>
                {
                    row.RelativeItem().Text("Subtotal");
                    row.ConstantItem(90).AlignRight().Text(Money(model.PricePerDay * model.TotalDays));
                });

                column.Item().Row(row =>
                {
                    row.RelativeItem().Text("Discount");
                    row.ConstantItem(90).AlignRight().Text($"- {Money(model.Discount)}");
                });

                column.Item().PaddingTop(4).BorderTop(1).BorderColor(Colors.Grey.Lighten1)
                    .PaddingTop(4).Row(row =>
                    {
                        row.RelativeItem().Text("Total").Bold().FontSize(12);
                        row.ConstantItem(90).AlignRight().Text(Money(model.TotalPrice)).Bold().FontSize(12);
                    });
            });
        }

        private static IContainer HeaderCell(IContainer container) =>
            container
                .BorderBottom(1).BorderColor(Accent)
                .PaddingVertical(5)
                .DefaultTextStyle(x => x.SemiBold().FontColor(Accent));

        private static IContainer BodyCell(IContainer container) =>
            container
                .BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                .PaddingVertical(6);

        private static string Date(DateTime value) =>
            value.ToString("dd MMM yyyy", Culture);

        private string Money(decimal value) =>
            $"{value.ToString("N2", Culture)} {model.Currency}";
    }
}