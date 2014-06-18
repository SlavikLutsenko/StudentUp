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
	}
}
