using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DecisionTreeEntropyGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            XDocument dataSetDocument = XDocument.Parse(Properties.Resources.MovieData);
            XElement dataSetElement = dataSetDocument.Root;
            DataSet dataSet = DataSet.FromXml(dataSetElement);

            QuestionTreeNode node = dataSet.GenerateNode();
            Console.ReadKey();
        }
    }
}
