using System;
using System.Web.Mvc;
using StudentUp.Models;

namespace StudentUp.Controllers
{
	/// <summary>
	/// Предназначен для выборки разной информации из БД
	/// </summary>
	public class SearchController : Controller
	{
		/// <summary>
		/// Возвращает страницу на которой выведены все студенты группы
		/// </summary>
		/// <param name="groupID">Идентификатор группы</param>
		/// <returns>Страница</returns>
		public ActionResult GetGroupOfStudents(int groupID)
		{
			ViewData["groupOfStudents"] = (new Group(groupID)).GetStudent();
			return View();
		}

		/// <summary>
		/// Возвращает страницу на которой выведены все предметы преподователя
		/// </summary>
		/// <param name="lecturerID">Идентификатор преподователя</param>
		/// <returns>Страница</returns>
		public ActionResult GetSubjectsLecturer(int lecturerID)
		{
			Lecturer lecturer = new Lecturer(lecturerID);
			lecturer.GetInformationAboutUserFromDB();
			ViewData["subjectsLecturer"] = lecturer.GetMySubjects();
			return View();
		}

		/// <summary>
		/// Возвращает все группы у которых преподается определенный предмет
		/// </summary>
		/// <param name="subjectID">Идентификатор предмета</param>
		/// <returns>Частичное представление</returns>
		public ActionResult GetGroupsOnTheSubject(int subjectID)
		{
			ViewData["groupsOnTheSubject"] = (new Subject(subjectID)).GetGroups();
			return View();
		}

		/// <summary>
		/// Возвращает всех студентов группы у которых преподается определенный предмет
		/// </summary>
		/// <param name="groupID">Идентификатор группы</param>
		/// <param name="subjectID">Идентификатор предмета</param>
		/// <returns>Частичное представление</returns>
		public ActionResult GetStudentsFromGroupOnTheSubject(int groupID, int subjectID)
		{
			ViewData["studentsFromGroupOnTheSubject"] = subjectID == -1 ? (new Group(groupID)).GetStudent() : (new Subject(subjectID)).GetStudentsFromGroup(groupID);
			return View();
		}

		/// <summary>
		/// Возвращает все группы которым преподает определенны преподаватель
		/// </summary>
		/// <param name="userID">Идентификатор пользователя - преподователя</param>
		/// <returns>Частичное представление</returns>
		public ActionResult GetGroupsOnTheLecturer(int userID)
		{
			ViewData["groupsOnTheLecturer"] = (new Lecturer(new Users(userID))).GetMyGroups();
			return View();
		}

		/// <summary>
		/// Возвращает всех студентов которым определенны преподователь преподает определенный предмет
		/// </summary>
		/// <param name="userID">Идентификатор пользователя - преподователя</param>
		/// <param name="subjectID">Идентификатор предмета. Если он равен -1 то выборка будет осуществлена по всем предметам преподователя</param>
		/// <returns>Частичное представление</returns>
		public ActionResult GetStudentsOnTheLecturer(int userID, int subjectID)
		{
			Group[] groups = subjectID == -1 ? (new Lecturer(new Users(userID))).GetMyGroups() : (new Subject(subjectID)).GetGroups();
			ViewData["studentsOnTheLecturer"] = (new Lecturer(new Users(userID))).GetMyStudents(groups);
			return View();
		}

		/// <summary>
		/// Возвращает все оценки выставленные преподователем
		/// </summary>
		/// <param name="userID">Идентификатор пользователя - преподователя</param>
		/// <param name="subjectsID">Идентификаторы предметов</param>
		/// <param name="groupsID">Идентификаторы групп</param>
		/// <param name="studentsID">Идентификаторы студентов</param>
		/// <returns>Частичное представление</returns>
		public ActionResult ShowMarkLecturer(int userID, string subjectsID, string groupsID, string studentsID)
		{
			Marks[] result = null;
			DB db = new DB();
			DB.ResponseTable marksTable = db.QueryToRespontTable(string.Format("select marks.Mark_id from groups inner join student inner join studentsubject inner join marks on groups.Group_id = student.Group_id and student.Student_id = studentsubject.Student_id and marks.StudentSubject_id = studentsubject.StudentSubject_id and groups.Group_id in {0} and student.Student_id in {1} and studentsubject.Subject_id in {2} order by groups.Name;", groupsID, studentsID, subjectsID));
			if (marksTable != null && marksTable.CountRow != 0)
			{
				result = new Marks[marksTable.CountRow];
				for (int i = 0, end = result.Length; i < end && marksTable.Read(); i++)
				{
					result[i] = new Marks(Convert.ToInt32(marksTable["Mark_id"]));
					result[i].GetInformationAboutUserFromDB();
				}
			}
			ViewData["showMark"] = result;
			return View();
		}

