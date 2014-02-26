using System;

namespace DevelopmentWithADot.NHibernateFluentValidation
{
	[Serializable]
	public sealed class ValidationException : Exception
	{
		public ValidationException(Object entity, String message) : base(message)
		{
			this.Entity = entity;
		}

		public Object Entity
		{
			get;
			private set;
		}		
	}
}
