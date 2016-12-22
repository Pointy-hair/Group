using System;

namespace RevolutionaryStuff.PowerBiToys.Objects
{
    public class Tables
    {
        public table[] value { get; set; }
    }

    public class table
    {
        public string Name { get; set; }
    }

    public class group
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsReadOnly { get; set; }
        public override string ToString() => $"{GetType().Name} id={Id} name={Name} isReadOnly={IsReadOnly}";
    }


    public class Product
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public bool IsCompete { get; set; }
        public DateTime ManufacturedOn { get; set; }
    }

    //Example of a new Product schema
    public class Product2
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public bool IsCompete { get; set; }
        public DateTime ManufacturedOn { get; set; }

        public string NewColumn { get; set; }
    }

}