		/// <summary>
		/// Возвращает все оценки полученные студентом
		/// </summary>
		/// <param name="userID">Идентификатор пользователя - студента</param>
		/// <param name="subjectsID">Идентификаторы предметов</param>
		/// <returns>Частичное представление</returns>
		public ActionResult ShowMarkStudent(int userID, string subjectsID)
		{
			Marks[] result = null;
			DB db = new DB();
			DB.ResponseTable marksTable = db.QueryToRespontTable(string.Format("select marks.Mark_id from users inner join student inner join studentsubject inner join marks on users.Student_id = student.Student_id and student.Student_id = studentsubject.Student_id and studentsubject.StudentSubject_id = marks.StudentSubject_id and users.User_id = {0} and studentsubject.Subject_id in {1};", userID, subjectsID));
			if (marksTable != null && marksTable.CountRow != 0)
			{
				result = new Marks[marksTable.CountRow];
				for (int i = 0, end = result.Length; i < end && marksTable.Read(); i++)
				{
					result[i] = new Marks(Convert.ToInt32(marksTable["Mark_id"]));
					result[i].GetInformationAboutUserFromDB();
				}
			}
			ViewData["showMark"] = result;
			return View();
		}

		/// <summary>
		/// Возвращает список атестаций
		/// </summary>
		/// <param name="numberAttestation">Номер атестации</param>
		/// <returns>Частичное представление</returns>
		public ActionResult AttestationInTheSubjectGroupsLecturer(int numberAttestation)
		{
			Examination[] result = new Examination[0];
			string[] tempEl = Request.Form["subjectsID"].Split(',');
			Subject[] subjects = new Subject[tempEl.Length];
			for (int i = 0; i < tempEl.Length; i++)
			{
				subjects[i] = new Subject(Convert.ToInt32(tempEl[i]));
				subjects[i].GetInformationAboutUserFromDB();
			}
			tempEl = Request.Form["groupsID"].Split(',');
			Group[] groups = new Group[tempEl.Length];
			int[] groupsID = new int[groups.Length];
			for (int i = 0; i < tempEl.Length; i++)
			{
				groups[i] = new Group(Convert.ToInt32(tempEl[i]));
				groupsID[i] = groups[i].ID;
				groups[i].GetInformationAboutUserFromDB();
			}
			for (int i = 0, end = subjects.Length; i < end; i++)
			{
				Examination[] temp = subjects[i].GetAttestation(numberAttestation, groupsID);
				if (temp != null)
				{
					int lastEl = result.Length;
					Array.Resize(ref result, result.Length + temp.Length);
					Array.Copy(temp, 0, result, lastEl, temp.Length);
				}
			}
			ViewData["attestation"] = result;
			ViewData["subjects"] = subjects;
			ViewData["groups"] = groups;
			ViewData["numberAttestation"] = numberAttestation;
			return View();
		}

		/// <summary>
		/// Возвращает список атестаций студента
		/// </summary>
		/// <param name="userID">Идентификатор пользователя - студента</param>
		/// <param name="numberAttestation">Номер атестации</param>
		/// <returns>Частичное представление</returns>
		public ActionResult AttestationInTheSubjectGroupsStudent(int userID, int numberAttestation)
		{
			string[] tempEl = Request.Form["subjectsID"].Split(',');
			Subject[] subjects = new Subject[tempEl.Length];
			int[] subjectsID = new int[tempEl.Length];
			for (int i = 0; i < tempEl.Length; i++)
			{
				subjects[i] = new Subject(Convert.ToInt32(tempEl[i]));
				subjectsID[i] = subjects[i].ID;
				subjects[i].GetInformationAboutUserFromDB();
			}
			ViewData["attestation"] = (new Student(new Users(userID))).GetAttestation(numberAttestation, subjectsID);
			ViewData["subjects"] = subjects;
			ViewData["numberAttestation"] = numberAttestation;
			return View();
		}

