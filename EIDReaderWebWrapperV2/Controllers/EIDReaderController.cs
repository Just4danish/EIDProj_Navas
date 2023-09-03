using EIDReaderLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Xml;

namespace EIDReaderWebWrapperV2.Controllers
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
            //created for loggin inconsole
            //can remove on production env
            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + ": Request Recieved: api/EIDReader/ListReaders");
            Helper.objEidReader = new EIDReader();
            Helper.objEidReader.objData = new EIDData();
            Helper.objEidReader.InitReader();
            if (Helper.objEidReader.objData.Status)
            {
                return Ok(Helper.objEidReader.objData);
            }
            else
            {
                return InternalServerError(new Exception(Helper.objEidReader.objData.Message));
            }
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("Connect")]
        public IHttpActionResult Connect(int Reader)
        {
            Helper.objEidReader.ConnectReaderByIndex(Reader);
            if (Helper.objEidReader.objData.Status)
            {
                return Ok("Connected");
            }
            else
            {
                return InternalServerError(new Exception(Helper.objEidReader.objData.Message));
            }
        }

        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpGet]
        [Route("DisConnect")]
        public IHttpActionResult DisConnect()
        {
            Helper.objEidReader.DisConnectReader();
            if (Helper.objEidReader.objData.Status)
            {
                return Ok("Disconnected");
            }
            else
            {
                return InternalServerError(new Exception(Helper.objEidReader.objData.Message));
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
            Helper.objEidReader.Read();
            if (Helper.objEidReader.objData.Status)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(Helper.objEidReader.objData.EIDDataXMLString);
                //Convert XML To String for parsing
                string jsonText = JsonConvert.SerializeXmlNode(xmlDoc);
                //Convert String to Json Dynamic Object
                dynamic stuff = JsonConvert.DeserializeObject(jsonText);
                return Ok(stuff);
            }
            else
            {
                return InternalServerError(new Exception(Helper.objEidReader.objData.Message));
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
            Helper.objEidReader = new EIDReader();
            Helper.objEidReader.objData = new EIDData();
            Helper.objEidReader.ConnectAndRead(readername);
            if (Helper.objEidReader.objData.Status)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(Helper.objEidReader.objData.EIDDataXMLString);
                //Convert XML To String for parsing
                string jsonText = JsonConvert.SerializeXmlNode(xmlDoc);
                //Convert String to Json Dynamic Object
                dynamic stuff = JsonConvert.DeserializeObject(jsonText);
                return Ok(stuff);
            }
            else
            {
                return InternalServerError(new Exception(Helper.objEidReader.objData.Message));
            }
        }
    }
}