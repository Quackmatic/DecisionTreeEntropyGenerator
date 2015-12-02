using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DecisionTreeEntropyGenerator
{
    public class DatumSchema
    {
        public string[] Attributes
        {
            get;
            protected set;
        }

        public Predicate<string> AnswerValidator
        {
            get;
            protected set;
        }

        public int GetAttributeIndex(string attribute)
        {
            for(int i = 0; i < Attributes.Length; i++)
            {
                if(Attributes[i] == attribute)
                {
                    return i;
                }
            }

            return -1;
        }

        public DatumSchema(string[] attributes, Predicate<string> answerValidator)
        {
            Attributes = attributes;
            AnswerValidator = answerValidator;
        }
        
        public static DatumSchema FromXml(XElement element)
        {
            XElement attributesElement = element.Element("Attributes");
            XElement answersElement = element.Element("Answers");

            string[] attributes = attributesElement
                .Elements("Attribute")
                .Select(e => e.Value)
                .ToArray();
            string[] validAnswers = answersElement
                .Elements("Answer")
                .Select(e => e.Value)
                .ToArray();

            return new DatumSchema(attributes, s => validAnswers.Contains(s));
        }
    }
}
