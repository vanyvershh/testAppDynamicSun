using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Diagnostics;
using testAppDynamicSun.Domain;
using testAppDynamicSun.Models;

namespace testAppDynamicSun.Controllers
{
    public class HomeController : Controller
    {

        private readonly WeatherValuesRepository weatherValuesRepository;
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;
        public HomeController(WeatherValuesRepository weatherValuesRepository, Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment)
        {
            this.weatherValuesRepository = weatherValuesRepository;
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult WeatherLoad()
        {
            return View();
        }
        public IActionResult WeatherView()
        {
            //выбираем все записи из БД и передаем их в представление
            var model = weatherValuesRepository.GetWeatherValues();
            ViewData["Title1"] = "Данные за всё время:";
            return View(model);
        }

        [HttpPost]
        public IActionResult WeatherView(DateTime month, DateTime year)
        {
            if (month.Year > 1) //если получили ввод из месяца, то выгружаем данные за месяц
            {
                var startDate = month; //первый день месяца
                //var endDate = new DateTime(startDate.Year, startDate.Month + 1, 1).AddDays(-1); //последний день месяца
                var endDate = new DateTime(startDate.Year, startDate.Month, DateTime.DaysInMonth(startDate.Year, startDate.Month));
                var model = weatherValuesRepository.GetIntervalWeatherValues(startDate, endDate);
                ViewData["Title1"] = $"Данные за {month:Y}" + ":";

                return View(model);
            }
            else //если получили данные из года, то выгружаем данные за год
            {
                var startDate = new DateTime(year.Year, 1, 1); //первый день года
                var endDate = new DateTime(startDate.Year + 1, 1, 1).AddDays(-1); //последний день года
                var model = weatherValuesRepository.GetIntervalWeatherValues(startDate, endDate);
                ViewData["Title1"] = "Данные за " + year.Year + " год:";
                return View(model);
            }


        }


        [HttpPost] //т.к. удаление WeatherValue изменяет состояние приложения, нельзя использовать метод GET
        public IActionResult WeatherValuesDelete(Guid id)
        {
            weatherValuesRepository.DeleteWeatherValue(new WeatherValue() { Id = id });
            return RedirectToAction("WeatherView");
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public ActionResult Import(IFormFileCollection filesExcel)
        {
            if (ModelState.IsValid)
            {
                foreach (IFormFile fileExcel in filesExcel)
                {
                    string folderName = "Upload";
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    string newPath = Path.Combine(webRootPath, folderName);
                    if (!Directory.Exists(newPath))
                    {
                        Directory.CreateDirectory(newPath);
                    }
                    if (fileExcel.Length > 0)
                    {
                        string sFileExtension = Path.GetExtension(fileExcel.FileName).ToLower();
                        List<ISheet> sheets = new List<ISheet>(); //страницы в таблице
                        string fullPath = Path.Combine(newPath, fileExcel.FileName);
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            fileExcel.CopyTo(stream);
                            stream.Position = 0;

                            if (sFileExtension == ".xls")
                            {
                                HSSFWorkbook workbook = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
                                for (int i = 0; i < workbook.NumberOfSheets; i++)
                                {
                                    sheets.Add(workbook.GetSheetAt(i));
                                }
                            }
                            else
                            {
                                XSSFWorkbook workbook = new XSSFWorkbook(stream); //This will read 2007 Excel format  
                                for (int i = 0; i < workbook.NumberOfSheets; i++)
                                {
                                    sheets.Add(workbook.GetSheetAt(i));
                                }
                            }

                        }
                        try
                        {
                            foreach (var sheet in sheets)
                            {
                                for (int i = 4; i <= sheet.LastRowNum; i++) //наинаем со строчки под индексом 4 (5ая в таблице)
                                {
                                    IRow row = sheet.GetRow(i);
                                    WeatherValue weatherValue = new WeatherValue();
                                    weatherValue.Date = DateTime.Parse(row.GetCell(0).StringCellValue);
                                    weatherValue.Time = NullToString(row.GetCell(1));
                                    if (!(weatherValuesRepository.isDuplicateWeatherValue(weatherValue.Date, weatherValue.Time))) //если не дупликат, то продолжаем
                                    {
                                        weatherValue.Temperature = NullToString(row.GetCell(2));
                                        weatherValue.Humidity = NullToString(row.GetCell(3));
                                        weatherValue.Td = NullToString(row.GetCell(4));
                                        weatherValue.Pressure = NullToString(row.GetCell(5));
                                        weatherValue.WindDirection = NullToString(row.GetCell(6));
                                        weatherValue.WindSpeed = NullToString(row.GetCell(7));
                                        weatherValue.Cloudiness = NullToString(row.GetCell(8));
                                        weatherValue.CloudBase = NullToString(row.GetCell(9));
                                        weatherValue.Visibility = NullToString(row.GetCell(10));
                                        weatherValue.Conditions = NullToString(row.GetCell(11)); ;
                                        weatherValuesRepository.SaveWeatherValue(weatherValue);
                                    }


                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            return RedirectToAction("WeatherLoadError");
                        }


                    }
                }
            }
            return RedirectToAction("WeatherView"); //после загрузки открываем страицу просмотра
        }

        public IActionResult WeatherLoadError()
        {
            return View();
        }

        public string NullToString(object value)
        {
            return value == null ? " " : value.ToString();
        }
    }
}