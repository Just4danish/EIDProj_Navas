using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using AE.EmiratesId.IdCard;
using AE.EmiratesId.IdCard.DataModels;
using Newtonsoft.Json;

namespace EIDReaderLib
{
    public class EIDReader
    {
        public Toolkit ToolkitObj = null;
        public CardReader[] readers = null;
        public CardReader objReader = null;
        string ConfigParams = null;
        public EIDData objData = new EIDData();

        public EIDReader()
        {
            string configFileDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            string logfolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\EID\\Logs";
            if (!Directory.Exists(logfolder))
            {
                Directory.CreateDirectory(logfolder);
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"vg_url = http://vg-pre-prod.ica.gov.ae/ValidationGatewayService");
            sb.AppendLine(@"vg_connection_timeout = 60");
            sb.AppendLine(@"config_directory = " + configFileDirectory.Replace(@"\", "/"));
            sb.AppendLine(@"read_publicdata_offline = true");
            sb.AppendLine(@"log_directory = " + logfolder.Replace(@"\", "/"));
            sb.AppendLine(@"log_level = ALL");
            sb.AppendLine(@"log_performance_time = true");
            sb.AppendLine(@"agent_tls_enabled = false");
            sb.AppendLine(@"config_connection_timeout = 10");
            sb.AppendLine(@"enable_digital_signature_service = false");

            ConfigParams = sb.ToString();
        }

        public void InitReader()
        {
            objData = new EIDData();
            //Init toolkit
            try
            {
                ToolkitObj = new Toolkit(true, ConfigParams);
                string v = ToolkitObj.GetToolkitVersion();
            }
            catch (ToolkitException ex)
            {
                objData.Message = "Toolkit init exception. " + ex.Message;
                objData.Status = false;
                objData.InitStatus = false;
                return;
            }

            //List readers
            try
            {
                readers = null;
                // List readers
                readers = ToolkitObj.ListReaders();
                if (readers == null || readers.Count() <= 0)
                {
                    objData.Message = "Toolkit list reader, 0 readers found. Insert Card Properly.";
                    objData.Status = false;
                    objData.InitStatus = false;
                    return;
                }
                else
                {
                    objData.Readers = readers;
                    objData.Status = true;
                    objData.InitStatus = true;
                }
            }
            catch (ToolkitException ex)
            {
                objData.Message = "Toolkit list reader exception. " + ex.Message;
                objData.Status = false;
                objData.InitStatus = false;
                return;
            }
            return;
        }

        public void ConnectReaderByIndex(int ReaderIndex = 0)
        {
            //Init 
            //Moved to to init api
            //InitReader();
            if (!objData.Status && objData.InitStatus)
            {
                //Failed to init stop
                return;
            }

            //Connect first reader
            try
            {
                if (readers != null && readers.Count() > 0)
                {
                    objReader = readers[ReaderIndex];
                    if (objReader != null)
                    {
                        objReader.Connect();
                        objData.Message = "ID Card Reader Connected";
                        objData.Status = true;
                    }
                    else
                    {
                        objData.Message = "ID Card Reader Not Connected";
                        objData.Status = false;
                    }
                }
            }
            catch (Exception ex)
            {
                objData.Message = "Connect reader exception. " + ex.Message;
                objData.Status = false;
                return;
            }
            return;
        }

        public void ConnectReaderByName(string readername)
        {
            //Init 
            //Moved to to init api
            //InitReader();
            if (!objData.Status && objData.InitStatus)
            {
                //Failed to init stop
                return;
            }

            //Connect first reader
            try
            {
                if (readers != null && readers.Count() > 0)
                {
                    if(readername == null || readername.Trim().Length == 0)
                    {
                        objReader = readers[0];
                    }
                    else
                    {
                        foreach (CardReader r in readers)
                        {
                            if (r.Name.Trim().ToLower() == readername.Trim().ToLower())
                            {
                                objReader = r;
                            }
                        }
                        if(objReader == null)
                        {
                            objReader = readers[0];
                        }
                    }

                    if (objReader != null)
                    {
                        objReader.Connect();
                        objData.Message = "ID Card Reader Connected";
                        objData.Status = true;
                    }
                    else
                    {
                        objData.Message = "ID Card Reader Not Connected";
                        objData.Status = false;
                    }
                }
            }
            catch (Exception ex)
            {
                objData.Message = "Connect reader exception. " + ex.Message;
                objData.Status = false;
                return;
            }
            return;
        }

        public void DisConnectReader()
        {
            //Disconnect Reader
            try
            {
                if (objReader != null)
                {
                    ToolkitObj.Cleanup();
                    objReader.Disconnect();
                }
            }
            catch (Exception ex)
            {
                if (!objData.Status)
                {
                    objData.Message = "Read Disconnect error";
                    objData.Status = false;
                    objData.InitStatus = false;
                    objData.ConnectStatus = false;
                    return;
                }
            }
            return;
        }

        /// <summary>
        /// Read EID Card connected
        /// </summary>
        /// <returns></returns>
        public void Read()
        {
            if (ConfigParams == null)
            {
                objData.Message = "Configuration file not found";
                objData.Status = false;
                return;
            }

            ////Init & Connect
            //if (!objData.Status)
            //{
            //    ConnectReader();
            //    //Failedd stop
            //    return objData;
            //}

            //Read Data
            try
            {
                GenerateRequestID();
                CardPublicData cardPublicDataObj = objReader.ReadPublicData(
                CurrentRequestID,
                true,
                true,
                true,
                true,
                true);
                objData.Message = "Read data success";
                objData.Status = true;
                objData.EIDDataXMLString = cardPublicDataObj.XmlString;
            }
            catch (Exception ex)
            {
                objData.Message = "Read data error. " + ex.Message;
                objData.Status = false;
                return;
            }

            //Disconnect
            //objData = DisConnectReader(objData);
            return;
        }

        /// <summary>
        /// Read EID Card connected
        /// </summary>
        /// <returns></returns>
        public void ConnectAndRead(string readername)
        {
            if (ConfigParams == null)
            {
                objData.Message = "Configuration file not found";
                objData.Status = false;
                return;
            }

            //Init & Connect
            if (!objData.Status)
            {
                //init
                InitReader();
                ConnectReaderByName(readername);
            }

            if (!objData.Status)
            {
                //Connection failed
                return;
            }

            //Read Data
            try
            {
                GenerateRequestID();
                CardPublicData cardPublicDataObj = objReader.ReadPublicData(
                CurrentRequestID,
                true,
                true,
                true,
                true,
                true);
                objData.Message = "Read data success";
                objData.Status = true;
                objData.EIDDataXMLString = cardPublicDataObj.XmlString;
            }
            catch (Exception ex)
            {
                objData.Message = "Read data error. " + ex.Message;
                objData.Status = false;
                return;
            }

            //Disconnect
            DisConnectReader();
            return;
        }

        private string CurrentRequestID = "";
        private void GenerateRequestID()
        {
            Random rand = new Random();
            Byte[] randBytes = new Byte[40];
            rand.NextBytes(randBytes);
            CurrentRequestID = Convert.ToBase64String(randBytes);
        }
    }
}
