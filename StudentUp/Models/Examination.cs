using System;

namespace StudentUp.Models
{
	/// <summary>
	/// Класс описывающий результаты атестаций или сессии
	/// </summary>
	public class Examination : IPartInstitute
	{
		/// <summary>
		/// Тип оценки
		/// </summary>
		public enum ExamType
		{
			/// <summary>
			/// Первая атестация
			/// </summary>
			firstAttestation,
			/// <summary>
			/// Вторая атестация
			/// </summary>
			secondAttestation,
			/// <summary>
			/// Зачет
			/// </summary>
			assesment,
			/// <summary>
			/// Диференцированый зачет
			/// </summary>
			diffAssesment,
			/// <summary>
			/// Экзамен
			/// </summary>
			exam,
			/// <summary>
			/// Первая пересдача
			/// </summary>
			firstRetake,
			/// <summary>
			/// Вторая пересдача
			/// </summary>
			secondRetake
		}

		/// <summary>
		/// Идентификатор результата
		/// </summary>
		int examinationID = -1;

		/// <summary>
		/// Идентификатор студента
		/// </summary>
		int studentID = -1;

		/// <summary>
		/// Идентификатор предмета
		/// </summary>
		int subjectID = -1;

		/// <summary>
		/// Набраные балы
		/// </summary>
		int mark;

		/// <summary>
		/// Минимальное количество балов для положительного результата
		/// </summary>
		int minMark;

		/// <summary>
		/// Тип результата
		/// </summary>
		Examination.ExamType examType;

		/// <summary>
		/// Конструктор класса
		/// </summary>
		/// <param name="newExaminationID">Идентификатор результата</param>
		public Examination(int newExaminationID)
		{
			if (newExaminationID <= 0) throw new ValidationDataException("no id");
			this.examinationID = newExaminationID;
		}

		/// <summary>
		/// Возвращает идентификатор результата
		/// </summary>
		public int ID { get { return this.examinationID; } }

		/// <summary>
		/// Возвращает идентификатор студента
		/// </summary>
		public int StudentID { get { return this.studentID; } }

		/// <summary>
		/// Возвращает идентификатор предмета
		/// </summary>
		public int SubjectID { get { return this.subjectID; } }

		/// <summary>
		/// Возвращает набраные балы студентом
		/// </summary>
		public int Mark { get { return this.mark; } }

		/// <summary>
		/// Возвращает минимальное количество балов для положительного результата
		/// </summary>
		public int MinMark { get { return this.minMark; } }

		/// <summary>
		/// Возвращает тип результата
		/// </summary>
		public Examination.ExamType Type { get { return this.examType; } }

		/// <summary>
		/// Проверяет есть ли такой результат в БД
		/// </summary>
		/// <returns>true - есть, false - нету</returns>
		public bool IsExistsInDB()
		{
			DB db = new DB();
			DB.ResponseTable examination = db.QueryToRespontTable(string.Format("select * from examination where Examination.Examination_id = {0};", this.examinationID));
			return examination != null && examination.CountRow == 1;
		}

		/// <summary>
		/// Берет информацию о результате из БД
		/// </summary>
		/// <returns>true - есть, false - нету</returns>
		public bool GetInformationAboutUserFromDB()
		{
			DB db = new DB();
			DB.ResponseTable examination = db.QueryToRespontTable(string.Format("select * from examination inner join studentsubject on examination.StudentSubject_id = studentsubject.StudentSubject_id where Examination.Examination_id = {0};", this.examinationID));
			if (examination != null && examination.CountRow > 0)
			{
				examination.Read();
				this.examinationID = Convert.ToInt32(examination["Examination_id"]);
				this.studentID = Convert.ToInt32(examination["Student_id"]);
				this.subjectID = Convert.ToInt32(examination["Subject_id"]);
				this.mark = Convert.ToInt32(examination["Mark"]);
				this.minMark = Convert.ToInt32(examination["Min_mark"]);
				this.examType = Examination.ConverStringToEnum(examination["Exam_type"].ToString());
				return true;
			}
			return false;
		}

		/// <summary>
		/// Преобразовывает элемент перчисления в строку
		/// </summary>
		/// <param name="value">Элемент перечисления</param>
		/// <returns>Строковая интерпритация элемнта перечисления</returns>
		public static string GetEnumDescription(Examination.ExamType value)
		{
			switch (value)
			{
				case ExamType.firstAttestation:
					return "атестація1";
				case ExamType.secondAttestation:
					return "атестація2";
				case ExamType.assesment:
					return "залік";
				case ExamType.diffAssesment:
					return "диф-залік";
				case ExamType.exam:
					return "іспит";
				case ExamType.firstRetake:
					return "пересдача1";
				case ExamType.secondRetake:
					return "пересдача2";
			}
			return null;
		}

		/// <summary>
		/// Преобразовывает строку в элемент перчисления
		/// </summary>
		/// <param name="value">Строковая интерпритация элемнта перечисления</param>
		/// <returns>Элемент перечисления</returns>
		public static Examination.ExamType ConverStringToEnum(string value)
		{
			switch (value)
			{
				case "атестація1":
					return ExamType.firstAttestation;
				case "атестація2":
					return ExamType.secondAttestation;
				case "залік":
					return ExamType.assesment;
				case "диф-залік":
					return ExamType.diffAssesment;
				case "іспит":
					return ExamType.exam;
				case "пересдача1":
					return ExamType.firstRetake;
				case "пересдача2":
					return ExamType.secondRetake;
			}
			return Examination.ExamType.firstAttestation;
		}
	}
}