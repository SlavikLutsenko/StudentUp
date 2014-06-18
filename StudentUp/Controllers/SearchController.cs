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

		public ActionResult GetGroupsOnTheSubject(int subjectID)
		{
			ViewData["groupsOnTheSubject"] = (new Subject(subjectID)).GetGroups();
			return View();
		}

		public ActionResult GetStudentsFromGroupOnTheSubject(int groupID, int subjectID)
		{
			ViewData["studentsFromGroupOnTheSubject"] = subjectID == -1 ? (new Group(groupID)).GetStudent() : (new Subject(subjectID)).GetStudentsFromGroup(groupID);
			return View();
		}

		public ActionResult GetGroupsOnTheLecturer(int userID)
		{
			ViewData["groupsOnTheLecturer"] = (new Lecturer(new Users(userID))).GetMyGroups();
			return View();
		}

		public ActionResult GetStudentsOnTheLecturer(int userID, int subjectID)
		{
			Group[] groups = subjectID == -1 ? (new Lecturer(new Users(userID))).GetMyGroups() : (new Subject(subjectID)).GetGroups();
			ViewData["studentsOnTheLecturer"] = (new Lecturer(new Users(userID))).GetMyStudents(groups);
			return View();
		}

		public ActionResult ShowMark(int userID, string subjectsID, string groupsID, string studentsID)
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
	}
}
