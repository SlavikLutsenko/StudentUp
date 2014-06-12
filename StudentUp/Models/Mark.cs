using System;

namespace StudentUp.Models
{
	/// <summary>
	/// Класс описывающий оценку
	/// </summary>
	public class Marks : IPartInstitute
	{
		/// <summary>
		/// За что была поставленна оценка
		/// </summary>
		public enum TypeMarks
		{
			/// <summary>
			/// Саостоятельная
			/// </summary>
			independent,
			/// <summary>
			/// Контрольная
			/// </summary>
			control,
			/// <summary>
			/// Лабораторная
			/// </summary>
			laboratory,
			/// <summary>
			/// Модуль
			/// </summary>
			module,
			/// <summary>
			/// Тест
			/// </summary>
			test,
			/// <summary>
			/// За работу на паре
			/// </summary>
			workOnPair
		}

		/// <summary>
		/// Индетификатор оценки
		/// </summary>
		readonly int markID = -1;

		/// <summary>
		/// Индетификатор студента
		/// </summary>
		int studentID = -1;

		/// <summary>
		/// Индетификатор предмета
		/// </summary>
		int subjectID = -1;

		/// <summary>
		/// Дата выставления
		/// </summary>
		DateTime date;

		/// <summary>
		/// Сама оценка
		/// </summary>
		int mark = 0;

		/// <summary>
		/// Бонусные балы
		/// </summary>
		int bonusMark = 0;

		/// <summary>
		/// максивальная оценка
		/// </summary>
		int maxMark = 0;

		/// <summary>
		/// Тип оценки
		/// </summary>
		Marks.TypeMarks typeMark;

		/// <summary>
		/// КОнструктор класса
		/// </summary>
		/// <param name="newMarkID">Индетификатор оценки</param>
		public Marks(int newMarkID)
		{
			if (newMarkID <= 0) throw new ValidationDataException("no id");
			this.markID = newMarkID;
		}

		/// <summary>
		/// Возвращает индетификатор оценки
		/// </summary>
		public int ID { get { return this.markID; } }

		/// <summary>
		/// Возвращает индетификатор студента
		/// </summary>
		public int StudentID { get { return this.studentID; } }

		/// <summary>
		/// Возвращает индетификатор предмета
		/// </summary>
		public int SubjectID { get { return this.subjectID; } }

		/// <summary>
		/// ВОзвращает саму оценку
		/// </summary>
		public int Mark { get { return this.mark; } }

		/// <summary>
		/// Возвращает бонусные балы
		/// </summary>
		public int BonusMark { get { return this.bonusMark; } }

		/// <summary>
		/// Возращает максимальную оценку
		/// </summary>
		public int MaxMark { get { return this.maxMark; } }

		/// <summary>
		/// Возвращает дату выставления оценки
		/// </summary>
		public DateTime Date { get { return this.date; } }

		/// <summary>
		/// ВОзвращает тип оценки
		/// </summary>
		public Marks.TypeMarks TypeMark { get { return this.typeMark; } }

		/// <summary>
		/// Проверяет была ли выставлена такая оценка
		/// </summary>
		/// <returns>true - есть, false - нету</returns>
		public bool IsExistsInDB()
		{
			DB db = new DB();
			DB.ResponseTable marks = db.QueryToRespontTable(string.Format("select * from Marks where Mark_id = {0}", this.markID));
			return marks != null && marks.CountRow == 1;
		}

