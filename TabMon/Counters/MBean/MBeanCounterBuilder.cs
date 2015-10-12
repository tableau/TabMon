using System;
using TabMon.Helpers;

namespace TabMon.Counters.MBean
{
    /// <summary>
    /// Builder class to clean up instantiation of AbstractMBeanCounter-derived classes.
    /// </summary>
    public class MBeanBuilder
    {
        private IMBeanClient Client { get; set; }
        private Host Host { get; set; }
        private string Source { get; set; }
        private string Path { get; set; }
        private string Category { get; set; }
        private string Counter { get; set; }
        private string Instance { get; set; }
        private string Unit { get; set; }

        public MBeanBuilder CreateCounter(Host host)
        {
            Host = host;
            return this;
        }

        [CLSCompliant(false)]
        public MBeanBuilder UsingClient(IMBeanClient client)
        {
            Client = client;
            return this;
        }

        public MBeanBuilder WithSourceName(string source)
        {
            Source = source;
            return this;
        }

        public MBeanBuilder WithPath(string path)
        {
            Path = path;
            return this;
        }

        public MBeanBuilder WithCategoryName(string category)
        {
            Category = category;
            return this;
        }

        public MBeanBuilder WithCounterName(string counterName)
        {
            Counter = counterName;
            return this;
        }

        public MBeanBuilder WithInstanceName(string instance)
        {
            Instance = instance;
            return this;
        }

        public MBeanBuilder WithUnit(string unit)
        {
            Unit = unit;
            return this;
        }

        [CLSCompliant(false)]
        public AbstractMBeanCounter Build(string type)
        {
            switch (type.ToLower())
            {
                case "tableauhealth":
                    return new TableauHealthCounter(Client, Host, Source, Path, Category, Counter, Instance, Unit);

                case "jvmhealth":
                    return new JavaHealthCounter(Client, Host, Source, Path, Category, Counter, Instance, Unit);

                default:
                    throw new ArgumentException(String.Format("Invalid type name '{0}'", type));
            }
        }
    }
}