		/// <summary>
		/// Выводит всех студентов соответствующийх запросу
		/// </summary>
		/// <param name="searchType">Тип запроса</param>
		/// <returns>Частичное представление</returns>
		public ActionResult SearchStudent(int searchType)
		{
			Student[] result = null;
			string[] tempEl = Request.Form["subjectsID"].Split(',');
			Subject[] subjects = new Subject[tempEl.Length];
			string subjectsID = "(";
			for (int i = 0; i < tempEl.Length; i++)
			{
				subjects[i] = new Subject(Convert.ToInt32(tempEl[i]));
				subjectsID += subjects[i].ID + (i == tempEl.Length - 1 ? "" : ",");
				subjects[i].GetInformationAboutUserFromDB();
			}
			tempEl = Request.Form["groupsID"].Split(',');
			Group[] groups = new Group[tempEl.Length];
			string groupsID = "(";
			for (int i = 0; i < tempEl.Length; i++)
			{
				groups[i] = new Group(Convert.ToInt32(tempEl[i]));
				groupsID += groups[i].ID + (i == tempEl.Length - 1 ? "" : ",");
				groups[i].GetInformationAboutUserFromDB();
			}
			groupsID += ")";
			subjectsID += ")";
			string query = "";
			switch (searchType)
			{
				case 1:
					query = string.Format("(select student.Student_id from groups inner join student inner join studentsubject on groups.Group_id = student.Group_id and student.Student_id = studentsubject.Student_id where groups.Group_id in {0} and studentsubject.Subject_id = {1} order by groups.Name, student.Surname limit 10000)", groupsID, subjects[0].ID);
					for (int i = 1; i < subjects.Length; i++)
					{
						query += "union all" + string.Format("(select student.Student_id from groups inner join student inner join studentsubject on groups.Group_id = student.Group_id and student.Student_id = studentsubject.Student_id where groups.Group_id in {0} and studentsubject.Subject_id = {1} order by groups.Name, student.Surname limit 10000)",groupsID, subjects[i].ID);
					}
					break;
				case 2:
					query = string.Format("select t1.Student_id, t1.Subject_id, t1.Examination_id from (select student.Student_id, student.Surname, studentsubject.Subject_id, examination.Examination_id, groups.Name from groups inner join student inner join studentsubject inner join examination on groups.Group_id =  student.Group_id and student.Student_id = studentsubject.Student_id and studentsubject.StudentSubject_id = examination.StudentSubject_id where groups.Group_id in {0} and studentsubject.Subject_id in {1} and examination.Exam_type = 'атестація1' and examination.Mark < examination.Min_mark order by student.Student_id, studentsubject.Subject_id, examination.Examination_id) as t1 inner join (select student.Student_id, student.Surname, studentsubject.Subject_id, examination.Examination_id, groups.Name from groups inner join student inner join studentsubject inner join examination on groups.Group_id = student.Group_id and student.Student_id = studentsubject.Student_id and studentsubject.StudentSubject_id = examination.StudentSubject_id where groups.Group_id in {0} and studentsubject.Subject_id in {1} and examination.Exam_type = 'атестація2' and examination.Mark < examination.Min_mark order by student.Student_id, studentsubject.Subject_id, examination.Examination_id) as t2 on t1.Student_id = t2.Student_id and t1.Subject_id = t2.Subject_id order by t1.Name, t1.Surname;", groupsID, subjectsID);
					break;
				case 3:
					query = string.Format("select student.Student_id from student inner join groups on student.Group_id = groups.Group_id where student.Employment_in_the_department <> '' and groups.Group_id in {0} order by groups.Name, student.Surname;", groupsID);
					break;
				case 4:
					query = string.Format("select student.Student_id from student inner join studentsubject inner join marks on student.Student_id = studentsubject.Student_id and studentsubject.StudentSubject_id = marks.StudentSubject_id where student.Student_id not in (select studentsubject.Student_id from marks inner join studentsubject on marks.StudentSubject_id = studentsubject.StudentSubject_id where marks.Mark <> marks.Max_mark)  and student.Group_id in {0};", groupsID);
					break;
				case 5:
					KickedSession();
					return View("KickedSession");
			}
			DB.ResponseTable studentsID = (new DB()).QueryToRespontTable(query);
			if (searchType == 2 && studentsID != null)
			{
				Examination[] examinations = new Examination[studentsID.CountRow];
				for (int i = 0, end = examinations.Length; i < end && studentsID.Read(); i++)
				{
					examinations[i] = new Examination(Convert.ToInt32(studentsID["Examination_id"]));
					examinations[i].GetInformationAboutUserFromDB();
				}
				ViewData["examinations"] = examinations;
			}
			if (studentsID != null)
			{
				studentsID.GoToStatrTable();
				result = new Student[studentsID.CountRow];
				for (int i = 0, end = result.Length; i < end && studentsID.Read(); i++)
				{
					result[i] = new Student(Convert.ToInt32(studentsID["Student_id"]));
					result[i].GetInformationAboutUserFromDB();
				}
			}
			ViewData["searchType"] = searchType;
			ViewData["students"] = result;
			ViewData["subjects"] = subjects;
			ViewData["groups"] = groups;
			return View();
		}

