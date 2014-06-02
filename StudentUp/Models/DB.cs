using System;
using System.Collections;
using MySql.Data.MySqlClient;
using System.Data;
using System.Configuration;

namespace StudentUp.Models
{
	/// <summary>
	/// Класс описывающий соединение с БД
	/// </summary>
	public class DB
	{
		/// <summary>
		/// Класс описывающий результат запроса представленный в виде таблици
		/// </summary>
		public class ResponseTable : IEnumerable, IEnumerator
		{
			/// <summary>
			/// Класс описывающий строку результирующей таблици
			/// </summary>
			/// <typeparam name="T">Тип елементов находщийхся в ячейке строки</typeparam>
			class TableRow<T>
			{
				/// <summary>
				/// Масив ячеек строки
				/// </summary>
				T[] cells = new T[0];

				/// <summary>
				/// Конструктор класса
				/// </summary>
				/// <param name="n">Количество ячеек</param>
				public TableRow(int n)
				{
					Array.Resize(ref this.cells, n);
				}

				/// <summary>
				/// Конструктор класса
				/// </summary>
				/// <param name="data">Данные почученные из запроса</param>
				public TableRow(MySqlDataReader data)
				{
					Array.Resize(ref this.cells, data.FieldCount);
					for (int i = 0, end = this.cells.Length; i < end; i++)
						if (data[i] is DBNull)
							this.cells[i] = default(T);
						else
							this.cells[i] = (T)data[i];
				}

				/// <summary>
				/// Возвращает количество ячеек
				/// </summary>
				public int Length { get { return this.cells.Length; } }

				/// <summary>
				/// Возвращает или устанавливает зачение ячейки с указанным номером
				/// </summary>
				/// <param name="n">Номер ячейки</param>
				/// <returns>Значение ячейки</returns>
				public T this[int n]
				{
					get { return this.cells[n]; }
					set { this.cells[n] = value; }
				}
			}

			/// <summary>
			/// Масив имен солонок таблици
			/// </summary>
			readonly TableRow<string> tableColumnName;

			/// <summary>
			/// Сама таблица
			/// </summary>
			readonly TableRow<object>[] table = new TableRow<object>[0];

			/// <summary>
			/// Номер текущей строки
			/// </summary>
			int currentRow = -1;

			/// <summary>
			/// Конструктор класса
			/// </summary>
			/// <param name="data">Данные почученные из запроса</param>
			public ResponseTable(MySqlDataReader data)
			{
				int currentNumberRow = 1;
				this.tableColumnName = new TableRow<string>(data.FieldCount);
				for (int i = 0, end = this.tableColumnName.Length; i < end; i++)
					this.tableColumnName[i] = data.GetName(i);
				for (int i = 0; data.Read(); i++)
				{
					if (this.table.Length <= currentNumberRow) Array.Resize(ref this.table, this.table.Length + 10);
					this.table[i] = new TableRow<object>(data);
					currentNumberRow++;
				}
				Array.Resize(ref this.table, currentNumberRow - 1);
			}

			/// <summary>
			/// Возвращает или устанавливает номер текущей строки
			/// </summary>
			public int CurrentRow
			{
				get { return this.currentRow; }
				set { this.currentRow = value; }
			}

			/// <summary>
			/// Возвращает количество строк в таблице
			/// </summary>
			public int CountRow { get { return this.table.Length; } }

			/// <summary>
			/// Возвращает количество столбцов в таблице
			/// </summary>
			public int CountColumn { get { return this.table[0].Length; } }

			/// <summary>
			/// Возвращает значение ячейки расположенной на пересечении текущей строки и указанного столбца
			/// </summary>
			/// <param name="n">Номер столбца</param>
			/// <returns>Значение ячейки</returns>
			public object this[int n] { get { return this.table[currentRow][n]; } }

			/// <summary>
			/// Возвращает значение ячейки расположенной на пересечении текущей строки и указанного столбца
			/// </summary>
			/// <param name="nameColumn">Имя столбца</param>
			/// <returns>Значение ячейки</returns>
			public object this[string nameColumn]
			{
				get
				{
					for (int i = 0, end = this.tableColumnName.Length; i < end; i++)
						if (this.tableColumnName[i].Equals(nameColumn)) return this[i];
					return null;
				}
			}

