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
		/// <returns>Страница</returns>
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
		/// <returns>Страница</returns>
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
	}
}
