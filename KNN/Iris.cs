namespace KNN
{
    public class Iris
    {
        public double Value1 { get; set; }
        public double Value2 { get; set; }
        public double Value3 { get; set; }
        public double Value4 { get; set; }
        public string Name { get; set; }

        public string DescribeName
        {
            get
            {
                if (Name == "1")
                {
                    return "Setosa";
                }
                else if (Name == "2")
                {
                    return "Versicolour";
                }
                else if (Name == "3")
                {
                    return "Virginica";
                }

                return string.Empty;
            }
        }

        public Iris()
        { }

        public Iris(double v1, double v2, double v3, double v4, string name)
        {
            Value1 = v1;
            Value2 = v2;
            Value3 = v3;
            Value4 = v4;
            Name = name;
        }

        public override string ToString()
        {
            return $"{Name} - {DescribeName}";
        }
    }
}
