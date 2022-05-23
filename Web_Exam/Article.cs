using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace Web_Exam
{
    public class Article
    {
        public string title;
        public string author;
        public string date;
        public string rate;
        public string countOfViews;
        public string description;

        public Article(string title, string author, string date, string rate, string countOfViews, string description)
        {
            this.title = title;
            this.author = author;
            this.date = date;
            this.rate = rate;
            this.countOfViews = countOfViews;
            this.description = description;
        }

        public static void printArticle(Article article)
        {
            Console.WriteLine(article.author + "\t" + article.date);
            Console.WriteLine(article.title);
            Console.WriteLine(article.description);
            Console.WriteLine(article.rate + "\t" + article.countOfViews);

            Console.WriteLine("\n\n");
        }

        public static void printToPDF(Article article)
        {
            PdfWriter writer = new PdfWriter($@"C:\\PDFs\{article.title}.pdf");
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf);

            String FONT_FILENAME = @"C:\\Windows\Fonts\Arial.ttf";
            PdfFont fontrus = PdfFontFactory.CreateFont(FONT_FILENAME, PdfEncodings.IDENTITY_H);

            document.SetFont(fontrus);

            Paragraph authorAndDate = new Paragraph(article.author + "\t" + article.date)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetFontSize(14);
            document.Add(authorAndDate);

            Paragraph title = new Paragraph(new Text(article.title)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetFontSize(20));
            document.Add(title);

            Paragraph description = new Paragraph(new Text(article.description)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetFontSize(16));
            document.Add(description);

            Paragraph rateAndCount = new Paragraph(article.rate + "\t" + article.countOfViews)
            .SetTextAlignment(TextAlignment.CENTER)
            .SetFontSize(14);
            document.Add(rateAndCount);

            document.Close();
        }
    }
}