		/// <summary>
		/// Берет всю информацию об оценке из БД
		/// </summary>
		/// <returns>true - есть, false - нету</returns>
		public bool GetInformationAboutUserFromDB()
		{
			DB db = new DB();
			DB.ResponseTable marks = db.QueryToRespontTable(string.Format("select * from marks inner join studentsubject on marks.StudentSubject_id = studentsubject.StudentSubject_id and marks.Mark_id = {0};", this.markID));
			if (marks != null && marks.CountRow == 1)
			{
				marks.Read();
				this.studentID = Convert.ToInt32(marks["Student_id"]);
				this.subjectID = Convert.ToInt32(marks["Subject_id"]);
				this.date = Convert.ToDateTime(marks["Date"]);
				this.mark = Convert.ToInt32(marks["Mark"]);
				this.bonusMark = Convert.ToInt32(marks["Bonus_mark"]);
				this.maxMark = Convert.ToInt32(marks["Max_mark"]);
				this.typeMark = Marks.ConverStringToEnum((string)marks["Type_marks"]);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Преобразовывает элемент перчисления в строку
		/// </summary>
		/// <param name="value">Элемент перечисления</param>
		/// <returns>Строковая интерпритация элемнта перечисления</returns>
		public static string GetEnumDescription(Marks.TypeMarks value)
		{
			switch (value)
			{
				case Marks.TypeMarks.independent:
					return "самостійна";
				case Marks.TypeMarks.control:
					return "контрольна";
				case Marks.TypeMarks.laboratory:
					return "лабораторна";
				case Marks.TypeMarks.module:
					return "модульна";
				case Marks.TypeMarks.test:
					return "тест";
				case Marks.TypeMarks.workOnPair:
					return "за роботу на парі";
			}
			return null;
		}
		//enum('самостійна', 'контрольна', 'лабораторна', 'модульна', 'тест', 'за роботу на парі')
		/// <summary>
		/// Преобразовывает строку в элемент перчисления
		/// </summary>
		/// <param name="value">Строковая интерпритация элемнта перечисления</param>
		/// <returns>Элемент перечисления</returns>
		public static Marks.TypeMarks ConverStringToEnum(string value)
		{
			switch (value)
			{
				case "самостійна":
					return Marks.TypeMarks.independent;
				case "контрольна":
					return Marks.TypeMarks.control;
				case "лабораторна":
					return Marks.TypeMarks.laboratory;
				case "модульна":
					return Marks.TypeMarks.module;
				case "тест":
					return Marks.TypeMarks.test;
				case "за роботу на парі":
					return Marks.TypeMarks.workOnPair;
			}
			return Marks.TypeMarks.workOnPair;
		}

		/// <summary>
		/// Добовляет оценку в БД
		/// </summary>
		/// <param name="studentID">Идентификатор студента</param>
		/// <param name="subjectID">Идентификатор предмета</param>
		/// <param name="mark">Сама оценка</param>
		/// <param name="bonusMark">Бонусные балы</param>
		/// <param name="maxMark">Максимальная оценка</param>
		/// <param name="typeMark">Тип оценки</param>
		/// <param name="date">Дата выставления оценки</param>
		/// <returns>Новыя оценка</returns>
		public static Marks AddMark(int studentID, int subjectID, int mark, int bonusMark, int maxMark, Marks.TypeMarks typeMark, DateTime date)
		{
			DB db = new DB();
			DB.ResponseTable StudentSubject = db.QueryToRespontTable(string.Format("select StudentSubject_id from studentsubject where Student_id = {0} and Subject_id = {1};", studentID, subjectID));
			StudentSubject.Read();
			db.QueryToRespontTable(string.Format("insert into Marks(Date, Mark, Bonus_mark, Max_mark, StudentSubject_id, Type_marks) value ('{0}', {1}, {2}, {3}, {4}, '{5}')", date.ToShortDateString(), mark, bonusMark, maxMark, Convert.ToInt32(StudentSubject["StudentSubject_id"]), Marks.GetEnumDescription(typeMark)));
			DB.ResponseTable markID = db.QueryToRespontTable("select LAST_INSERT_ID() as id;");
			markID.Read();
			Marks currentMark = new Marks(Convert.ToInt32(markID["id"]));
			currentMark.GetInformationAboutUserFromDB();
			return currentMark;
		}
	}
}