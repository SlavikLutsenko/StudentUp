using System;

namespace StudentUp.Models
{
	/// <summary>
	/// Описывает предмет у преподователя
	/// </summary>
	public class Subject : IPartInstitute
	{
		/// <summary>
		/// Тип сдачи предмета
		/// </summary>
		public enum ExamType
		{
			/// <summary>
			/// Не требуется сдача
			/// </summary>
			nothing,
			/// <summary>
			/// Екзамен
			/// </summary>
			exam,
			/// <summary>
			/// Диференцированный зачет
			/// </summary>
			differentiated_offset,
			/// <summary>
			/// Зачет
			/// </summary>
			offset
		}

		/// <summary>
		/// Идентификатор предмета
		/// </summary>
		int subjectID = -1;

		/// <summary>
		/// Название предмета
		/// </summary>
		string name;

		/// <summary>
		/// Индетификатор преподователя, ведущий этот предмет
		/// </summary>
		int lecturerID = -1;

		/// <summary>
		/// Тип сдачи предмета
		/// </summary>
		ExamType typeExam;

		/// <summary>
		/// Конструктор класса
		/// </summary>
		/// <param name="newSubjectID">Идетификатор предмета</param>
		public Subject(int newSubjectID)
		{
			this.subjectID = newSubjectID;
		}

		/// <summary>
		/// Возвращает идетификатор предмета
		/// </summary>
		public int ID { get { return this.subjectID; } }

		/// <summary>
		/// Возвращает название предмета
		/// </summary>
		public string Name { get { return this.name; } }

		/// <summary>
		/// Возвращает идентифкатор преподователя ведущий этот предмет
		/// </summary>
		public int LecturerID { get { return this.lecturerID; } }

		/// <summary>
		/// Возвращает тип сдачи предмета
		/// </summary>
		public ExamType TypeExam { get { return this.typeExam; } }

		/// <summary>
		/// Проверяет есть ли такой предмет в БД
		/// </summary>
		/// <returns>true - есть, false - нету</returns>
		public bool IsExistsInDB()
		{
			DB db = new DB();
			DB.ResponseTable subject = db.QueryToRespontTable(string.Format("select * from Subject where Subject_id = {0};", this.subjectID));
			return subject != null && subject.CountRow == 1;
		}

