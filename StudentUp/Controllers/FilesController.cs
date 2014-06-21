using System;
using System.IO;
using System.Web.Mvc;
using Microsoft.Office.Interop.Excel;
using StudentUp.Models;

namespace StudentUp.Controllers
{
	/// <summary>
	/// Контролер предназначен для создания и отправки файлов клиенту
	/// </summary>
	public class FilesController : Controller
	{
		/// <summary>
		/// Возвращает MIME-тип файла
		/// </summary>
		/// <param name="fileExtension">разширение файла</param>
		/// <returns>MIME-тип файла</returns>
		private string GetFileExtension(string fileExtension)
		{
			switch (fileExtension)
			{
				case ".htm":
				case ".html":
				case ".log":
					return "text/HTML";
				case ".txt":
					return "text/plain";
				case ".doc":
					return "application/ms-word";
				case ".zip":
					return "application/zip";
				case ".xls":
				case ".csv":
					return "application/vnd.ms-excel";
				case ".pdf":
					return "application/pdf";
				case ".xml":
				case ".sdxl":
					return "application/xml";
				default:
					return "application/octet-stream";
			}
		}

		public ActionResult Download(string fileName)
		{
			string fullPath = Path.Combine(Server.MapPath("~/Files"), fileName);
			return File(fullPath, GetFileExtension(fileName), fileName);
		}

		public void DeleteFile(string fileName)
		{
			System.IO.File.Delete(Server.MapPath("~/Files") + "\\" + fileName);
		}

		public string Attestation(int numberAttestation)
		{
			string fileName = "attestation" + numberAttestation + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".xlsx";
			DeleteFile(fileName);
			DB db = new DB();
			DB.ResponseTable groupsID = db.QueryToRespontTable("select groups.Group_id from groups order by groups.Name;");
			if (groupsID != null)
			{
				//Приложение самого Excel
				Microsoft.Office.Interop.Excel.Application ObjExcel = new Microsoft.Office.Interop.Excel.Application();
				ObjExcel.SheetsInNewWorkbook = groupsID.CountRow;
				//Книга.
				Microsoft.Office.Interop.Excel.Workbook ObjWorkBook = ObjExcel.Workbooks.Add(System.Reflection.Missing.Value);
				for (int i = 0, end = groupsID.CountRow; i < end && groupsID.Read(); i++)
				{
					Worksheet currentWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)ObjWorkBook.Sheets[i + 1];
					Group currentGroup = new Group(Convert.ToInt32(groupsID["Group_id"]));
					currentGroup.GetInformationAboutUserFromDB();
					currentWorksheet.Name = currentGroup.Name;
					Student[] students = currentGroup.GetStudent();
					Subject[] subjectsStudents = currentGroup.GetSubjects();
					Microsoft.Office.Interop.Excel.Range headCell = currentWorksheet.Range["A1", (char)(65 + subjectsStudents.Length + 1) + "1"];
					headCell.Merge();
					headCell.ColumnWidth = (35 + 10 * subjectsStudents.Length) / subjectsStudents.Length;
					headCell.RowHeight = 75;
					headCell.Value2 = string.Format("Національний технічний університет України \"Київський політехнічний інститут\"\n Факультет інформатики та обчислювальної техніки    \n АТЕСТАЦІЙНА ВІДОМОСТЬ      Група  {0}        курс{1}", currentGroup.Name, 1);

					currentWorksheet.Range["A3"].RowHeight = 15;
					currentWorksheet.Range["A4"].RowHeight = 105;

					currentWorksheet.Range["A3", "A4"].Merge();
					currentWorksheet.Range["A3", "A4"].Value2 = "№\nп/п";
					currentWorksheet.Range["A3", "A4"].ColumnWidth = 4;

					currentWorksheet.Range["B3", "B4"].Merge();
					currentWorksheet.Range["B3", "B4"].Value2 = "Прізвище та ініціали студента";
					currentWorksheet.Range["B3", "B4"].ColumnWidth = 25;

					for (int j = 0; j < students.Length; j++)
					{
						currentWorksheet.Cells[j + 5, 1] = j + 1;
						currentWorksheet.Cells[j + 5, 2] = students[j].ShortName;
					}

					int numberOffset = 3, numberExam = subjectsStudents.Length + 2;
					for (int j = 0; j < subjectsStudents.Length; j++)
						if (subjectsStudents[j].TypeExam != Subject.ExamType.exam)
						{
							currentWorksheet.Range[(char)(65 + numberOffset - 1) + "4"].Value2 = subjectsStudents[j].Name;
							Examination[] examinations = subjectsStudents[j].GetAttestation(1, new[] { currentGroup.ID });
							if (examinations != null)
								for (int k = 0; k < examinations.Length; k++)
									currentWorksheet.Range[(char)(65 + numberOffset - 1) + (5 + k).ToString()].Value2 = examinations[k].Mark < examinations[k].MinMark ? "н/з" : "з";
							numberOffset++;
						}
						else
						{
							currentWorksheet.Range[(char)(65 + numberExam - 1) + "4"].Value2 = subjectsStudents[j].Name;
							Examination[] examinations = subjectsStudents[j].GetAttestation(numberAttestation, new[] { currentGroup.ID });
							if (examinations != null)
								for (int k = 0; k < examinations.Length; k++)
									currentWorksheet.Range[(char)(65 + numberExam - 1) + (5 + k).ToString()].Value2 = examinations[k].Mark < examinations[k].MinMark ? "н/з" : "з";
							numberExam--;
						}

					currentWorksheet.Range["C3", (char)(65 + numberOffset - 1) + "3"].Merge();
					currentWorksheet.Range["C3", (char)(65 + numberOffset - 1) + "3"].Value2 = "залікові дисципліни";
					currentWorksheet.Range[(char)(65 + numberExam + 1) + "3", (char)(65 + subjectsStudents.Length + 1) + "3"].Merge();
					currentWorksheet.Range[(char)(65 + numberExam + 1) + "3", (char)(65 + subjectsStudents.Length + 1) + "3"].Value2 = "екзаменац.дисцип.";
				}
				ObjExcel.DisplayAlerts = false;
				ObjWorkBook.SaveAs(Server.MapPath("~/Files") + "\\" + fileName);
				ObjExcel.DisplayAlerts = true;
				//Закрытие книгу Excel.
				ObjWorkBook.Close();
				//Закрытие приложения Excel.
				ObjExcel.Quit();
				return fileName;
			}
			return fileName;
		}

	}
}
