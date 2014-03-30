using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DevelopmentWithADot.NHibernateFluentValidation
{
	public sealed class FluentValidation
	{
		private readonly IDictionary<Type, Dictionary<Delegate, String>> conditions = new ConcurrentDictionary<Type, Dictionary<Delegate, String>>();

		internal FluentValidation()
		{
			this.Enabled = true;
		}

		public Boolean Enabled
		{
			get;
			set;
		}

		public void Clear<T>()
		{
			foreach (var type in this.conditions.Where(x => typeof(T).IsAssignableFrom(x.Key)).Select(x => x.Key))
			{
				this.conditions.Remove(type);
			}
		}

		public FluentValidation Entity<T>(Func<T, Boolean> condition, String message)
		{
			if (this.conditions.ContainsKey(typeof(T)) == false)
			{
				this.conditions[typeof(T)] = new Dictionary<Delegate, String>();
			}

			this.conditions[typeof(T)][condition] = message;			

			return (this);
		}

		public void Validate(Object entity)
		{
			if (this.Enabled == true)
			{
				var applicableConditions = this.conditions.Where(x => entity.GetType().IsAssignableFrom(x.Key)).Select(x => x.Value);

				foreach (var applicableCondition in applicableConditions)
				{
					foreach (var condition in applicableCondition)
					{
						var del = condition.Key;

						if (Object.Equals(del.DynamicInvoke(entity), false) == true)
						{
							throw (new ValidationException(entity, condition.Value));
						}
					}
				}
			}
		}
	}
}
