namespace MailService.API.Services
{
    public class PdfService : IPdfService
    {
        public async Task<byte[]> GenerateTwoFactorPdf(string fullName, string code)
        {
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "TwoFactorTemplate.html");

            if (!File.Exists(templatePath))
                throw new FileNotFoundException("HTML template not found", templatePath);


            var html = await File.ReadAllTextAsync(templatePath);


            html = html.Replace("@Model.FullName", fullName)
                       .Replace("@Model.Code", code)
                       .Replace("@DateTime.Now.Year", DateTime.Now.Year.ToString());


            await using var pdfStream = new MemoryStream();


            HtmlConverter.ConvertToPdf(html, pdfStream);


            return pdfStream.ToArray();
        }
    }
}
