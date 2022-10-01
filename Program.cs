using System;
using System.IO;
using System.Text;
using System.Xml;

namespace WebAppParser
{
    class WebAppParser
    {
        
        static void Main(string[] args)
        {
            //isAbstractMethod bool is used so that only one "abstract_method" JSON array is produced and things like the package and imports are not parsed
            bool isAbstractMethod = false;
            //used for getting the type of the parameter when parsing the XML file
            string type = "";
            //used to create the text for the JSON file by having values appended to it
            StringBuilder output = new StringBuilder();

            try
            {
                //attempts to open the XML file in the program directory
                using (var filestream = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + @"\" + "WebApp.xml"))
                using (XmlReader xmlReader = XmlReader.Create(filestream, new XmlReaderSettings()))
                {
                    while (xmlReader.Read())
                    {
                        //the XML file is only parsed when it finds the first "abstract_method" tag
                        if (xmlReader.Name == "abstract_method" && !isAbstractMethod)
                        {
                            output.AppendLine("{");
                            output.AppendLine("\"abstract_method\": [");

                            isAbstractMethod = true;
                        }
                        //switch statement based on the XML node parsed
                        switch (xmlReader.NodeType)
                        {
                            case XmlNodeType.Element:

                                //switch statement based on the XML tag parsed
                                switch (xmlReader.Name)
                                {
                                    case "abstract_method":
                                        output.AppendLine("{\n\"name\": \"" + xmlReader.GetAttribute("name") + "\",");
                                        break;
                                    case "access":
                                        output.Append("\"access\": ");
                                        break;
                                    case "argument":
                                        type = xmlReader.GetAttribute("type");
                                        output.Append("{\n\"var\": ");
                                        break;
                                    case "parameters":
                                        output.AppendLine("\"parameters\": [");
                                        break;
                                    case "throws":
                                        output.AppendLine("\"exceptions\": [");
                                        break;
                                    case "return":
                                        output.Append("\"return\": ");
                                        break;
                                }
                                break;

                            //parses the values in the XML tag
                            case XmlNodeType.Text:
                                if (isAbstractMethod) output.Append("\"" + xmlReader.Value + "\"");
                                break;

                            case XmlNodeType.EndElement:

                                //switch statement based on the XML end tags parsed
                                switch (xmlReader.Name)
                                {

                                    case "access":
                                        output.AppendLine(",");
                                        break;
                                    case "argument":
                                        output.AppendLine(",\n\"type\": \"" + type + "\"\n},");
                                        break;
                                    case "parameters":
                                        //removes extra comma from the parsed XML
                                        output.Length -= 3;
                                        output.Append("\n],\n");
                                        break;
                                    case "throws":
                                        //removes extra comma from the parsed XML
                                        output.Length -= 2;
                                        output.AppendLine("\n],");
                                        break;
                                    case "exception":
                                        output.Append(",\n");
                                        break;
                                    case "return":
                                        output.Append("\n},\n");
                                        break;
                                }
                                break;
                        }

                    }
                    //removes extra comma from the parsed XML
                    output.Length -= 2;
                    output.Append("\n]\n}");
                }
                //JSON file is written to the same location as the application
                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\" + "WebApp.json", output.ToString());
                //Text from the JSON file is printed onto the console
                Console.WriteLine(output.ToString());
                Console.WriteLine("JSON file created. Press enter to exit...");
                var x = Console.ReadLine();
            }

            //Error message printed in case the file is not found
            catch (Exception e)
            {
                Console.WriteLine("XML file not found. Press enter to exit...");
                var x = Console.ReadLine();
            }
        }
    }
}
