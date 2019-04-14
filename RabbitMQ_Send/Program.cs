using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using System;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;

namespace RabbitMQ_Send
{
    class Program
    {
        static void Main(string[] args)
        {
            SendString();
            //SendJson();
            //XmlTry();
            //SendXml();

            //DisplayInfo();
        }

        static void DisplayInfo ()
        {
            Console.WriteLine($"Calling appdomain: {Thread.GetDomain().FriendlyName}");

            Console.WriteLine($"Entry assembly name: {Assembly.GetEntryAssembly().FullName}");

            Console.WriteLine($"Calling assembly name: {Assembly.GetCallingAssembly().FullName}");
            Console.ReadKey();
        }

        static void SendString()
        {
            string usrinput = string.Empty;

            while (usrinput.ToLower() != "end")
            {
                Console.WriteLine(">>> What message would you like to send?");
                usrinput = Console.ReadLine();

                if (usrinput.ToLower() != "end")
                {
                    var factory = new ConnectionFactory() { HostName = "localhost" };
                    using (var connection = factory.CreateConnection())
                    using (var channel = connection.CreateModel())
                    {
                        //this part creates the queue
                        channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);

                        string message = usrinput;
                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "", routingKey: "hello", basicProperties: null, body: body);
                        Console.WriteLine(" [x] Sent: {0}", message);

                        Console.WriteLine("Type [end] and press [enter] to exit.");
                        //Console.ReadLine(); 
                    }
                }
            }
        }

        static void SendJson()
        {
            string usrinput = string.Empty;

            Console.WriteLine(">>> Let's send someone's profile as a Json! What is his name?");
            usrinput = Console.ReadLine();

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                //this part creates the queue
                channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);

                //note that this should have double quotes to be valid Json
                //but the Parse method takes care of that.
                string json = @"{
                                  Name: '" + usrinput + @"',
                                  Hobbies: [
                                    'Soccer',
                                    'Reading',
                                    'Gaming',
                                    'Poetry'
                                  ]
                                }";
                var JsonObject = JObject.Parse(json);
                Console.WriteLine($"Name value in the Json object is: '{JsonObject.GetValue("Name")}' ");
                Console.WriteLine("Now lets print the whole thing.");
                Console.WriteLine(JsonObject.ToString());
                Console.WriteLine("Ok, that was fun, now let's just send it as a string");
                var body = Encoding.UTF8.GetBytes(JsonObject.ToString());

                channel.BasicPublish(exchange: "", routingKey: "hello", basicProperties: null, body: body);
                Console.WriteLine(" [x] Sent: {0}", JsonObject.ToString());

                Console.WriteLine("Press [enter] to exit.");
                Console.ReadLine(); 
            }

        }

        static void SendXml()
        {
            string usrinput = string.Empty;

            Console.WriteLine(">>> Let's send someone's profile as a Xml! What is her/his name?");
            usrinput = Console.ReadLine();

            Console.WriteLine(">>> And what is her/his pet's name?");
            string petname = Console.ReadLine();

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                //this part creates the queue
                channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);

                //note that this should have double quotes to be valid Json
                //but the Parse method takes care of that.
                string xml = $"<persons><person name=\"{usrinput}\"><pet name=\"{petname}\">cat</pet></person></persons>";
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xml);
                Console.WriteLine($"Person name value in the Xml object is: '{xmlDoc.GetElementsByTagName("person")[0].Attributes["name"].Value}' and cat name =  {xmlDoc.GetElementsByTagName("person")[0].FirstChild.Attributes["name"].Value}");
                Console.WriteLine("Now lets print the whole thing.");
                Console.WriteLine(xmlDoc.OuterXml);
                Console.WriteLine("Ok, that was fun, now let's just send it as a string");
                var body = Encoding.UTF8.GetBytes(xmlDoc.OuterXml);

                channel.BasicPublish(exchange: "", routingKey: "hello", basicProperties: null, body: body);
                Console.WriteLine(" [x] Sent: {0}", xmlDoc.OuterXml);

                Console.WriteLine("Press [enter] to exit.");
                Console.ReadLine();
            }

        }

        static void XmlTry()
        {
            //https://docs.microsoft.com/en-us/dotnet/api/system.xml.xmldocument?view=netframework-4.7.2
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml("<user name=\"John Doe\">A user node</user>");
            Console.WriteLine(xmlDoc.DocumentElement.GetAttribute("name"));
            Console.ReadKey();

            XmlDocument xmlDoc2 = new XmlDocument();
            xmlDoc2.LoadXml("<users><user hobby=\"football\">InnerText/InnerXml is here</user><user hobby=\"reading\">User2</user></users>");
            Console.WriteLine("OuterXml: " + xmlDoc2.DocumentElement.OuterXml);
            Console.WriteLine("InnerXml: " + xmlDoc2.DocumentElement.InnerXml);
            Console.WriteLine("InnerText: " + xmlDoc2.DocumentElement.InnerText);
            Console.WriteLine("Hobby: " + xmlDoc2.DocumentElement.ChildNodes[0].Attributes["hobby"].Value);
            Console.ReadKey();

            XmlNode root = xmlDoc.FirstChild;

            if (root.HasChildNodes)
            {
                for (int i = 0; i < root.ChildNodes.Count; i++)
                {
                    Console.WriteLine("Child node innertext: " + root.ChildNodes[i].InnerText);
                }
            }

            Console.ReadKey();

            XmlNodeList elemList = xmlDoc2.GetElementsByTagName("user");
            for (int i = 0; i < elemList.Count; i++)
            {
                Console.WriteLine(elemList[i].InnerXml);
            }
            Console.ReadKey();

            XmlNode xmlNode = xmlDoc2.DocumentElement.SelectSingleNode("user");
            Console.WriteLine("This is when we try it with Xpath: " + xmlNode.InnerText);
            Console.ReadKey();
        }

    }
}