		/// <summary>
		/// Выводит результат сессии по предмету определенной группы
		/// </summary>
		/// <param name="subjectID">Идентификатор предмета</param>
		/// <param name="groupID">Идентификатор группы</param>
		/// <param name="typeSession">Тип сессии</param>
		/// <returns>Частичное представление</returns>
		public ActionResult SessionResultLecturer(int subjectID, int groupID, string typeSession)
		{
			Examination[] result = null;
			DB.ResponseTable examID = (new DB()).QueryToRespontTable(string.Format("select examination.Examination_id from student inner join studentsubject inner join examination on student.Student_id = studentsubject.Student_id and studentsubject.StudentSubject_id = examination.StudentSubject_id where student.Group_id = {0} and studentsubject.Subject_id = {1} and examination.Exam_type = '{2}';", groupID, subjectID, typeSession));
			if (examID != null)
			{
				result = new Examination[examID.CountRow];
				for (int i = 0; i < result.Length && examID.Read(); i++)
				{
					result[i] = new Examination(Convert.ToInt32(examID["Examination_id"]));
					result[i].GetInformationAboutUserFromDB();
				}
			}
			ViewData["sessionResult"] = result;
			return View();
		}

		/// <summary>
		/// Выводит оценки по сессии студента по заданным предметам
		/// </summary>
		/// <param name="userID">Идентификатор пользователя - студента</param>
		/// <param name="typeSession">Тип сессии</param>
		/// <returns>Частичное представление</returns>
		public ActionResult SessionResultStudent(int userID, string typeSession)
		{
			string[] tempEl = Request.Form["subjectsID"].Split(',');
			Subject[] subjects = new Subject[tempEl.Length];
			string subjectsID = "(";
			for (int i = 0; i < tempEl.Length; i++)
			{
				subjects[i] = new Subject(Convert.ToInt32(tempEl[i]));
				subjectsID += subjects[i].ID + (i == tempEl.Length - 1 ? "" : ",");
				subjects[i].GetInformationAboutUserFromDB();
			}
			subjectsID += ")";
			Examination[] result = null;
			DB.ResponseTable examID = (new DB()).QueryToRespontTable(string.Format("select examination.Examination_id from users inner join student inner join studentsubject inner join examination on users.Student_id = student.Student_id and student.Student_id = studentsubject.Student_id and examination.StudentSubject_id = studentsubject.StudentSubject_id where users.User_id = {0} and studentsubject.Subject_id in {1} and examination.Exam_type = '{2}';", userID, subjectsID, typeSession));
			if (examID != null)
			{
				result = new Examination[examID.CountRow];
				for (int i = 0; i < result.Length && examID.Read(); i++)
				{
					result[i] = new Examination(Convert.ToInt32(examID["Examination_id"]));
					result[i].GetInformationAboutUserFromDB();
				}
			}
			ViewData["sessionResult"] = result;
			return View();
		}

