using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Text;
using iText.Signatures;
using Microsoft.AspNetCore.Hosting;
using System.Net;
using Org.BouncyCastle.X509;
using iText.Kernel.Pdf;
using iText.Kernel.Geom;
using iText.Layout;
using iText.Kernel;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ScanApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CertificateController : ControllerBase
    {


        private readonly IHostingEnvironment _hostingEnvironment;

        public CertificateController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: api/<CertificateController>
        [HttpGet("{GeneratePDF}")]
        public bool GeneratePDFForm()
        {
            generateCert();
            return true;
        }

        private void generateCert()
        {
            string connectionString = "Data Source =.\\SQLEXPRESS; Initial Catalog = CertificateWorkFlow; Trusted_Connection = True; MultipleActiveResultSets = True; Connect Timeout = 60000;";
            string spGetAllCerticate = "GetAllCertificates";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(spGetAllCerticate))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Connection = connection;

                    connection.Open();

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            //SignatureFiledswithPDFFormField(reader);
                        }
                    }

                    reader.Close();
                    connection.Close();
                }
            }
        }

        //private void SignatureFiledswithPDFFormField(SqlDataReader rdr)
        //{
        //    string strOutputFilePath = _hostingEnvironment.WebRootPath+ "\\assets\\forms\\" + rdr["Certificate_Number"].ToString() + ".pdf";
        //    Rectangle rect = new Rectangle(1100, 1400);
        //    Document document = new Document(PageSize.LETTER.Rotate(), 0, 0, 0, 0);
        //    document.SetMargins(0, 0, 5, 5);
        //    PdfWriter writer;

        //    writer = PdfWriter.GetInstance(document, new FileStream(strOutputFilePath, FileMode.Create));
        //    document.Open();

        //    PdfPTable table = new PdfPTable(1);
        //    PdfPCell cell = new PdfPCell();
        //    ElementList list = XMLWorkerHelper.ParseToElementList(getHTMLString(rdr), null);
        //    foreach (iTextSharp.text.IElement element in list)
        //    {
        //        cell.AddElement((iTextSharp.text.IElement)element);
        //    }
        //    table.AddCell(cell);
        //    document.Add(table);

        //    PdfFormField signProdManager = PdfFormField.CreateSignature(writer);
        //    signProdManager.SetWidget(new Rectangle(90, 375, 395, 450), PdfAnnotation.HIGHLIGHT_OUTLINE);
        //    signProdManager.Name = "ProdMgr";
        //    signProdManager.FieldName = "ProdMgr";
        //    signProdManager.SetPage();
        //    writer.AddAnnotation(signProdManager);

        //    PdfFormField signQAManager = PdfFormField.CreateSignature(writer);
        //    signQAManager.SetWidget(new Rectangle(398, 375, 700, 450), PdfAnnotation.HIGHLIGHT_PUSH);
        //    signQAManager.FieldName = "QAMgr";
        //    signQAManager.SetPage();
        //    signQAManager.SetFieldFlags(PdfAnnotation.FLAGS_PRINT);
        //    writer.AddAnnotation(signQAManager);

        //    PdfFormField signLocalIBR = PdfFormField.CreateSignature(writer);
        //    signLocalIBR.SetWidget(new Rectangle(90, 218, 395, 292), PdfAnnotation.HIGHLIGHT_OUTLINE);
        //    signLocalIBR.Name = "LocalIBR";
        //    signLocalIBR.FieldName = "LocalIBR";
        //    writer.AddAnnotation(signLocalIBR);

        //    PdfFormField signRegIBR = PdfFormField.CreateSignature(writer);
        //    signRegIBR.SetWidget(new Rectangle(398, 218, 700, 292), PdfAnnotation.HIGHLIGHT_OUTLINE);
        //    signRegIBR.Name = "RegIBR";
        //    signRegIBR.FieldName = "RegIBR";
        //    writer.AddAnnotation(signRegIBR);

        //    document.Close();

        //}

        private string getHTMLString(SqlDataReader rdr)
        {

            var template = new HtmlTemplate(_hostingEnvironment.WebRootPath +"\\assets\\templates\\ksbtemplate.html");
            string output = template.Render(new
            {
                CERTIFICATE_NUMBER = rdr["Certificate_Number"].ToString(),
                DATE = Convert.ToDateTime(rdr["Date"]).ToShortDateString(),
                DESIGN_PRESSURE = rdr["Design_Pressure"].ToString(),
                DRAWING_NUMBER = rdr["Drawing_Number"].ToString(),
                HEAT_NUMBER = rdr["Heat_Number"].ToString(),
                MAIN_DIMENSIONS = rdr["Main_Dimensions"].ToString(),
                MAKER_NAME = rdr["Maker_Name"].ToString(),
                SIZE = rdr["Size"].ToString(),
                SPECIFICATION = rdr["Specification"].ToString(),
                TEST_PRESSURE = rdr["Test_Pressure"].ToString(),
                SERIAL_NUMBERS = rdr["SerialNumbers"]
            });

            return output;
        }

        //private void ReadSignatureFileds()
        //{
        //    StringBuilder sb = new StringBuilder();
        //    PdfReader reader = new PdfReader("result2.pdf");
        //    AcroFields af = reader.AcroFields;
        //    List<string> signs = af.GetSignatureNames();
        //    foreach (string sign in signs)
        //    {
        //        iTextSharp.text.pdf.security.PdfPKCS7 pk = af.VerifySignature(sign);
        //        sb.AppendFormat("Signature field name: {0}\n", sign);
        //        sb.AppendFormat("Signature signer name: {0}\n", pk.SignName);
        //        sb.AppendFormat("Signature date: {0}\n", pk.SignDate);
        //    }
        //    Console.WriteLine(sb.ToString());
        //    System.Threading.Thread.Sleep(60000);

        //}

        public static readonly string DEST = @"C:\Users\dilse\source\repos\senthilpnr\ScanApp\ScanApp\wwwroot\assets\";

        public static readonly string SRC = @"C:\Users\dilse\source\repos\senthilpnr\ScanApp\ScanApp\wwwroot\assets\5678.pdf";
        public static readonly string CERT = "https://demo.itextsupport.com/SigningApp/itextpdf.cer";

        public static readonly String[] RESULT_FILES =
        {
            "hello_server.pdf"
        };

        [HttpGet("{Sign}/{DigitalSign}")]
        public bool DigitalSignNew()
        {
            DirectoryInfo directory = new DirectoryInfo(DEST);
            directory.Create();

            // Set security protocol version to TLS 1.2 to avoid https connection issues
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

            Uri certUrl = new Uri(CERT);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(certUrl);
            request.Method = WebRequestMethods.Http.Get;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            X509CertificateParser parser = new X509CertificateParser();
            X509Certificate[] chain = new X509Certificate[1];
            using (Stream stream = response.GetResponseStream())
            {
                chain[0] = parser.ReadCertificate(stream);
            }

            this.Sign(SRC, DEST + RESULT_FILES[0], chain, PdfSigner.CryptoStandard.CMS,
                "Test", "Ghent");

            return true;
        }
        public void Sign(String src, String dest, X509Certificate[] chain, PdfSigner.CryptoStandard subfilter,
            String reason, String location)
        {
            PdfReader reader = new PdfReader(src);
            PdfSigner signer = new PdfSigner(reader, new FileStream(dest, FileMode.Create), new StampingProperties());

            // Create the signature appearance
            //Rectangle rect = new Rectangle(36, 648, 200, 100);
            //iText.Signatures.PdfSignatureAppearance appearance = signer.GetSignatureAppearance();
            //appearance
            //    .SetReason(reason)
            //    .SetLocation(location)
            //    .SetPageRect(rect)
            //    .SetPageNumber(1);
            signer.SetFieldName("QAMgr");


            //IExternalDigest digest = new BouncyCastleDigest();
            IExternalSignature signature = new ServerSignature();
            //IExternalSignature sing = new  

            // Sign the document using the detached mode, CMS or CAdES equivalent.
            //signer.SignDetached(digest, signature, chain, null, null, null,
            //        0, subfilter);

            // Sign the document using the detached mode, CMS or CAdES equivalent.
            signer.SignDetached(signature, chain, null, null, null, 0, subfilter);
        }
    }

    public class ServerSignature : IExternalSignature
    {
        public static readonly String SIGN = "http://demo.itextsupport.com/SigningApp/signbytes";

        public String GetHashAlgorithm()
        {
            return DigestAlgorithms.SHA256;
        }

        public String GetEncryptionAlgorithm()
        {
            return "RSA";
        }

        public byte[] Sign(byte[] message)
        {
            try
            {
                Uri url = new Uri(SIGN);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "POST";

                using (Stream stream = httpWebRequest.GetRequestStream())
                {
                    stream.Write(message, 0, message.Length);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    Stream stream = httpResponse.GetResponseStream();
                    stream.CopyTo(memoryStream);
                    stream.Close();

                    return memoryStream.ToArray();
                }
            }
            catch (IOException e)
            {
                throw new PdfException(e);
            }
        }
    }
    public class HtmlTemplate
    {
        private string _html;

        public HtmlTemplate(string templatePath)
        {
            using (var reader = new StreamReader(templatePath))
                _html = reader.ReadToEnd();
        }

        public string Render(object values)
        {
            string output = _html;
            foreach (var p in values.GetType().GetProperties())
                output = output.Replace("{" + p.Name + "}", (p.GetValue(values, null) as string) ?? string.Empty);
            return output;
        }
    }

}
