using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using CRMYonetimSistemi.Models;

namespace CRMYonetimSistemi.Documents
{
    public class ProformaDocument : IDocument
    {
        private readonly Sale _sale;

        public ProformaDocument(Sale sale)
        {
            _sale = sale;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Margin(50);
                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().Element(ComposeFooter);
                });
        }

        void ComposeHeader(IContainer container)
        {
            var titleStyle = TextStyle.Default.FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("CRMYonetimSistemi Imports").SemiBold().FontSize(24);
                });
                row.ConstantItem(150).AlignRight().Text("PROFORMA").Style(titleStyle);
            });
        }

        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(40).Column(column =>
            {
                column.Spacing(20);

                // Müşteri Bilgileri ve Fatura Detayları
                column.Item().Row(row =>
                {
                    row.RelativeItem().Border(1).Padding(10).Column(column =>
                    {
                        column.Item().Row(row =>
                        {
                            row.ConstantItem(50).Text("KİME:").SemiBold();
                            row.RelativeItem().Text(_sale.Customer!.Name);
                        });
                    });
                    row.ConstantItem(50);
                    row.RelativeItem().Border(1).Padding(10).Column(column =>
                    {
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Fatura No:").SemiBold();
                            row.RelativeItem().AlignRight().Text($"PRO-{_sale.Id:D6}");
                        });
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Tarih:").SemiBold();
                            row.RelativeItem().AlignRight().Text($"{_sale.SaleDate:dd.MM.yyyy}");
                        });
                    });
                });

                // Fatura Kalemleri Tablosu
                column.Item().Element(ComposeTable);

                // Kur Bilgisi ve Toplamlar Bölümü
                column.Item().AlignRight().Element(ComposeTotals);
            });
        }

        void ComposeTable(IContainer container)
        {
            var currencySymbol = _sale.Currency == CurrencyType.USD ? "$" : "₺";

            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Header(header =>
                {
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("ÜRÜN ADI").SemiBold();
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignRight().Text("ADET").SemiBold();
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignRight().Text($"BİRİM FİYAT ({currencySymbol})").SemiBold();
                    header.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignRight().Text($"TOPLAM TUTAR ({currencySymbol})").SemiBold();
                });

                foreach (var item in _sale.SaleItems)
                {
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(item.Product!.Name);
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text(item.Quantity.ToString());
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text($"{item.UnitPrice:N2}");
                    table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text($"{(item.Quantity * item.UnitPrice):N2}");
                }
            });
        }

        void ComposeTotals(IContainer container)
        {
            var currencySymbol = _sale.Currency == CurrencyType.USD ? "$" : "₺";

            container.Width(250).Column(column => 
            {
                if (_sale.Currency == CurrencyType.USD && _sale.ExchangeRate.HasValue)
                {
                    column.Item().PaddingBottom(5).Row(row =>
                    {
                        row.RelativeItem().Text("İşlem Kuru:").Italic();
                        row.RelativeItem().AlignRight().Text($"1 USD = {_sale.ExchangeRate.Value:N4} TL").Italic();
                    });
                }

                column.Item().BorderTop(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).Row(row =>
                {
                    row.RelativeItem().Text("ARA TOPLAM");
                    row.RelativeItem().AlignRight().Text($"{_sale.TotalAmount:N2} {currencySymbol}");
                });

                column.Item().BorderTop(1).BorderColor(Colors.Grey.Lighten2).Background(Colors.Grey.Lighten3).Padding(5).Row(row =>
                {
                    row.RelativeItem().Text("GENEL TOPLAM").Bold();
                    row.RelativeItem().AlignRight().Text($"{_sale.TotalAmount:N2} {currencySymbol}").Bold();
                });

                 if (_sale.Currency == CurrencyType.USD)
                {
                    column.Item().BorderTop(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).Row(row =>
                    {
                        row.RelativeItem().Text("GENEL TOPLAM (TL)").Bold();
                        row.RelativeItem().AlignRight().Text($"{_sale.TotalAmountInTRY:N2} ₺").Bold();
                    });
                }
            });
        }

        void ComposeFooter(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().AlignCenter().Text("BİZİMLE ÇALIŞTIĞINIZ İÇİN TEŞEKKÜR EDERİZ!").SemiBold();
                column.Item().PaddingTop(15);
                column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                column.Item().PaddingTop(10);
                column.Item().AlignCenter().Text("Sipariş ve Bilgi İçin").FontSize(10).SemiBold();
                column.Item().PaddingTop(5);
                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(contactColumn =>
                    {
                        contactColumn.Item().AlignCenter().Text("Test1").FontSize(10);
                        contactColumn.Item().AlignCenter().Text("Tel/WhatsApp: +90 123 456 78 90").FontSize(10);
                    });

                    row.RelativeItem().Column(contactColumn =>
                    {
                        contactColumn.Item().AlignCenter().Text("Test2").FontSize(10);
                        contactColumn.Item().AlignCenter().Text("Tel/WhatsApp: +90 123 456 78 90").FontSize(10);
                    });
                });
            });
        }
    }
}