			/// <summary>
			/// Перейти на следующую строку таблици
			/// </summary>
			/// <returns>Был ли произведенн переход</returns>
			public bool Read()
			{
				if (++currentRow < this.table.Length) return true;
				return false;
			}

			/// <summary>
			/// Возвращает имя столбца
			/// </summary>
			/// <param name="n">Номер столбца</param>
			/// <returns>Имя столбца</returns>
			public string GetNameColumn(int n)
			{
				return this.tableColumnName[n];
			}

			/*Реализация для foreach (проход по столбцам)*/
			/// <summary>
			/// Номер текущего столбца
			/// </summary>
			int currentColumn = -1;

			/// <summary>
			/// Возвращает текущий объект преобразованный к IEnumerator
			/// </summary>
			/// <returns>Текущий объект преобразованный к IEnumerator</returns>
			public IEnumerator GetEnumerator()
			{
				return this;
			}

			/// <summary>
			/// Возвращает значение текущего столбца
			/// </summary>
			public object Current
			{
				get { return this[currentColumn]; }
			}

			/// <summary>
			/// Переходит на следующий столбец
			/// </summary>
			/// <returns>Был ли произведен переход</returns>
			public bool MoveNext()
			{
				if (++this.currentColumn < this.CountColumn) return true;
				this.Reset();
				return false;
			}

			/// <summary>
			/// Устанавливает указатель на первый столбец
			/// </summary>
			public void Reset()
			{
				this.currentColumn = -1;
			}
		}

		/// <summary>
		/// Дескриптор соеденения с БД
		/// </summary>
		readonly MySqlConnection db;

		/// <summary>
		/// Конструктор класса
		/// </summary>
		/// <param name="databaseName">Имя необходимой БД</param>
		/// <param name="URLServer">Адресс БД</param>
		/// <param name="serverPort">порт БД</param>
		/// <param name="userName">имя пользователя</param>
		/// <param name="userPassword">пароль пользователя</param>
		public DB(string databaseName, string URLServer, string serverPort, string userName, string userPassword)
		{
			this.db = new MySqlConnection("Database=" + databaseName +
										  ";Data Source=" + URLServer +
										  ";Port=" + serverPort +
										  ";User Id=" + userName +
										  ";Password=" + userPassword);
		}

		/// <summary>
		/// Конструктор класса. Создается по конфигу приложения
		/// </summary>
		public DB()
		{
			this.db = new MySqlConnection("Database=" + ConfigurationManager.AppSettings.Get("DBname") +
										  ";Data Source=" + ConfigurationManager.AppSettings.Get("DBip") +
										  ";Port=" + ConfigurationManager.AppSettings.Get("DBport") +
										  ";User Id=" + ConfigurationManager.AppSettings.Get("DBuser") +
										  ";Password=" + ConfigurationManager.AppSettings.Get("DBpassword"));
		}

		/// <summary>
		/// Деструктор класса, автоматически отсоеденяется от БД
		/// </summary>
		~DB()
		{
			this.Close();
		}

		/// <summary>
		/// Открывает соединение с БД если оно закрыто
		/// </summary>
		public void Open()
		{
			if (this.db.State == System.Data.ConnectionState.Closed) this.db.Open();
		}

		/// <summary>
		/// Закрывает соединение с БД если оно открыто
		/// </summary>
		public void Close()
		{
			if (this.db.State == System.Data.ConnectionState.Open) this.db.Close();
		}

		/// <summary>
		/// Выполняет переданный запрос без закрытия соединения
		/// </summary>
		/// <param name="request">Запрос в БД</param>
		/// <returns>Текущае соединение</returns>
		public MySqlDataReader Query(string request)
		{
			this.Open();
			return (new MySqlCommand
			{
				CommandText = request,
				Connection = this.db
			}).ExecuteReader(CommandBehavior.CloseConnection);
		}

		/// <summary>
		/// Выполняет переданный запрос с закрытием соединения и сохранением данных
		/// </summary>
		/// <param name="request">Запрос в БД</param>
		/// <returns>Результат запроса</returns>
		public ResponseTable QueryToRespontTable(string request)
		{
			ResponseTable result = null;
			MySqlDataReader data = this.Query(request);
			if (data.HasRows) result = new ResponseTable(data);
			this.Close();
			return result;
		}
	}
}