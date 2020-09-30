using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.text.pdf.security;
using System.IO;
using System.Text;
using iText.Signatures;
using Microsoft.AspNetCore.Hosting;


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
                            SignatureFiledswithPDFFormField(reader);
                        }
                    }

                    reader.Close();
                    connection.Close();
                }
            }
        }

        private void SignatureFiledswithPDFFormField(SqlDataReader rdr)
        {
            string strOutputFilePath = _hostingEnvironment.WebRootPath+ "\\assets\\forms\\" + rdr["Certificate_Number"].ToString() + ".pdf";
            Rectangle rect = new Rectangle(1100, 1400);
            Document document = new Document(PageSize.LETTER.Rotate(), 0, 0, 0, 0);
            document.SetMargins(0, 0, 5, 5);
            PdfWriter writer;

            writer = PdfWriter.GetInstance(document, new FileStream(strOutputFilePath, FileMode.Create));
            document.Open();

            PdfPTable table = new PdfPTable(1);
            PdfPCell cell = new PdfPCell();
            ElementList list = XMLWorkerHelper.ParseToElementList(getHTMLString(rdr), null);
            foreach (iTextSharp.text.IElement element in list)
            {
                cell.AddElement((iTextSharp.text.IElement)element);
            }
            table.AddCell(cell);
            document.Add(table);

            PdfFormField signProdManager = PdfFormField.CreateSignature(writer);
            signProdManager.SetWidget(new Rectangle(90, 375, 395, 450), PdfAnnotation.HIGHLIGHT_OUTLINE);
            signProdManager.Name = "ProdMgr";
            signProdManager.FieldName = "ProdMgr";
            signProdManager.SetPage();
            writer.AddAnnotation(signProdManager);

            PdfFormField signQAManager = PdfFormField.CreateSignature(writer);
            signQAManager.SetWidget(new Rectangle(398, 375, 700, 450), PdfAnnotation.HIGHLIGHT_PUSH);
            signQAManager.FieldName = "QAMgr";
            signQAManager.SetPage();
            signQAManager.SetFieldFlags(PdfAnnotation.FLAGS_PRINT);
            writer.AddAnnotation(signQAManager);

            PdfFormField signLocalIBR = PdfFormField.CreateSignature(writer);
            signLocalIBR.SetWidget(new Rectangle(90, 218, 395, 292), PdfAnnotation.HIGHLIGHT_OUTLINE);
            signLocalIBR.Name = "LocalIBR";
            signLocalIBR.FieldName = "LocalIBR";
            writer.AddAnnotation(signLocalIBR);

            PdfFormField signRegIBR = PdfFormField.CreateSignature(writer);
            signRegIBR.SetWidget(new Rectangle(398, 218, 700, 292), PdfAnnotation.HIGHLIGHT_OUTLINE);
            signRegIBR.Name = "RegIBR";
            signRegIBR.FieldName = "RegIBR";
            writer.AddAnnotation(signRegIBR);

            document.Close();

        }

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

        private void ReadSignatureFileds()
        {
            StringBuilder sb = new StringBuilder();
            PdfReader reader = new PdfReader("result2.pdf");
            AcroFields af = reader.AcroFields;
            List<string> signs = af.GetSignatureNames();
            foreach (string sign in signs)
            {
                iTextSharp.text.pdf.security.PdfPKCS7 pk = af.VerifySignature(sign);
                sb.AppendFormat("Signature field name: {0}\n", sign);
                sb.AppendFormat("Signature signer name: {0}\n", pk.SignName);
                sb.AppendFormat("Signature date: {0}\n", pk.SignDate);
            }
            Console.WriteLine(sb.ToString());
            System.Threading.Thread.Sleep(60000);

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