		/// <summary>
		/// Выводит форму для выставления сесии
		/// </summary>
		/// <param name="subjectID">Идентификатор предмета</param>
		/// <param name="groupID">Идентификатор группы</param>
		/// <param name="typeSession">Тип сессии</param>
		/// <returns>Частичное представление</returns>
		public ActionResult ShowSetSession(int subjectID, int groupID, string typeSession)
		{
			string limit;
			Student[] result = null;
			switch (typeSession)
			{
				case "іспит":
					limit = "('іспит')";
					break;
				case "пересдача1":
					limit = "('іспит', 'пересдача1')";
					break;
				case "пересдача2":
					limit = "('іспит', 'пересдача1', 'пересдача2')";
					break;
				default:
					limit = "('іспит')";
					break;
			}
			DB.ResponseTable studentsID = (new DB()).QueryToRespontTable(string.Format("select student.Student_id from student inner join studentsubject on student.Student_id = studentsubject.Student_id where student.Group_id = {0} and studentsubject.Subject_id = {1} and student.Student_id not in (select student.Student_id from student inner join studentsubject inner join examination on student.Student_id = studentsubject.Student_id and studentsubject.StudentSubject_id = examination.StudentSubject_id where student.Group_id = {0} and studentsubject.Subject_id = {1} and examination.Exam_type in {2});", groupID, subjectID, limit));
			if (studentsID != null)
			{
				result = new Student[studentsID.CountRow];
				for (int i = 0; i < studentsID.CountRow && studentsID.Read(); i++)
				{
					result[i] = new Student(Convert.ToInt32(studentsID["Student_id"]));
					result[i].GetInformationAboutUserFromDB();
				}
			}
			ViewData["students"] = result;
			return View();
		}

		/// <summary>
		/// Выводит студентов которых могут выгнать по результатам сессии
		/// </summary>
		/// <returns>Частичное представление</returns>
		public ActionResult KickedSession()
		{
			Student[] students = null;
			Examination[] examinations = null;
			string[] tempEl = Request.Form["subjectsID"].Split(',');
			Subject[] subjects = new Subject[tempEl.Length];
			string subjectsID = "(";
			for (int i = 0; i < tempEl.Length; i++)
			{
				subjects[i] = new Subject(Convert.ToInt32(tempEl[i]));
				subjectsID += subjects[i].ID + (i == tempEl.Length - 1 ? "" : ",");
				subjects[i].GetInformationAboutUserFromDB();
			}
			tempEl = Request.Form["groupsID"].Split(',');
			Group[] groups = new Group[tempEl.Length];
			string groupsID = "(";
			for (int i = 0; i < tempEl.Length; i++)
			{
				groups[i] = new Group(Convert.ToInt32(tempEl[i]));
				groupsID += groups[i].ID + (i == tempEl.Length - 1 ? "" : ",");
				groups[i].GetInformationAboutUserFromDB();
			}
			groupsID += ")";
			subjectsID += ")";
			DB.ResponseTable examResult = (new DB()).QueryToRespontTable(string.Format("select student.Student_id, examination.Examination_id from groups inner join student inner join studentsubject inner join examination on groups.Group_id = student.Group_id and student.Student_id = studentsubject.Student_id and studentsubject.StudentSubject_id = examination.StudentSubject_id where student.Group_id in {0} and studentsubject.Subject_id in {1} and examination.Exam_type in ('іспит', 'пересдача1', 'пересдача2') and examination.Mark/examination.Min_mark < 0.6 order by groups.Name, student.Surname;", groupsID, subjectsID));
			if (examResult != null)
			{
				students = new Student[examResult.CountRow];
				examinations = new Examination[examResult.CountRow];
				for (int i = 0; i < examResult.CountRow && examResult.Read(); i++)
				{
					students[i] = new Student(Convert.ToInt32(examResult["Student_id"]));
					students[i].GetInformationAboutUserFromDB();
					examinations[i] = new Examination(Convert.ToInt32(examResult["Examination_id"]));
					examinations[i].GetInformationAboutUserFromDB();
				}
			}
			ViewData["students"] = students;
			ViewData["examinations"] = examinations;
			ViewData["subjects"] = subjects;
			ViewData["groups"] = groups;
			return View();
		}
	}
}
