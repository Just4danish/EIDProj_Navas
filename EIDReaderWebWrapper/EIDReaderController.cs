using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using EIDReaderLib;
using Newtonsoft.Json;
using System.Xml;

namespace EIDReaderWebWrapper
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/EIDReader")]
    public class EIDReaderController : ApiController
    {
        //EIDData objData;
        //EIDReader objEidReader;
        public EIDReaderController()
        {
            
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("ListReaders")]
        public IHttpActionResult ListReaders()
        {
            Program.objEidReader = new EIDReader();
            Program.objEidReader.objData = new EIDData();
            Program.objEidReader.InitReader();
            if (Program.objEidReader.objData.Status)
            {
                return Ok(Program.objEidReader.objData);
            }
            else
            {
                return InternalServerError(new Exception(Program.objEidReader.objData.Message));
            }
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("Connect")]
        public IHttpActionResult Connect(int Reader)
        {
            Program.objEidReader.ConnectReaderByIndex(Reader);
            if (Program.objEidReader.objData.Status)
            {
                return Ok("Connected");
            }
            else
            {
                return InternalServerError(new Exception(Program.objEidReader.objData.Message));
            }
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("DisConnect")]
        public IHttpActionResult DisConnect()
        {
            Program.objEidReader.DisConnectReader();
            if (Program.objEidReader.objData.Status)
            {
                return Ok("Disconnected");
            }
            else
            {
                return InternalServerError(new Exception(Program.objEidReader.objData.Message));
            }
        }

        /// <summary>
        /// must Call "ListReaders" then "Connect"
        /// After reading call "DisConnect"
        /// </summary>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("ReadData")]
        public IHttpActionResult ReadData()
        {
            Program.objEidReader.Read();
            if (Program.objEidReader.objData.Status)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(Program.objEidReader.objData.EIDDataXMLString);
                //Convert XML To String for parsing
                string jsonText = JsonConvert.SerializeXmlNode(xmlDoc);
                //Convert String to Json Dynamic Object
                dynamic stuff = JsonConvert.DeserializeObject(jsonText);
                return Ok(stuff);
            }
            else
            {
                return InternalServerError(new Exception(Program.objEidReader.objData.Message));
            }
        }

        /// <summary>
        /// inclued connect, read, disconnect in single funcion
        /// </summary>
        /// <returns></returns>
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("ConnectAndReadData")]
        public IHttpActionResult ConnectAndReadData(string readername)
        {
            //Program.objEidReader.Read();
            Program.objEidReader = new EIDReader();
            Program.objEidReader.objData = new EIDData();
            Program.objEidReader.ConnectAndRead(readername);
            if (Program.objEidReader.objData.Status)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(Program.objEidReader.objData.EIDDataXMLString);
                //Convert XML To String for parsing
                string jsonText = JsonConvert.SerializeXmlNode(xmlDoc);
                //Convert String to Json Dynamic Object
                dynamic stuff = JsonConvert.DeserializeObject(jsonText);
                return Ok(stuff);
            }
            else
            {
                return InternalServerError(new Exception(Program.objEidReader.objData.Message));
            }
        }
    }
}
