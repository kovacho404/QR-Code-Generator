using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using QRCoder;

namespace QRKodGenerator
{
    public class Form1 : Form
    {
        private TextBox txtTekst;
        private TextBox txtOpis; // Novi TextBox za opis ispod QR koda
        private Button btnGenerisi;
        private Button btnPrint;
        private PictureBox picBoxQRCode;
        private Bitmap qrCodeImage;

        public Form1()
        {
            this.Text = "QR Code Generator";
            this.Size = new Size(400, 550);

            // TextBox za unos teksta za QR kod
            txtTekst = new TextBox { Location = new Point(20, 20), Width = 250 };
            this.Controls.Add(txtTekst);

            // Button za generisanje QR koda
            btnGenerisi = new Button { Text = "Generiši", Location = new Point(280, 20), Width = 80 };
            btnGenerisi.Click += BtnGenerisi_Click;
            this.Controls.Add(btnGenerisi);

            // PictureBox za prikazivanje QR koda
            picBoxQRCode = new PictureBox { Location = new Point(20, 60), Size = new Size(300, 300), BorderStyle = BorderStyle.FixedSingle };
            this.Controls.Add(picBoxQRCode);

            // TextBox za unos opisa ispod QR koda
            txtOpis = new TextBox { Location = new Point(20, 380), Width = 300, PlaceholderText = "Unesite opis za ispod QR koda" };
            this.Controls.Add(txtOpis);

            // Button za štampanje QR koda
            btnPrint = new Button { Text = "Štampaj", Location = new Point(150, 420), Width = 80 };
            btnPrint.Click += BtnPrint_Click;
            this.Controls.Add(btnPrint);
        }

        // Metoda za generisanje QR koda
        private void BtnGenerisi_Click(object sender, EventArgs e)
        {
            string tekst = txtTekst.Text;
            if (string.IsNullOrWhiteSpace(tekst))
            {
                MessageBox.Show("Molim unesite tekst za generisanje QR koda.");
                return;
            }

            using (QRCodeGenerator generator = new QRCodeGenerator())
            {
                QRCodeData data = generator.CreateQrCode(tekst, QRCodeGenerator.ECCLevel.Q);
                using (QRCode qrCode = new QRCode(data))
                {
                    qrCodeImage = qrCode.GetGraphic(20);
                    picBoxQRCode.Image = qrCodeImage;
                    picBoxQRCode.SizeMode = PictureBoxSizeMode.Zoom;
                }
            }
        }

        // Metoda za štampanje
        private void BtnPrint_Click(object sender, EventArgs e)
        {
            if (qrCodeImage == null)
            {
                MessageBox.Show("Prvo generišite QR kod pre štampanja.");
                return;
            }

            string opis = txtOpis.Text; // Tekst ispod QR koda
            PrintDocument printDoc = new PrintDocument();

            // Poveži PrintPage događaj
            printDoc.PrintPage += (s, ev) =>
            {
                // Definiši željenu veličinu QR koda u pikselima
                int desiredWidth = 200;  // Širina QR koda
                int desiredHeight = 200; // Visina QR koda

                // Dimenzije štampane oblasti
                float pageWidth = ev.PageSettings.PrintableArea.Width;
                float pageHeight = ev.PageSettings.PrintableArea.Height;

                // Izračunaj poziciju za centriranje QR koda
                float x = (pageWidth - desiredWidth) / 2; // Centriraj horizontalno
                float y = (pageHeight - desiredHeight) / 2 - 50; // Centriraj vertikalno sa malo pomeranja gore

                // Štampanje QR koda
                ev.Graphics.DrawImage(qrCodeImage, x, y, desiredWidth, desiredHeight);

                // Štampanje teksta ispod QR koda
                if (!string.IsNullOrWhiteSpace(opis))
                {
                    using (Font font = new Font("Arial", 12))
                    {
                        SizeF textSize = ev.Graphics.MeasureString(opis, font);
                        float textX = (pageWidth - textSize.Width) / 2; // Centriraj tekst horizontalno
                        float textY = y + desiredHeight + 10; // Postavi tekst ispod QR koda sa razmakom
                        ev.Graphics.DrawString(opis, font, Brushes.Black, textX, textY);
                    }
                }
            };

            // Prikazivanje dijaloga za štampanje
            PrintPreviewDialog previewDialog = new PrintPreviewDialog { Document = printDoc };
            if (previewDialog.ShowDialog() == DialogResult.OK)
            {
                printDoc.Print();
            }
        }

        // Glavna metoda za pokretanje aplikacije
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
