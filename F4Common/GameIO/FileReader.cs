using FalconNet.Common.Maths;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace FalconNet.F4Common
{
    public class FileReader
    {
        private readonly InputDataDesc[] m_desc;

        private InputDataDesc FindField(string key)
        {
            for (int i = 0; i < m_desc.Length; i++)
            {
                if (m_desc[i].name == key)
                    return m_desc[i];
            }
            throw new ApplicationException("key not found : " + key);
        }

        public FileReader(InputDataDesc[] desc)
        {
            m_desc = desc;
        }

        public virtual void Initialise(object dataObj)
        {
            for (int i = 0; i < m_desc.Length; i++)
            {
                m_desc[i].action(dataObj, m_desc[i].defvalue);
            }
        }

        public virtual bool ParseField(object dataObj, string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return false;
            if (line.StartsWith("#") || line.StartsWith(";"))
                return false;
            string[] parts = line.Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            InputDataDesc field = FindField(parts[0]);
            if (parts.Length == 1)
                AssignField(field, dataObj, null);
            else
                AssignField(field, dataObj, parts[1]);
            return true;
        }

        public bool AssignField(InputDataDesc field, object dataObj, string strval)
        {
            switch (field.type)
            {
                case format.ID_INT:
                    int ival = Int16.Parse(strval);
                    field.action(dataObj, ival);
                    break;

                case format.ID_FLOAT:
                    float fval = (float)double.Parse(strval, CultureInfo.InvariantCulture);
                    field.action(dataObj, fval);
                    break;

                case format.ID_STRING:
                case format.ID_CHAR:
                    field.action(dataObj, strval);
                    break;

                case format.ID_VECTOR: // X, Y, Z
                    string[] parts = strval.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    vector v = new vector();
                    v.x = (float)double.Parse(parts[0], CultureInfo.InvariantCulture);
                    v.y = (float)double.Parse(parts[1], CultureInfo.InvariantCulture);
                    if (parts.Length > 2)
                        v.z = (float)double.Parse(parts[2], CultureInfo.InvariantCulture);
                    field.action(dataObj, v);
                    break;

                case format.ID_FLOAT_ARRAY:
                case format.ID_LOOKUPTABLE:
                case format.ID_2DTABLE:
                    throw new NotImplementedException();
            }
            return true;
        }

        //public static InputDataDesc FindField(InputDataDesc desc, string key)
        //{
        //    throw new NotImplementedException();
        //}

        //public static bool ParseField(object dataObj, string line, InputDataDesc desc)
        //{
        //    throw new NotImplementedException();
        //}

        //public static void Initialise(object dataObj, InputDataDesc desc)
        //{
        //    throw new NotImplementedException();
        //}

        public static bool ParseSimlibFile(object dataObj, InputDataDesc[] desc, SimlibFileClass inputFile)
        {
            FileReader fr = new FileReader(desc);
            string line;

            fr.Initialise(dataObj);

            while ((line = inputFile.GetNext()) != null)
            {
                if (fr.ParseField(dataObj, line) == false)
                {
                    // MLR 12/16/2003 -
                    // Who cares if one line failed!  This breaks files that have obsolete/unsupported data in them.
                    // return false;
                }
            }

            return true;
        }
    }
}
