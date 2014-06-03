using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace StudentUp.Models
{
	/// <summary>
	/// Класс описывающий список сообщений пользователю
	/// </summary>
	public class Messages: IEnumerable, IEnumerator
	{
		/// <summary>
		/// Класс описывающий само сообщение
		/// </summary>
		public class Message
		{
			/// <summary>
			/// Тип сообщения
			/// </summary>
			public enum TypeMessage
			{
				/// <summary>
				/// Сообщение об успехе
				/// </summary>
				good,
				/// <summary>
				/// Сообщение об ошибке
				/// </summary>
				error
			}

			/// <summary>
			/// Тип сообщения
			/// </summary>
			readonly TypeMessage type;
			
			/// <summary>
			/// Само сообщение
			/// </summary>
			readonly string value = string.Empty;

			/// <summary>
			/// Конструктор класса
			/// </summary>
			/// <param name="newType">Тип сообщения</param>
			/// <param name="newValue">Само сообщение</param>
			public Message(TypeMessage newType, string newValue)
			{
				this.type = newType;
				this.value = newValue;
			}

			/// <summary>
			/// Возвращает тип сообщения
			/// </summary>
			public TypeMessage Type { get { return this.type; } }

			/// <summary>
			/// Возвращает само сообщение
			/// </summary>
			public string Value { get { return this.value; } }
		}

		/// <summary>
		/// Список сообщений
		/// </summary>
		readonly List<Message> value = new List<Message>();

		/// <summary>
		/// Количество сообщений
		/// </summary>
		public int Count { get { return this.value.Count; } }

		/// <summary>
		/// Возвращает сообщений под указаным номером начиная с 0
		/// </summary>
		/// <param name="n">Номер требуемого сообщения</param>
		/// <returns>Само сообщение</returns>
		public Message this[int n]{	get	{	return this.value[n];	}	}

		/// <summary>
		/// Бобовляет сообщение
		/// </summary>
		/// <param name="newMessage">Новое сообщение</param>
		/// <returns>Добавленное сообщение</returns>
		public Message Add(Message newMessage)
		{
			this.value.Add(newMessage);
			return newMessage;
		}

		/// <summary>
		/// Бобовляет сообщение
		/// </summary>
		/// <param name="newType">Тип сообщения</param>
		/// <param name="newMessage">Само сообщение</param>
		/// <returns>Добавленное сообщение</returns>
		public Message Add(Message.TypeMessage newType, string newMessage)
		{
			this.value.Add(new Message(newType, newMessage));
			return this.value.Last();
		}

		//для foreach
		/// <summary>
		/// Номер текущего сообщения
		/// </summary>
		int currentMessage = -1;

		/// <summary>
		/// Возвращает текущий объект преобразованный к IEnumerator
		/// </summary>
		/// <returns>Текущий объект преобразованный к IEnumerator</returns>
		public IEnumerator GetEnumerator()
		{
			return this;
		}

		/// <summary>
		/// Возвращает текущее сообщение
		/// </summary>
		public object Current
		{
			get { return this[this.currentMessage]; }
		}

		/// <summary>
		/// Переходит на следущее сообщение если это возможно иначе сбрасывает счетчик
		/// </summary>
		/// <returns>Возвращает true если переход был очущетвлен иначе false</returns>
		public bool MoveNext()
		{
			if (++this.currentMessage < this.value.Count) return true;
			Reset();
			return false;
		}

		/// <summary>
		/// Сбрасывает счетчик в начальное положение
		/// </summary>
		public void Reset()
		{
			this.currentMessage = -1;
		}
	}
}