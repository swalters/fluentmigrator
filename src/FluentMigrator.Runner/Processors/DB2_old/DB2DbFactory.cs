using System;
using System.Data.Common;
using System.Reflection;

namespace FluentMigrator.Runner.Processors.DB2
{
    public class DB2DbFactory : ReflectionBasedDbFactory
    {
    	private const string AssemblyName = "IBM.Data.DB2";
    	private const string ClassName = "IBM.Data.DB2.DB2Factory";

    	public DB2DbFactory()
			: base(AssemblyName, ClassName)
        {
        }

		protected override DbProviderFactory CreateFactory()
		{
			//Assembly a = Assembly.LoadFrom(AssemblyName);
			//Type t = a.GetType(ClassName);

			//object o = t.GetMethod("Instance").Invoke(null, new object[] { });
			//return (DbProviderFactory)o;
			DbProviderFactory factory = DbProviderFactories.GetFactory("IBM.Data.DB2");
			return factory;
		}
    }
}