		/// <summary>
		/// Берет информацию о предмете из БД
		/// </summary>
		/// <returns>true - есть, false - нету</returns>
		public bool GetInformationAboutUserFromDB()
		{
			DB db = new DB();
			DB.ResponseTable subject = db.QueryToRespontTable(string.Format("select * from Subject where Subject_id = {0};", this.subjectID));
			if (subject != null && subject.CountRow > 0)
			{
				subject.Read();
				this.subjectID = Convert.ToInt32(subject["Subject_id"]);
				this.name = (string)subject["Name"];
				this.lecturerID = Convert.ToInt32(subject["Lecturer_id"]);
				this.typeExam = Subject.ConverStringToEnum((string)subject["Exam_type"]);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Преобразовывает элемент перчисления в строку
		/// </summary>
		/// <param name="value">Элемент перечисления</param>
		/// <returns>Строковая интерпритация элемнта перечисления</returns>
		public static string GetEnumDescription(Subject.ExamType value)
		{
			switch (value)
			{
				case Subject.ExamType.nothing:
					return "-";
				case Subject.ExamType.exam:
					return "іспит";
				case Subject.ExamType.differentiated_offset:
					return "диференційований залік";
				case Subject.ExamType.offset:
					return "залік";
			}
			return null;
		}

		/// <summary>
		/// Преобразовывает строку в элемент перчисления
		/// </summary>
		/// <param name="value">Строковая интерпритация элемнта перечисления</param>
		/// <returns>Элемент перечисления</returns>
		public static Subject.ExamType ConverStringToEnum(string value)
		{
			switch (value)
			{
				case "-":
					return Subject.ExamType.nothing;
				case "іспит":
					return Subject.ExamType.exam;
				case "диференційований залік":
					return Subject.ExamType.differentiated_offset;
				case "залік":
					return Subject.ExamType.offset;
			}
			return Subject.ExamType.nothing;
		}

		/// <summary>
		/// Возвращает все группы студентов которые изучают этот предмет
		/// </summary>
		/// <returns>Масив групп</returns>
		public Group[] GetGroups()
		{
			Group[] result = null;
			DB db = new DB();
			DB.ResponseTable groupID = db.QueryToRespontTable(string.Format("select groups.Group_id from groups inner join student inner join StudentSubject inner join subject on groups.Group_id = student.Group_id and student.Student_id = StudentSubject.Student_id and StudentSubject.Subject_id = subject.Subject_id and subject.Subject_id = {0} Group by groups.Group_id order by groups.Name;", this.subjectID));
			if (groupID != null)
			{
				result = new Group[groupID.CountRow];
				for (int i = 0, end = result.Length; i < end && groupID.Read(); i++)
				{
					result[i] = new Group(Convert.ToInt32(groupID["Group_id"]));
					result[i].GetInformationAboutUserFromDB();
				}
			}
			return result;
		}

		/// <summary>
		/// Возвращает всех студентов группы которые изучают этот предмет
		/// </summary>
		/// <param name="groupID">Идентификатор группы</param>
		/// <returns>Масив студентов</returns>
		public Student[] GetStudentsFromGroup(int groupID)
		{
			Student[] result = null;
			DB db = new DB();
			DB.ResponseTable studentID = db.QueryToRespontTable(string.Format("select student.Student_id from groups inner join student inner join StudentSubject inner join subject on groups.Group_id = student.Group_id and groups.Group_id = {0} and student.Student_id = StudentSubject.Student_id and StudentSubject.Subject_id = subject.Subject_id and subject.Subject_id = {1};", groupID, this.subjectID));
			if (studentID != null)
			{
				result = new Student[studentID.CountRow];
				for (int i = 0, end = result.Length; i < end && studentID.Read(); i++)
				{
					result[i] = new Student(Convert.ToInt32(studentID["Student_id"]));
					result[i].GetInformationAboutUserFromDB();
				}
			}
			return result;
		}

		/// <summary>
		/// Устанавливает студентов на этот предмет
		/// </summary>
		/// <param name="students">Масив индетификаторв студентов</param>
		public void SetStudent(int[] students)
		{
			DB db = new DB();
			string query = "insert into StudentSubject(Subject_id, Student_id) values ";
			for (int i = 0, end = students.Length; i < end; i++)
			{
				query += string.Format("({0}, {1})", this.subjectID, students[i]);
				if (i < end - 1) query += ",";
				else query += ";";
			}
			db.QueryToRespontTable(query);
		}

		/// <summary>
		/// Добовляет предмет в БД
		/// </summary>
		/// <param name="name">Название предмета</param>
		/// <param name="lecturerID">Идентификатор преподователя ведущий этот предмет</param>
		/// <param name="examType">Тип сдачи предмета</param>
		/// <returns>Новый предмет</returns>
		public static Subject AddSubject(string name, int lecturerID, string examType)
		{
			DB db = new DB();
			if (Subject.ConverStringToEnum(examType) == ExamType.nothing && examType != "-") return null;
			db.QueryToRespontTable(string.Format("insert into Subject(Lecturer_id, Name, Exam_type) value ({0}, '{1}', '{2}');", lecturerID, name, examType));
			DB.ResponseTable idSubject = db.QueryToRespontTable("select LAST_INSERT_ID() as id;");
			idSubject.Read();
			Subject subject = new Subject(Convert.ToInt32(idSubject["id"]));
			subject.GetInformationAboutUserFromDB();
			return subject;
		}
	}
}