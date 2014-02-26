using NHibernate.Event;

namespace DevelopmentWithADot.NHibernateFluentValidation
{
	sealed class FlushEntityValidatorListener : IFlushEntityEventListener
	{
		#region IFlushEntityEventListener Members

		public void OnFlushEntity(FlushEntityEvent @event)
		{
			var validator = @event.Session.SessionFactory.GetValidator();

			if (validator != null)
			{
				validator.Validate(@event.Entity);
			}
		}

		#endregion
	}
}
