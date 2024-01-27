using System;
using System.Xml.Linq;
using Npgsql;

public class DatabaseToXmlConverter
{
    private string connectionString;

    public DatabaseToXmlConverter(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public XDocument CreateXmlFromDatabase()
    {
        // Create the root element and other necessary elements
        XNamespace ns = "http://www.w3.org/2001/06/grammar";
        XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";

        XElement rootElement = new XElement(ns + "grammar",
            new XAttribute("version", "1.0"),
            new XAttribute(XNamespace.Xml + "lang", "pl-PL"),
            new XAttribute("xmlns", ns),
            new XAttribute(XNamespace.Xmlns + "xsi", xsi),
            new XAttribute(xsi + "schemaLocation", "http://www.w3.org/2001/06/grammar http://www.w3.org/TR/speech-grammar/grammar.xsd"),
            new XAttribute("root", "rootRule"),
            new XElement(ns + "rule", new XAttribute("id", "rootRule"), new XAttribute("scope", "public"),
                new XElement(ns + "one-of",
                    new XElement(ns + "item",
                        new XElement(ns + "ruleref", new XAttribute("uri", "#slowo"))
                    )
                )
            )
        );

        XElement slowoElement = new XElement(ns + "rule", new XAttribute("id", "slowo"), new XAttribute("scope", "public"),
            new XElement(ns + "one-of"));
        rootElement.Add(slowoElement);

        // Connect to the PostgreSQL database and retrieve data
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            string query = "select slowo from sklep.slowa"; // Replace with your query
            NpgsqlCommand command = new NpgsqlCommand(query, connection);

            using (NpgsqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string word = reader.GetString(0); // Assumes the word is in the first column
                    slowoElement.Element(ns + "one-of").Add(new XElement(ns + "item", word));
                }
            }

        }

        // Create the XDocument and return it
        XDocument document = new XDocument(new XDeclaration("1.0", "UTF-8", null), rootElement);
        return document;
    }
}
