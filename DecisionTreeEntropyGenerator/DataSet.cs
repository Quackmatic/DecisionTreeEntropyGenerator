using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace DecisionTreeEntropyGenerator
{
    public class DataSet
    {
        public DatumSchema Schema
        {
            get;
            protected set;
        }

        public Datum[] Data
        {
            get;
            protected set;
        }

        public DataSet(DatumSchema schema, params Datum[] data)
        {
            Schema = schema;
            Data = data;
        }

        public QuestionTreeNode GenerateNode()
        {
            return new QuestionTreeNode(
                Schema,
                Data,
                Schema.Attributes
                    .Where(a => a.IsQueryable)
                    .ToArray());
        }

        public static DataSet FromXml(XElement element)
        {
            XElement schemaElement = element.Element("Schema");
            DatumSchema schema = DatumSchema.FromXml(schemaElement);

            XElement dataElement = element.Element("Data");
            Datum[] data = dataElement
                .Elements("Datum")
                .Select(datumElement =>
                {
                    try
                    {
                        string answer = datumElement.Attribute("Answer").Value;
                        string[] attributeValues = new string[schema.Attributes.Length];

                        for (int i = 0; i < attributeValues.Length; i++)
                        {
                            string attributeName = schema.Attributes[i].Name;
                            var attributeValueElement = datumElement.Elements(attributeName);
                            if (attributeValueElement.Count() == 0)
                            {
                                throw new IOException(String.Format(
                                    "The element at line {0} in the input data does not contain " +
                                    "an attribute value for {1}.",
                                    (datumElement as IXmlLineInfo).LineNumber,
                                    attributeName));
                            }
                            string value = attributeValueElement.First().Value;
                            attributeValues[i] = value;
                        }

                        return new Datum(schema, answer, attributeValues);
                    }
                    catch (Exception ex)
                    {
                        throw new IOException(String.Format(
                            "An exception occurred parsing the datum at line {0}.",
                            (datumElement as IXmlLineInfo).LineNumber), ex);
                    }
                }).ToArray();

            return new DataSet(schema, data);
        }
    }
}
