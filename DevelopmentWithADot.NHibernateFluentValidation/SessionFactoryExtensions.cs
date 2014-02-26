using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NHibernate;
using NHibernate.Event;
using NHibernate.Impl;

namespace DevelopmentWithADot.NHibernateFluentValidation
{
	public static class SessionFactoryExtensions
	{
		internal static readonly IDictionary<GCHandle, FluentValidation> validations = new ConcurrentDictionary<GCHandle, FluentValidation>();

		public static FluentValidation FluentlyValidate(this ISessionFactory sessionFactory)
		{
			var validation = GetValidator(sessionFactory);

			if (validation == null)
			{
				validation = Register(sessionFactory);
			}

			return (validation);
		}

		public static void DisableFluentValidation(this ISessionFactory sessionFactory)
		{
			Unregister(sessionFactory);
		}

		internal static FluentValidation GetValidator(this ISessionFactory sessionFactory)
		{
			return (validations.Where(x => x.Key.Target == sessionFactory).Select(x => x.Value).SingleOrDefault());
		}

		private static void Unregister(ISessionFactory sessionFactory)
		{
			var validation = validations.Where(x => x.Key.Target == sessionFactory).SingleOrDefault();

			if (Object.Equals(validation, null) == true)
			{
				validations.Remove(validation);
			}

			(sessionFactory as SessionFactoryImpl).EventListeners.FlushEntityEventListeners = (sessionFactory as SessionFactoryImpl).EventListeners.FlushEntityEventListeners.Where(x => !(x is FlushEntityValidatorListener)).ToArray();
		}

		private static FluentValidation Register(ISessionFactory sessionFactory)
		{
			var validation = (validations[GCHandle.Alloc(sessionFactory)] = new FluentValidation());
			(sessionFactory as SessionFactoryImpl).EventListeners.FlushEntityEventListeners = (sessionFactory as SessionFactoryImpl).EventListeners.FlushEntityEventListeners.Concat(new IFlushEntityEventListener[] { new FlushEntityValidatorListener() }).ToArray();
			return (validation);
		}
	}
}
