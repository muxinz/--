using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;
using ShErrorDeal;

namespace ShDataDLL
{
    public class XmlHelper
    {
        private XmlDocument _xml;
        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="xmlstr"></param>
        /// <param name="rootnode"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetxmlText(string xmlstr, string rootnode)//����������ڵ㲻�����ظ�������һ���ڵ�һ��������������ֵ�Ĳ�����
        {
            Dictionary<string, string> txtDic = new Dictionary<string, string>();
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlstr);
            XmlNode root = xml.SelectSingleNode(rootnode);
            XmlNodeList nodelist = root.ChildNodes;
            for (int i = 0; i < nodelist.Count; i++)
            {
                XmlNode xmlnode = nodelist[i];
                string nodename = xmlnode.Name.ToString().Trim();
                string nodevalue = xmlnode.InnerText.ToString().Trim();
                txtDic.Add(nodename, nodevalue);
            }
            return txtDic;
        }
        
        //�ܺ�÷���ҵ�ָ���ڵ��ֵ�����ظ�������
        public string GetInnerText(string strXml, string parentNode, string xmlNode) //strXml:xml�ַ���//parentNode:���ڵ������Ҫ���ҵĴ�ڵ�//xmlNode:��Ҫ�ҵ��Ľڵ�
        {
            string nodeText = "";

            try
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(strXml);
                XmlNode root = xml.SelectSingleNode(parentNode);
                XmlNodeList nodelist = root.ChildNodes;
                foreach (XmlNode node in nodelist)
                {
                    if (node.Name == xmlNode)
                    {
                        XmlElement ele = (XmlElement)node;
                        nodeText = ele.InnerText;
                    }
                }
            }
            catch (Exception ex)
            {
                AnalysisError Analysis_Error = new AnalysisError();
                string SaveError = Analysis_Error.CatchErrorForSave(ex, strXml, "xmlhelper_GetInnerText");
                //���������Ϣ��Logs��
                Deal_error Dealerror = new Deal_error();
                Dealerror.deal_Error(SaveError, "OtherError", "xmlhelper_GetInnerText", 0);
            }
            return nodeText;
        }

        #region ��������ԭ���Ļ������¼ӵķ���DataTableToXml(),DataSeTToXml(),XmlToDataSet(),XmlToDataTable()
        /// <summary>
        /// DataTableת����xml
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string DtToXml(DataTable dt)
        {
            try
            {
                System.IO.TextWriter tw = new System.IO.StringWriter();
                if (dt!=null)//2015-12-04���Ƿ�Ϊnull�ж�
                {
                    dt.TableName = dt.TableName.Length == 0 ? "Table1" : dt.TableName;
                    dt.WriteXml(tw);
                    dt.WriteXmlSchema(tw);
                }
                return tw.ToString();
            }
            catch (Exception ex)
            {
                AnalysisError Analysis_Error = new AnalysisError();
                string SaveError = Analysis_Error.CatchErrorForSave(ex, "��", "xmlhelper_DtToXml");
                //���������Ϣ��Logs��
                Deal_error Dealerror = new Deal_error();
                Dealerror.deal_Error(SaveError, "OtherError", "xmlhelper_DtToXml", 0);

                //������Ϣ���ر���
                string SendError = Analysis_Error.CatchErrorForClient(ex);

                return SendError;
            }

        }
        /// <summary>
        /// DataSetת����xml
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public string DsToXml(DataSet ds)
        {
            try
            {
                System.IO.TextWriter tw = new System.IO.StringWriter();
                if (ds != null)//2015-12-04���Ƿ�Ϊnull�ж�
                {
                    ds.WriteXml(tw);
                    ds.WriteXmlSchema(tw);
                }

                return tw.ToString();
            }
            catch (Exception ex)
            {
                AnalysisError Analysis_Error = new AnalysisError();
                string SaveError = Analysis_Error.CatchErrorForSave(ex, "��", "xmlhelper_DsToXml");
                //���������Ϣ��Logs��
                Deal_error Dealerror = new Deal_error();
                Dealerror.deal_Error(SaveError, "OtherError", "xmlhelper_DsToXml", 0);

                //������Ϣ���ر���
                string SendError = Analysis_Error.CatchErrorForClient(ex);

                return SendError;
            }

        }
        /// <summary>
        /// xmlת����DataSet
        /// </summary>
        /// <param name="XmlStr"></param>
        /// <returns></returns>
        public DataSet XmlToDs(string XmlStr)
        {
            DataSet ds_temp = new DataSet();
            try
            {

                System.IO.TextReader str_ds = new System.IO.StringReader(XmlStr.Substring(0, XmlStr.IndexOf("<?xml")));
                System.IO.TextReader trSchema = new System.IO.StringReader(XmlStr.Substring(XmlStr.IndexOf("<?xml")));

                ds_temp.ReadXmlSchema(trSchema);
                ds_temp.ReadXml(str_ds);

            }
            catch //(Exception ex)
            {
                ds_temp = null;
            }
            return ds_temp;

        }
        /// <summary>
        /// xmlת���ɱ�
        /// </summary>
        /// <param name="XmlStr"></param>
        /// <returns></returns>
        public DataTable XmlToDt(string XmlStr)
        {
            DataTable dtReturn = new DataTable();
            try
            {
                System.IO.TextReader trDataTable = new System.IO.StringReader(XmlStr.Substring(0, XmlStr.IndexOf("<?xml")));
                System.IO.TextReader trSchema = new System.IO.StringReader(XmlStr.Substring(XmlStr.IndexOf("<?xml")));

                dtReturn.ReadXmlSchema(trSchema);
                dtReturn.ReadXml(trDataTable);
            }
            catch
            {
                dtReturn = null;
            }

            return dtReturn;
        }
        #endregion 

        #region Ŀǰ��û�����õ�

        #region ���캯�� Xml�������� public XmlHelper()
        /// <summary>
        /// Xml�������� 
        /// </summary>
        public XmlHelper()
        {
            _xml = new XmlDocument();
        }
        #endregion

        #region ����ָ��������·����XML�ĵ� public virtual void Load(string path)
        /// <summary>
        /// ����ָ��������·����XML�ĵ�
        /// </summary>
        /// <param name="path"></param>
        public void Load(string path){
            try
            {
                if (_xml != null)
                {
                    _xml = new XmlDocument();
                }
                _xml.Load(path);
            }
            catch (Exception e) {
                throw new Exception(e.Message.ToString());
            }
        }
        #endregion

        #region ����ָ��������·����XML�ĵ�������DataSet public DataSet GetDataSet(string path)
        /// <summary>
        /// ����ָ��������·����XML�ĵ�������DataSet
        /// </summary>
        /// <param name="path"></param>
        public DataSet GetDataSet(string path)
        {
            try
            {
                DataSet ds = new DataSet();
                ds.ReadXml(path);
                return ds;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
        }
        #endregion

        #region ��ȡXMLΪDataSet������DataSet public DataSet GetDataSet()
        /// <summary>
        /// ��ȡXMLΪDataSet������DataSet
        /// </summary>
        /// <returns></returns>
        public DataSet GetDataSet()
        {
            XmlReader xmlreader = XmlReader.Create(new System.IO.StringReader(_xml.OuterXml));
            try
            {
                DataSet ds = new DataSet();
                ds.ReadXml(xmlreader);
                return ds;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message.ToString());
            }
        }
        #endregion

        #region �����ַ��������ӵ�  public void SetRootXml(string content)
        /// <summary>
        /// �����ַ��������ӵ�
        /// </summary>
        /// <param name="content"></param>
        public void SetRootXml(string content)
        {
            _xml.DocumentElement.InnerXml = content;
        }
        #endregion

        #region ����Xml�ĵ� public XmlDocument CreateXml(string version, string encoding, string standalone, string rootname)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="version">�汾</param>
        /// <param name="encoding">����</param>
        /// <param name="standalone">��yes����no��</param>
        /// <param name="rootname">���ӵ���</param>
        /// <returns></returns>
        public XmlDocument CreateXml(string version, string encoding, string standalone, string rootname)
        {
            XmlNode m_root = _xml.DocumentElement;
            _xml.AppendChild(_xml.CreateXmlDeclaration(version, encoding, standalone));
            _xml.AppendChild(_xml.CreateElement(rootname));
            return _xml;
        }
        #endregion

        #region ��ȡXml�ĵ� public XmlDocument GetXml()
        /// <summary>
        /// ��ȡXml�ĵ�
        /// </summary>
        /// <returns>����XmlDocument</returns>

        public XmlDocument GetXml() {
            return _xml;
        }
        #endregion

        #region ��ȡ���ӵ��µ��ַ��� public string GetString()
        /// <summary>
        /// ��ȡ���ӵ��µ��ַ���
        /// </summary>
        /// <returns></returns>
        public string GetString()
        {
            XmlNode m_root = _xml.DocumentElement;
            return m_root.InnerXml;
        }
        #endregion

        #region ��ȡָ���ӵ��б� public virtual XmlNodeList GetNodesList(string nodepath)
        /// <summary>
        /// ��ȡָ���ӵ��б�
        /// </summary>
        /// <param name="nodepath">�ӵ�·��</param>
        /// <returns>XmlNodeList</returns>
        public virtual XmlNodeList GetNodesList(string nodepath)
        {
            XmlNodeList xmlnode = _xml.SelectNodes(nodepath);
            return xmlnode;
        }
        #endregion

        #region ��ȡ�����ӵ� public virtual XmlNode GetNode(string nodepath)
        /// <summary>
        /// ��ȡ�����ӵ�
        /// </summary>
        /// <param name="nodepath">�ӵ�·��</param>
        /// <returns>XmlNode</returns>
        public virtual XmlNode GetNode(string nodepath)
        {
            XmlNode xmlnode = _xml.SelectSingleNode(nodepath);
            return xmlnode;
        }
        #endregion

        #region ��ȡ�����ӵ��ֵ public virtual string GetNodeText(string nodepath)
        /// <summary>
        /// ��ȡ�����ӵ��ֵ 
        /// </summary>
        /// <param name="nodepath">�ӵ�·��</param>
        /// <returns>null or string</returns>
        public virtual string GetNodeText(string nodepath)
        {
            XmlNode xmlnode = _xml.SelectSingleNode(nodepath);
            if (xmlnode == null)
            {
                return null;
            }
            else
            {
                return xmlnode.InnerText.ToString();
            }
        }
        #endregion

        #region ��ȡ�ӵ����� public virtual string GetNodeKey(string nodepath, string key)
        /// <summary>
        /// ��ȡ�ӵ�����
        /// </summary>
        /// <param name="nodepath">�ӵ�·��</param>
        /// <param name="key">����</param>
        /// <returns>null or string</returns>
        public virtual string GetNodeKey(string nodepath, string key)
        {
            XmlNode xmlnode = _xml.SelectSingleNode(nodepath);
            if (xmlnode == null)
            {
                return null;
            }
            else
            {
                XmlAttribute xmlnodekey = xmlnode.Attributes[key];
                if (xmlnodekey == null)
                {
                    return null;
                }
                else
                {
                    return xmlnodekey.Value.ToString();
                }
            }
        }
        #endregion

        #region ���ýӵ��ֵ public void SetNodeText(string NodeName, string NodeText)
        public void SetNodeText(string NodeName, string NodeText)
        {
            XmlNode mynode = _xml.SelectSingleNode(NodeName);
            mynode.InnerText = NodeText;
        }
        #endregion

        #region ��ӽӵ� public bool AddNode(string key, string value)
        /// <summary>
        /// ��ӽӵ�
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <returns>bool</returns>
        public bool AddNode(string key, string value)
        {

            XmlNode m_root = _xml.DocumentElement;
            if (m_root == null)
            {
                return false;
            }

            XmlElement _ItemName = _xml.CreateElement(key);
            _ItemName.InnerXml = value;
            m_root.AppendChild(_ItemName);
            return true;
        }
        #endregion

        #region ����XML ��Ӳ��  public bool Save()
        /// <summary>
        /// ����XML ��Ӳ��
        /// </summary>
        /// <param name="path">����·��</param>
        /// <returns></returns>
        public bool Save(string path) {
            _xml.Save(path);
            return true;
        }
        #endregion

        #endregion

    }
}
