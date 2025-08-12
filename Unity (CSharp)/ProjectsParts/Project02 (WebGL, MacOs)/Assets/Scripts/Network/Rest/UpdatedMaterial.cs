using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Network.Rest
{
	public class UpdatedMaterial
	{
		public UnityEngine.Events.UnityEvent<UpdatedMaterial> OnUpdateEvent = new UnityEngine.Events.UnityEvent<UpdatedMaterial>();

		public void Update(UpdatedMaterial updateMaterial)
		{
			var fieldsArr = this.GetType().GetFields();
			List<string> updateParams = new List<string>();

			for (int i = 0; i < fieldsArr.Length; i++)
			{
				var field = fieldsArr[i];
				object[] attributesField = field.GetCustomAttributes(typeof(UpdateAttribute), true);
				if (attributesField.Length > 0)
				{
					object oldValue = field.GetValue(this);
					object newValue = field.GetValue(updateMaterial);

					if (oldValue == null && newValue == null) continue;

					if ((oldValue == null && newValue != null)
					|| (oldValue != null && newValue == null)
					|| !newValue.Equals(oldValue)
					//|| (Convert.ChangeType(oldValue, field.FieldType) != Convert.ChangeType(newValue, field.FieldType))
					)
					{
						field.SetValue(this, field.GetValue(updateMaterial));
						updateParams.Add(field.Name);
					}
				}

			}
			EmitUpdateEvent();
		}

		protected virtual void EmitUpdateEvent()
		{
			OnUpdateEvent?.Invoke(this);
		}

	}
}