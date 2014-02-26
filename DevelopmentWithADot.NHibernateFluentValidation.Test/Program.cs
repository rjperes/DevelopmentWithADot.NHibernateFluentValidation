using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Mapping.ByCode;

namespace DevelopmentWithADot.NHibernateFluentValidation.Test
{
	class Xpto
	{
		public Int32 Id { get; set; }
		public String Name { get; set; }
	}
	class Program
	{
		static void Main(String[] args)
		{
			var cfg = new Configuration();
			cfg.DataBaseIntegration(x =>
				{
					x.Dialect<MsSql2008Dialect>();
					x.Driver<Sql2008ClientDriver>();
					x.ConnectionString = @"Data Source=(local)\SQLEXPRESS; Initial Catalog=NHibernate; Integrated Security=SSPI";
					x.SchemaAction = SchemaAutoAction.Update;
				})
				.SetProperty(NHibernate.Cfg.Environment.UseProxyValidator, Boolean.FalseString);

			var model = new ConventionModelMapper();
			model.BeforeMapClass += (a, b, c) => { c.Lazy(false); c.Id(x => x.Generator(Generators.Identity)); };

			var mappings = model.CompileMappingFor(new Type[] { typeof(Xpto) });

			cfg.AddMapping(mappings);

			using (var sessionFactory = cfg.BuildSessionFactory())
			{
				var validation = sessionFactory
					.FluentlyValidate()
					.Entity<Xpto>(x => String.IsNullOrWhiteSpace(x.Name), "Name is empty")
					.Entity<Xpto>(x => x.Id == 0, "Invalid id");

				using (var session = sessionFactory.OpenSession())
				using (var tx = session.BeginTransaction())
				{
					try
					{
						session.Save(new Xpto());
						session.Flush();
					}
					catch
					{
						//expected
					}

					//disable all validations
					//sessionFactory.DisableFluentValidation();
					
					//disable validations over the Xpto class
					validation.Clear<Xpto>();

					session.Save(new Xpto());
					session.Flush();
					//should work
				}
			}
		}
	}
}
