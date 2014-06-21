using System;
using System.IO;
using System.Web.Mvc;
using Microsoft.Office.Interop.Excel;
using StudentUp.Models;

using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;

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
				case ".docx":
					return "application/ms-word";
				case ".zip":
					return "application/zip";
				case ".xls":
				case ".xlsx":
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

		/// <summary>
		/// Дает пользователю загрузить файл с сервера
		/// </summary>
		/// <param name="fileName">Имя файла</param>
		/// <returns>Файл</returns>
		public ActionResult Download(string fileName)
		{
			string fullPath = Path.Combine(Server.MapPath("~/Files"), fileName);
			return File(fullPath, GetFileExtension("." + fileName.Split('.')[1]), fileName);
		}

		/// <summary>
		/// Удаляет файл с сервера
		/// </summary>
		/// <param name="fileName">Имя файла</param>
		public void DeleteFile(string fileName)
		{
			System.IO.File.Delete(Server.MapPath("~/Files") + "\\" + fileName);
		}

		/// <summary>
		/// Создает файл с атестациями и отправляет его пользователю
		/// </summary>
		/// <param name="numberAttestation">Номер атестации</param>
		/// <returns>Имя файла для загрузки</returns>
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

		/// <summary>
		/// Создает и отправляет пользоваетеляю файл с результатами сесии группы
		/// </summary>
		/// <param name="subjectID">Идентификатор предмета</param>
		/// <param name="groupID">Идентификатор группы</param>
		/// <returns>Имя файла для загрузки</returns>
		public string Session(int subjectID, int groupID)
		{
			string fileName = "session" + "_" + DateTime.Now.ToString("dd_MM_yyyy") + ".docx";
			DeleteFile(fileName);
			Group @group = new Group(groupID);
			group.GetInformationAboutUserFromDB();
			Student[] students = group.GetStudent();
			Subject subject = new Subject(subjectID);
			subject.GetInformationAboutUserFromDB();
			Lecturer lecturer = new Lecturer(subject.LecturerID);
			lecturer.GetInformationAboutUserFromDB();
			Examination[] examinations = subject.GetSession(group.ID);
			if (examinations == null) return null;
			Microsoft.Office.Interop.Word.Application applicationWord = new Microsoft.Office.Interop.Word.Application();

			applicationWord.Documents.Add(Type.Missing, false, Microsoft.Office.Interop.Word.WdNewDocumentType.wdNewBlankDocument, false);

			Microsoft.Office.Interop.Word.Document documentWord = applicationWord.Documents.get_Item(1);
			documentWord.Activate();

			//applicationWord.Visible = true;

			Microsoft.Office.Interop.Word.Paragraph documentParagraph = documentWord.Paragraphs.First;
			documentParagraph.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphRight;
			documentParagraph.Range.Font.Bold = 1;
			documentParagraph.Range.Text = "Іспит";

			documentParagraph = documentWord.Paragraphs.Add().Next();
			documentParagraph.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
			documentParagraph.Range.Font.Bold = 0;
			documentParagraph.Range.Text = "Національний технічний університет України \"Київський політехнічний інститут\"";

			documentParagraph = documentWord.Paragraphs.Add().Next();
			documentParagraph.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
			documentParagraph.Range.Font.Bold = 1;
			documentParagraph.Range.Text = "Факультет інформатики та обчислювальної техніки";

			documentParagraph = documentWord.Paragraphs.Add().Next();
			documentParagraph.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
			documentParagraph.Range.Font.Bold = 0;
			documentParagraph.Range.Text = group.Name;

			documentParagraph = documentWord.Paragraphs.Add().Next();
			documentParagraph.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
			documentParagraph.Range.Font.Bold = 1;
			documentParagraph.Range.Text = "Заліково-екзаменаційна відомость № " + (new Random()).Next(20, 700);

			documentParagraph = documentWord.Paragraphs.Add().Next();
			documentParagraph.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
			documentParagraph.Range.Font.Bold = 0;
			documentParagraph.Range.Font.Underline = Microsoft.Office.Interop.Word.WdUnderline.wdUnderlineSingle;
			documentParagraph.Range.Text = subject.Name;

			documentParagraph = documentWord.Paragraphs.Add().Next();
			documentParagraph.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
			documentParagraph.Range.Font.Bold = 0;
			documentParagraph.Range.Font.Underline = Microsoft.Office.Interop.Word.WdUnderline.wdUnderlineNone;
			documentParagraph.Range.Text = "за " + students[0].CurrentSemester + " навчальний семетр" + "\t\t Дата _____________";

			documentParagraph = documentWord.Paragraphs.Add().Next();
			documentParagraph.Range.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
			documentParagraph.Range.Font.Bold = 0;
			documentParagraph.Range.Text = "Екзаменатор  " + lecturer.ShortName;

			documentParagraph = documentWord.Paragraphs.Add().Next();
			Microsoft.Office.Interop.Word.Table table = documentWord.Tables.Add(documentParagraph.Range, students.Length + 2, 7);
			Microsoft.Office.Interop.Word.Border[] borders = new Microsoft.Office.Interop.Word.Border[6];//массив бордеров
			borders[0] = table.Borders[Microsoft.Office.Interop.Word.WdBorderType.wdBorderLeft];//левая граница 
			borders[1] = table.Borders[Microsoft.Office.Interop.Word.WdBorderType.wdBorderRight];//правая граница 
			borders[2] = table.Borders[Microsoft.Office.Interop.Word.WdBorderType.wdBorderTop];//нижняя граница 
			borders[3] = table.Borders[Microsoft.Office.Interop.Word.WdBorderType.wdBorderBottom];//верхняя граница
			borders[4] = table.Borders[Microsoft.Office.Interop.Word.WdBorderType.wdBorderHorizontal];//горизонтальная граница
			borders[5] = table.Borders[Microsoft.Office.Interop.Word.WdBorderType.wdBorderVertical];//вертикальная граница
			foreach (Microsoft.Office.Interop.Word.Border border in borders)
			{
				border.LineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;//ставим стиль границы 
				border.Color = Microsoft.Office.Interop.Word.WdColor.wdColorBlack;//задаем цвет границы
			}
			documentWord.Range(table.Cell(1, 1).Range.Start, table.Cell(2, 1).Range.End).Select();
			applicationWord.Selection.Cells.Merge();
			documentWord.Range(table.Cell(1, 2).Range.Start, table.Cell(2, 2).Range.End).Select();
			applicationWord.Selection.Cells.Merge();
			documentWord.Range(table.Cell(1, 3).Range.Start, table.Cell(2, 3).Range.End).Select();
			applicationWord.Selection.Cells.Merge();
			documentWord.Range(table.Cell(1, 5).Range.Start, table.Cell(1, 6).Range.End).Select();
			applicationWord.Selection.Cells.Merge();
			documentWord.Range(table.Cell(1, 6).Range.Start, table.Cell(2, 7).Range.End).Select();
			applicationWord.Selection.Cells.Merge();

			table.Cell(1, 1).Range.Text = "№\nз/п";
			table.Cell(1, 2).Range.Text = "Прізвище, ініціали студента";
			table.Cell(1, 3).Range.Text = "№ залікової книжки";
			table.Cell(1, 4).Range.Text = "Рейтингові бали";
			table.Cell(2, 4).Range.Text = "Всього";
			table.Cell(1, 5).Range.Text = "Результат";
			table.Cell(2, 5).Range.Text = "Оцінка ECTS";
			table.Cell(2, 6).Range.Text = "Традиційна оцінка";
			table.Cell(1, 6).Range.Text = "Підпис виклад.";

			for (int i = 0, end = students.Length; i < end; i++)
			{
				table.Cell(i + 3, 1).Range.Text = (i + 1).ToString();
				table.Cell(i + 3, 2).Range.Text = students[i].ShortName;
				table.Cell(i + 3, 3).Range.Text = students[i].RecordBook;
				table.Cell(i + 3, 4).Range.Text = examinations[i].Mark.ToString();
				table.Cell(i + 3, 5).Range.Text = Marks.ToBolognaSystem(examinations[i].Mark,100);
				table.Cell(i + 3, 6).Range.Text = Marks.ToTraditional(Marks.ToBolognaSystem(examinations[i].Mark, 100));
			}

			documentWord.SaveAs(Server.MapPath("~/Files") + "\\" + fileName);
			documentWord.Close();
			applicationWord.Quit();
			return fileName;
		}

	}
}
