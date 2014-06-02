using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace StudentUp.Models
{
	public class Messages: IEnumerable, IEnumerator
	{
		public class Message
		{
			public enum TypeMessage
			{
				good,
				error
			}

			readonly TypeMessage type;

			readonly string value = string.Empty;

			public Message(TypeMessage newType, string newValue)
			{
				this.type = newType;
				this.value = newValue;
			}

			public TypeMessage Type { get { return this.type; } }

			public string Value { get { return this.value; } }
		}

		readonly List<Message> value = new List<Message>();

		public int Count { get { return this.value.Count; } }

		public Message this[int n]{	get	{	return this.value[n];	}	}

		public Message Add(Message newMessage)
		{
			this.value.Add(newMessage);
			return newMessage;
		}

		public Message Add(Message.TypeMessage newType, string newMessage)
		{
			this.value.Add(new Message(newType, newMessage));
			return this.value.Last();
		}

		//для foreach

		int currentMessage = -1;

		public IEnumerator GetEnumerator()
		{
			return this;
		}

		public object Current
		{
			get { return this[this.currentMessage]; }
		}

		public bool MoveNext()
		{
			if (++this.currentMessage < this.value.Count) return true;
			Reset();
			return false;
		}

		public void Reset()
		{
			this.currentMessage = -1;
		}
	}
}