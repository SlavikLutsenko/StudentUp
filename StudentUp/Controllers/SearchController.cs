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
    }
}
